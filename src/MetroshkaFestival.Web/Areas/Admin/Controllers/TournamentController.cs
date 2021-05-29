using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MetroshkaFestival.Application.Commands;
using MetroshkaFestival.Application.Commands.Records.Tournaments;
using MetroshkaFestival.Application.Queries.Models.Tournaments;
using MetroshkaFestival.Application.Services;
using MetroshkaFestival.Core.Exceptions.Common;
using MetroshkaFestival.Core.Exceptions.ExceptionCodes;
using MetroshkaFestival.Data;
using MetroshkaFestival.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Serilog;
using PagedList.Core;

namespace MetroshkaFestival.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    [Route("[area]/[controller]/[action]")]
    public class TournamentController : Controller
    {
        private readonly DataContext _dataContext;

        public TournamentController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpGet]
        public IActionResult Index([FromQuery] GetTournamentListQueryModel query, CancellationToken ct)
        {
            GetTournamentListQueryResult result;

            try
            {
                IQueryable<Tournament> tournamentsQuery = _dataContext.Tournaments
                    .Include(x => x.City);

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
                    CanBeRemoved = x.CanBeRemoved,
                }).ToArray();

                query.Page.NumPage = Math.Min(query.Page.NumPage, (int) Math.Ceiling((double)tournaments.Length / query.Page.RecordsCountPerPage));
                var tournamentListModel = new TournamentListModel
                {
                    Query = query,
                    Tournaments = new PagedList<TournamentListItemModel>(tournaments.AsQueryable(),
                        query.Page.NumPage == 0 ? 1 : query.Page.NumPage,
                        query.Page.RecordsCountPerPage)
                };

                result = GetTournamentListQueryResult.BuildResult(tournamentListModel);
            }
            catch (Exception e)
            {
                Log.Error("An error occured while getting tournament list", e);
                result = GetTournamentListQueryResult.BuildResult(error: e.Message);
            }

            if (result.Error != null)
            {
                throw new Exception(result.Error);
            }

            FillSelected();
            return View(result.Result);
        }

        [HttpGet]
        public async Task<ActionResult> TournamentSummary(string returnUrl, int? tournamentId = null, bool isReadOnly = true)
        {
            if (tournamentId == null)
            {
                var tournamentEditCommand = new AddOrUpdateTournamentCommandRecord(returnUrl);

                FillSelected();
                return View("TournamentEdit", tournamentEditCommand);
            }

            var tournament = await _dataContext.Tournaments
                .Include(x => x.City)
                .Include(x => x.Groups)
                .ThenInclude(x => x.AgeCategory)
                .FirstOrDefaultAsync(x => x.Id == tournamentId);

            if (tournament == null)
            {
                throw new HttpResponseException(404, "Турнир не найден");
            }

            if (!isReadOnly)
            {
                var tournamentEditCommand = new AddOrUpdateTournamentCommandRecord(returnUrl, tournament.Id, tournament.Name, tournament.YearOfTour, tournament.City.Id, tournament.Description);
                FillSelected();
                return View("TournamentEdit", tournamentEditCommand);
            }

            var tournamentModel = new TournamentModel
            {
                Id = tournament.Id,
                Name = tournament.Name,
                YearOfTour = tournament.YearOfTour,
                City = tournament.City,
                Description = tournament.Description,
                Groups = tournament.Groups
            };
            return View("TournamentSummary", tournamentModel);
        }

        [HttpPost]
        public async Task<ActionResult> AddOrUpdateTournament([FromForm] AddOrUpdateTournamentCommandRecord commandRecord, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                FillSelected();
                return View("TournamentEdit", commandRecord);
            }

            CommandResult result;
            try
            {
                var tournament = await _dataContext.Tournaments.FirstOrDefaultAsync(x => x.Id == commandRecord.Id, ct);
                var city = await _dataContext.Cities.FirstOrDefaultAsync(x => x.Id == commandRecord.CityId.Value, ct);
                if (city == null)
                {
                    throw new Exception(TournamentExceptionCodes.UnknownCity);
                }

                var yearOfTour = commandRecord.YearOfTour ?? throw new Exception(TournamentExceptionCodes.YearOfTourIsRequired);

                if (tournament == null)
                {
                    tournament = new Tournament
                    {
                        City = city,
                        YearOfTour = yearOfTour,
                        Description = commandRecord.Description
                    };

                    await _dataContext.Tournaments.AddAsync(tournament, ct);
                    await _dataContext.SaveChangesAsync(ct);
                    result = AddOrUpdateTournamentCommandResult.BuildResult();
                }
                else
                {
                    tournament.City = city;
                    tournament.YearOfTour = yearOfTour;
                    tournament.Description = commandRecord.Description;

                    await _dataContext.SaveChangesAsync(ct);
                    result = AddOrUpdateTournamentCommandResult.BuildResult();
                }
            }
            catch (Exception e)
            {
                Log.Error("An error occured while adding or updating tournament", e);
                result = AddOrUpdateTournamentCommandResult.BuildResult(e.Message);
            }

            if (result.Error != null)
            {
                ModelState.AddModelError("AddOrUpdateTournament", result.Error);
                FillSelected();
                return View("TournamentEdit", commandRecord);
            }

            if (!ModelState.IsValid)
            {
                ErrorService.Error(ViewData, UnprocessableEntity);
            }

            return RedirectPermanent(commandRecord.ReturnUrl);
        }

        [HttpGet]
        public async Task<ActionResult> DeleteTournament(int tournamentId)
        {
            var tournament = await _dataContext.Tournaments.FirstOrDefaultAsync(x => x.Id == tournamentId);

            if (tournament == null)
            {
                throw new HttpResponseException(StatusCodes.Status404NotFound);
            }

            _dataContext.Tournaments.Remove(tournament);
            await _dataContext.SaveChangesAsync();

            return RedirectToAction("Index", "Tournament");
        }

        private void FillSelected()
        {
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