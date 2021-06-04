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

namespace MetroshkaFestival.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("[controller]/[action]")]
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
        [AllowAnonymous]
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

                if (!(User?.Identity?.IsAuthenticated ?? true))
                {
                    tournamentsQuery = tournamentsQuery.Where(x => x.IsHiddenFromPublic == false);
                }

                if (query.Filter != null)
                {
                    tournamentsQuery = query.Filter.Apply(tournamentsQuery);
                }

                if (query.Sort != null)
                {
                    tournamentsQuery = query.Sort.Apply(tournamentsQuery);
                }
                else
                {
                    tournamentsQuery = tournamentsQuery.OrderBy(x => x.YearOfTour).ThenBy(x => x.City.Name);
                }

                var tournaments = tournamentsQuery.Select(x => new TournamentListItemModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    YearOfTour = x.YearOfTour,
                    CountRequests = x.AgeCategories.SelectMany(x => x.Teams.Where(t => t.TeamStatus == TeamStatus.AwaitConfirmation)).Count(),
                    City = x.City,
                    CanBeRemoved = !x.AgeCategories.SelectMany(c => c.Teams).Any(),
                    IsSetOpenUntilDate = x.IsSetOpenUntilDate,
                    IsTournamentOver = x.IsTournamentOver
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
        [AllowAnonymous]
        public ActionResult TournamentSummary(GetTournamentSummaryQueryModel query)
        {
            var tournament = _dataContext.Tournaments
                .Include(x => x.City)
                .Include(x => x.AgeCategories)
                .ThenInclude(x => x.Teams)
                .ThenInclude(x => x.TeamCity)
                .Include(x => x.AgeCategories)
                .ThenInclude(x => x.Teams)
                .ThenInclude(x => x.Players)
                .FirstOrDefault(x => x.Id == query.TournamentId);

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
                IsSetOpenUntilDate = tournament.IsSetOpenUntilDate,
                IsTournamentOver = tournament.IsTournamentOver,
                IsHiddenFromPublic = tournament.IsHiddenFromPublic,
                ReturnUrl = query.ReturnUrl
            };

            try
            {
                var teams = tournament.AgeCategories.SelectMany(x => x.Teams).ToArray();
                if (query.Sort != null)
                {
                    teams = query.Sort.Apply(teams).ToArray();
                }

                tournamentModel.AgeCategories = tournament.AgeCategories;
                tournamentModel.Teams = teams;
            }
            catch (Exception e)
            {
                Log.Error("An error occured while getting tournament summary", e);
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

        [HttpGet]
        public async Task<ActionResult> GetUpdateTournamentPage(UpdateTournamentCommandRecord command)
        {
            var tournament = await _dataContext.Tournaments.FirstOrDefaultAsync(x => x.Id == command.TournamentId);
            if (tournament == null)
            {
                throw new HttpResponseException(StatusCodes.Status404NotFound, TournamentExceptionCodes.NotFound);
            }

            return View("Update", command);
        }

        [HttpPost]
        public async Task<ActionResult> UpdateTournament([FromForm] UpdateTournamentCommandRecord command, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                return View("Update", command);
            }

            try
            {
                var tournament = await _dataContext.Tournaments.FirstOrDefaultAsync(x => x.Id == command.TournamentId, ct);
                if (tournament == null)
                {
                    throw new HttpResponseException(StatusCodes.Status404NotFound, TournamentExceptionCodes.NotFound);
                }

                if (!tournament.IsTournamentOver)
                {
                    if (command.IsSetOpenUntilDate.HasValue)
                    {
                        tournament.IsSetOpenUntilDate = command.IsSetOpenUntilDate.Value;
                    }

                    tournament.IsHiddenFromPublic = command.IsHiddenFromPublic;
                }

                tournament.IsTournamentOver = command.IsTournamentOver;
                if (command.IsTournamentOver)
                {
                    tournament.IsSetOpenUntilDate = DateTime.UtcNow;
                }

                await _dataContext.SaveChangesAsync(ct);
            }
            catch (HttpResponseException e)
            {
                if (e.Status is StatusCodes.Status404NotFound or StatusCodes.Status412PreconditionFailed)
                {
                    throw;
                }
            }
            catch (Exception e)
            {
                Log.Error("An error occured while adding tournament", e);
                ModelState.AddModelError("UpdateTournament", e.Message);
                return View("Update", command);
            }

            return RedirectPermanent(command.ReturnUrl);
        }

        [HttpPost]
        public async Task<ActionResult> AddTournament([FromForm] AddTournamentCommandRecord command, CancellationToken ct)
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
                var isSetOpenUntilDate = command.IsSetOpenUntilDate ?? throw new ApplicationException(TournamentExceptionCodes.IsSetOpenUntilDateIsRequired);

                var tournament = new Tournament
                {
                    Type = tournamentType,
                    City = city,
                    YearOfTour = yearOfTour,
                    Description = command.Description,
                    IsSetOpenUntilDate = isSetOpenUntilDate.ToUniversalTime(),
                    IsTournamentOver = false,
                    IsHiddenFromPublic = true
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

        [HttpPost]
        public Task BuildTestTournament([FromForm] int testTournamentYear)
        {
            var city = _dataContext.Cities.First();
            var tournament = new Tournament
            {
                Type = TournamentType.Default,
                City = city,
                YearOfTour = testTournamentYear,
                IsSetOpenUntilDate = new DateTime(testTournamentYear, DateTime.Now.Month, DateTime.Now.AddDays(7).Day, 20, 0, 0),
                IsTournamentOver = false,
                IsHiddenFromPublic = false,
                CanBeRemoved = false
            };

            _dataContext.Tournaments.Add(tournament);
            _dataContext.SaveChanges();

            city.CanBeRemoved = _cityService.CanBeRemoved(city);
            tournament.AgeCategories.Add(new AgeCategory
            {
                AgeGroup = AgeGroup.Junior,
                MinBirthDate = new DateTime(testTournamentYear - 11, 1, 1),
                MaxBirthDate = new DateTime(testTournamentYear - 10, 12, 31)
            });

            tournament.AgeCategories.Add(new AgeCategory
            {
                AgeGroup = AgeGroup.Senior,
                MinBirthDate = new DateTime(testTournamentYear - 13, 1, 1),
                MaxBirthDate = new DateTime(testTournamentYear - 12, 12, 31)
            });
             _dataContext.SaveChanges();

            var createdTournament =  _dataContext.Tournaments
                .Include(x => x.City)
                .Include(x => x.AgeCategories)
                .ThenInclude(x => x.Teams)
                .ThenInclude(x => x.Players)
                .Include(x => x.AgeCategories)
                .ThenInclude(x => x.Teams)
                .ThenInclude(x => x.TeamCity)
                .First(x => x.Id == tournament.Id);

            var ageCategory = createdTournament.AgeCategories.First();
            for (var i = 0; i < 32; i++)
            {
                var newTeam = new Team
                {
                    SchoolName = $"School{i}",
                    TeamCity = createdTournament.City,
                    AgeCategory = ageCategory,
                    TeamStatus = TeamStatus.Published,
                    TeamName = $"Team{i}"
                };

                ageCategory.Teams.Add(newTeam);
                _dataContext.SaveChanges();

                for (var j = 0; j < 15; j++)
                {
                    newTeam.Players.Add(new Player
                    {
                        FirstName = $"FirstName{i}",
                        LastName = $"LastName{i}",
                        DateOfBirth = DateTime.Now,
                        NumberInTeam = i,
                        Team = newTeam
                    });
                    _dataContext.SaveChanges();
                }
            }

            return Task.CompletedTask;
        }
    }
}