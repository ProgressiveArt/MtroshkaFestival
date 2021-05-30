using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EnumsNET;
using MetroshkaFestival.Application.Commands.Records.Tournaments;
using MetroshkaFestival.Application.Queries.Models.Tournaments;
using MetroshkaFestival.Application.Services;
using MetroshkaFestival.Core.Exceptions.Common;
using MetroshkaFestival.Core.Exceptions.ExceptionCodes;
using MetroshkaFestival.Core.Models.Common;
using MetroshkaFestival.Data;
using MetroshkaFestival.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using Serilog;

namespace MetroshkaFestival.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    [Route("[area]/[controller]/[action]")]
    public class TournamentController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly CityService _cityService;

        public TournamentController(DataContext dataContext, CityService cityService)
        {
            _dataContext = dataContext;
            _cityService = cityService;
        }

        [HttpGet]
        public IActionResult Index([FromQuery] GetTournamentListQueryModel query)
        {
            var tournamentListModel = new TournamentListModel
            {
                Query = query,
                Tournaments = new PagedList<TournamentListItemModel>(
                    Array.Empty<TournamentListItemModel>().AsQueryable(),
                    query.Page.NumPage == 0 ? 1 : query.Page.NumPage,
                    query.Page.RecordsCountPerPage)
            };

            try
            {
                IQueryable<Tournament> tournamentsQuery = _dataContext.Tournaments
                    .Include(x => x.City)
                    .Include(x => x.AgeCategories)
                    .ThenInclude(x => x.Teams);

                if (query.Filter != null)
                {
                    tournamentsQuery = query.Filter.Apply(tournamentsQuery);
                }

                if (query.Sort != null)
                {
                    tournamentsQuery = query.Sort.Apply(tournamentsQuery);
                }

                var tournaments = tournamentsQuery.Select(x => new TournamentListItemModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    YearOfTour = x.YearOfTour,
                    City = x.City,
                    CanBeRemoved = x.AgeCategories.Any(c => c.Teams.Count == 0),
                }).ToArray();

                query.Page.NumPage = Math.Min(query.Page.NumPage,
                    (int) Math.Ceiling((double) tournaments.Length / query.Page.RecordsCountPerPage));
                tournamentListModel = new TournamentListModel
                {
                    Query = query,
                    Tournaments = new PagedList<TournamentListItemModel>(tournaments.AsQueryable(),
                        query.Page.NumPage == 0 ? 1 : query.Page.NumPage,
                        query.Page.RecordsCountPerPage)
                };
            }
            catch (HttpResponseException e)
            {
                if (e.Status == StatusCodes.Status404NotFound)
                {
                    throw;
                }
            }
            catch (Exception e)
            {
                Log.Error("An error occured while getting tournament list", e);
                tournamentListModel.Error = e.Message;
            }

            FillSelected();
            return View(tournamentListModel);
        }

        [HttpGet]
        public async Task<ActionResult> TournamentSummary(GetTournamentSummaryQueryModel query)
        {
            var tournament = await _dataContext.Tournaments
                .Include(x => x.City)
                .Include(x => x.AgeCategories)
                .ThenInclude(x => x.Teams)
                .ThenInclude(x => x.TeamCity)
                .Include(x => x.AgeCategories)
                .ThenInclude(x => x.Teams)
                .ThenInclude(x => x.Players)
                .FirstOrDefaultAsync(x => x.Id == query.TournamentId);

            if (tournament == null)
            {
                throw new HttpResponseException(StatusCodes.Status404NotFound, TournamentExceptionCodes.NotFound);
            }

            var tournamentModel = new TournamentModel
            {
                Query = query,
                Id = tournament.Id,
                TournamentType = tournament.Type,
                Name = tournament.Name,
                YearOfTour = tournament.YearOfTour,
                City = tournament.City,
                Description = tournament.Description,
                ReturnUrl = query.ReturnUrl
            };

            try
            {
                var teams = tournament.AgeCategories.SelectMany(x => x.Teams.Where(t => t.TeamStatus == TeamStatus.Published)).ToArray();
                if (query.Sort != null)
                {
                    teams = query.Sort.Apply(teams).ToArray();
                }

                tournamentModel.AgeCategories = tournament.AgeCategories;
                tournamentModel.Teams = teams;
            }
            catch (Exception e)
            {Log.Error("An error occured while getting tournament summary", e);
                tournamentModel.Error = e.Message;
                return View("Summary", tournamentModel);
            }

            return View("Summary", tournamentModel);
        }

        [HttpGet]
        public ActionResult GetAddTournamentPage(string returnUrl)
        {
            var command = new AddTournamentCommandRecord(returnUrl);
            FillSelected();
            return View("Add", command);
        }

        [HttpPost]
        public async Task<ActionResult> AddTournament([FromForm] AddTournamentCommandRecord command,
            CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                FillSelected();
                return View("Add", command);
            }

            try
            {
                var isAlreadyExists = await _dataContext.Tournaments
                    .Include(x => x.City)
                    .AnyAsync(x => x.Type == command.TournamentType &&
                                   x.City.Id == command.CityId &&
                                   x.YearOfTour == command.YearOfTour, ct);

                if (isAlreadyExists)
                {
                    throw new ApplicationException(TournamentExceptionCodes.AlreadyExist);
                }

                var city = await _dataContext.Cities.FirstOrDefaultAsync(x => x.Id == command.CityId.Value, ct);
                if (city == null)
                {
                    throw new ApplicationException(CityExceptionCodes.UnknownCity);
                }

                var yearOfTour = command.YearOfTour ?? throw new ApplicationException(TournamentExceptionCodes.YearOfTourIsRequired);
                var tournamentType = command.TournamentType ?? throw new ApplicationException(TournamentExceptionCodes.TournamentTypeIsRequired);

                var tournament = new Tournament
                {
                    Type = tournamentType,
                    City = city,
                    YearOfTour = yearOfTour,
                    Description = command.Description,
                };

                await _dataContext.Tournaments.AddAsync(tournament, ct);
                await _dataContext.SaveChangesAsync(ct);

                city.CanBeRemoved = _cityService.CanBeRemoved(city);
                tournament.AgeCategories.Add(new AgeCategory
                {
                    AgeGroup = AgeGroup.Junior,
                    MinBirthDate = new DateTime(yearOfTour - 11, 1, 1),
                    MaxBirthDate = new DateTime(yearOfTour - 10, 12, 31)
                });

                tournament.AgeCategories.Add(new AgeCategory
                {
                    AgeGroup = AgeGroup.Senior,
                    MinBirthDate = new DateTime(yearOfTour - 13, 1, 1),
                    MaxBirthDate = new DateTime(yearOfTour - 12, 12, 31)
                });
                await _dataContext.SaveChangesAsync(ct);
            }
            catch (Exception e)
            {
                Log.Error("An error occured while adding tournament", e);
                ModelState.AddModelError("AddTournament", e.Message);
                FillSelected();
                return View("Add", command);
            }

            return RedirectPermanent(command.ReturnUrl);
        }

        [HttpGet]
        public async Task<ActionResult> DeleteTournament(int tournamentId)
        {
            var tournament = await _dataContext.Tournaments
                .Include(x => x.City)
                .Include(x => x.AgeCategories)
                .ThenInclude(x => x.Teams)
                .FirstOrDefaultAsync(x => x.Id == tournamentId);

            if (tournament == null)
            {
                throw new HttpResponseException(StatusCodes.Status404NotFound, TournamentExceptionCodes.NotFound);
            }

            if (tournament.AgeCategories.Any(x => x.Teams.Any()))
            {
                throw new HttpResponseException(StatusCodes.Status412PreconditionFailed, TournamentExceptionCodes.CanNotBeRemoved);
            }

            var city = tournament.City;
            _dataContext.Tournaments.Remove(tournament);
            await _dataContext.SaveChangesAsync();

            city.CanBeRemoved = _cityService.CanBeRemoved(city);
            await _dataContext.SaveChangesAsync();

            return RedirectToAction("Index", "Tournament");
        }

        private void FillSelected()
        {
            var tournamentTypeNames = Enum.GetValues(typeof(TournamentType))
                .Cast<int>()
                .ToArray();

            var tournamentTypes = tournamentTypeNames.Select(x => new SelectListItem
            {
                Value = x.ToString(),
                Text = $"{((TournamentType) x).AsString(EnumFormat.Description)}"
            });
            ViewBag.TournamentTypes = tournamentTypes;

            var citiesDictionary = _dataContext.Cities.ToDictionary(key => key.Id, value => value.Name);
            var cityIds = new List<int?> {null};
            cityIds.AddRange(citiesDictionary.Keys.Select(key => (int?) key));

            var cities = cityIds.Select(x => new SelectListItem
            {
                Value = x.ToString(),
                Text = x.HasValue ? $"{citiesDictionary[x.Value]}" : "---"
            });
            ViewBag.Cities = cities.OrderBy(x => x.Text);

            var years = new List<int?> {null};
            for (var i = DateTime.Now.Year; i < DateTime.Now.Year + 20; i++)
            {
                years.Add(i);
            }

            var yearSelectListItems = years.Select(x => new SelectListItem
            {
                Value = x.ToString(),
                Text = x.HasValue ? x.ToString() : "---"
            });
            ViewBag.Years = yearSelectListItems;
        }
    }
}