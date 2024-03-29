﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EnumsNET;
using MetroshkaFestival.Application.Commands.Records.Teams;
using MetroshkaFestival.Application.Queries.Models.Teams;
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
using Serilog;

namespace MetroshkaFestival.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("[controller]/[action]")]
    public class TeamController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly CityService _cityService;

        public TeamController(DataContext dataContext, CityService cityService)
        {
            _dataContext = dataContext;
            _cityService = cityService;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index([FromQuery] GetTeamListQueryModel query)
        {
            var teamListModel = new TeamListModel
            {
                Teams = Array.Empty<TeamListItemModel>(),
                Query = query
            };

            try
            {
                Enum.TryParse<AgeGroup>(query.AgeGroupName, ignoreCase: true, out var ageGroup);
                var ageCategory = _dataContext.AgeCategories
                    .Include(x => x.Tournament)
                    .Include(x => x.Teams)
                    .ThenInclude(x => x.TeamCity)
                    .Include(x => x.Teams)
                    .ThenInclude(x => x.Players)
                    .FirstOrDefault(x => x.TournamentId == query.TournamentId && x.AgeGroup == ageGroup);

                if (ageCategory == null)
                {
                    throw new HttpResponseException(StatusCodes.Status404NotFound, TournamentExceptionCodes.AgeCategoryNotFound);
                }

                var teams = ageCategory.Teams.AsQueryable();
                if (query.Filter != null)
                {
                    teams = query.Filter.Apply(teams);
                }

                if (query.Sort != null)
                {
                    teams = query.Sort.Apply(teams);
                }

                teamListModel.Teams = teams.Select(x => new TeamListItemModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    TeamCity = x.TeamCity,
                    SchoolName = x.SchoolName,
                    TeamStatus = x.TeamStatus,
                    CountMembers = x.Players.Count
                }).ToArray();

                teamListModel.TournamentIsOver = ageCategory.Tournament.IsTournamentOver;
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
                Log.Error("An error occured while getting team list", e);
                teamListModel.Error = e.Message;
            }

            return View(teamListModel);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult GetTeamSummaryPage(string returnUrl, int tournamentId, string ageGroupName,
            int teamId, string tournamentNameAndCategory)
        {
            Enum.TryParse<AgeGroup>(ageGroupName, ignoreCase: true, out var ageGroup);
            var team = _dataContext.Teams
                .Include(x => x.TeamCity)
                .Include(x => x.Players)
                .Include(x => x.AgeCategory)
                .ThenInclude(x => x.Tournament)
                .FirstOrDefault(x => x.Id == teamId);

            if (team == null)
            {
                throw new HttpResponseException(StatusCodes.Status404NotFound, TeamExceptionCodes.NotFound);
            }

            if (!(User?.Identity?.IsAuthenticated ?? true) && team.TeamStatus == TeamStatus.AwaitConfirmation)
            {
                throw new HttpResponseException(StatusCodes.Status404NotFound, TeamExceptionCodes.NotFound);
            }

            var model = new TeamModel
            {
                Id = team.Id,
                TeamName = team.TeamName,
                TeamCity = team.TeamCity,
                SchoolName = team.SchoolName,
                TeamStatus = team.TeamStatus,
                CountMembers = team.Players.Count,
                TournamentNameAndCategory = tournamentNameAndCategory,
                TournamentId = tournamentId,
                TournamentIsOver = team.AgeCategory.Tournament.IsTournamentOver,
                ReturnUrl = returnUrl,
                AgeGroupName = team.AgeCategory.AgeGroup.AsString(EnumFormat.Name),
                Players = team.Players.OrderBy(x => x.NumberInTeam).ToArray()
            };

            return View("Summary", model);
        }

        [HttpGet]
        public ActionResult GetAddOrUpdateTeamPage(AddOrUpdateTeamCommandRecord command)
        {
            FillSelected();
            return View("AddOrUpdate", command);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult GetAddRequestToTourPage(AddRequestToTourCommandRecord command)
        {
            FillSelected();
            return View("AddRequestToTour", command);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> AddRequestToTour([FromForm] AddRequestToTourCommandRecord command, CancellationToken ct)
        {
            if (!ModelState.IsValid || command.Error != null)
            {
                FillSelected();
                return View("AddRequestToTour", command);
            }

            try
            {
                Enum.TryParse<AgeGroup>(command.AgeGroupName, ignoreCase: true, out var ageGroup);
                var ageCategory = await _dataContext.AgeCategories
                    .Include(x => x.Teams)
                    .ThenInclude(x => x.TeamCity)
                    .Include(x => x.Tournament)
                    .FirstOrDefaultAsync(x => x.TournamentId == command.TournamentId && x.AgeGroup == ageGroup, ct);

                if (ageCategory == null)
                {
                    throw new HttpResponseException(StatusCodes.Status404NotFound, TournamentExceptionCodes.AgeCategoryNotFound);
                }

                if (ageCategory.Tournament.IsTournamentOver)
                {
                    throw new Exception(TournamentExceptionCodes.CanNotBeUpdated);
                }

                var city = await _dataContext.Cities.FirstOrDefaultAsync(x => x.Id == command.TeamCityId.Value, ct);
                if (city == null)
                {
                    throw new ApplicationException(CityExceptionCodes.UnknownCity);
                }

                var isAlreadyExists = ageCategory.Teams.Any(x => x.TeamName.ToUpper() == command.TeamName.ToUpper() &&
                                                                 x.TeamCity == city &&
                                                                 x.SchoolName.ToUpper() ==
                                                                 command.SchoolName.ToUpper());

                if (isAlreadyExists)
                {
                    throw new ApplicationException(TeamExceptionCodes.AlreadyExist);
                }

                var team = new Team
                {
                    TeamName = command.TeamName,
                    TeamCity = city,
                    SchoolName = command.SchoolName
                };

                var players = new List<Player>();
                foreach (var player in command.Players)
                {
                    var playerIsAlreadyExists =  players
                        .Any(x => x.FirstName.ToUpper() == player.FirstName.ToUpper()
                             && x.LastName.ToUpper() == player.LastName.ToUpper()
                             && x.DateOfBirth == player.DateOfBirth);

                    var numberIsAlreadyInUse = players.Any(x => x.NumberInTeam == player.NumberInTeam);
                    if (playerIsAlreadyExists)
                    {
                        throw new ApplicationException(PlayerExceptionCodes.AlreadyExist);
                    }

                    if (numberIsAlreadyInUse)
                    {
                        throw new ApplicationException(PlayerExceptionCodes.NumberIsBusy);
                    }

                    var dateOfBirth = player.DateOfBirth ?? throw new Exception(PlayerExceptionCodes.DateOfBirthIsRequired);
                    var numberInTeam = player.NumberInTeam ?? throw new Exception(PlayerExceptionCodes.NumberInTeamIsRequired);
                    var newPlayer = new Player
                    {
                        FirstName = player.FirstName,
                        LastName = player.LastName,
                        DateOfBirth = dateOfBirth,
                        NumberInTeam = numberInTeam
                    };

                    players.Add(newPlayer);
                }

                ageCategory.Teams.Add(team);
                await _dataContext.SaveChangesAsync(ct);

                city.CanBeRemoved = _cityService.CanBeRemoved(city);
                await _dataContext.SaveChangesAsync(ct);

                foreach (var player in players)
                {
                    team.Players.Add(player);
                    await _dataContext.SaveChangesAsync(ct);
                }
            }
            catch (Exception e)
            {
                Log.Error("An error occured while adding request to tournament", e);
                command = command with{Error = e.Message};
                FillSelected();
                return View("AddRequestToTour", command);
            }

            return Redirect(command.ReturnUrl);
        }

        [HttpPost]
        public async Task<ActionResult> AddOrUpdateTeam([FromForm] AddOrUpdateTeamCommandRecord command, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                FillSelected();
                return View("AddOrUpdate", command);
            }

            try
            {
                Enum.TryParse<AgeGroup>(command.AgeGroupName, ignoreCase: true, out var ageGroup);
                var ageCategory = await _dataContext.AgeCategories
                    .Include(x => x.Teams)
                    .ThenInclude(x => x.TeamCity)
                    .Include(x => x.Tournament)
                    .FirstOrDefaultAsync(x => x.TournamentId == command.TournamentId && x.AgeGroup == ageGroup, ct);

                if (ageCategory == null)
                {
                    throw new HttpResponseException(StatusCodes.Status404NotFound, TournamentExceptionCodes.AgeCategoryNotFound);
                }

                if (ageCategory.Tournament.IsTournamentOver)
                {
                    throw new Exception(TournamentExceptionCodes.CanNotBeUpdated);
                }

                var city = await _dataContext.Cities.FirstOrDefaultAsync(x => x.Id == command.TeamCityId.Value, ct);
                if (city == null)
                {
                    throw new ApplicationException(CityExceptionCodes.UnknownCity);
                }

                var isAlreadyExists = ageCategory.Teams.Any(x => x.TeamName.ToUpper() == command.TeamName.ToUpper() &&
                                                                 x.TeamCity == city &&
                                                                 x.SchoolName.ToUpper() ==
                                                                 command.SchoolName.ToUpper());

                if (isAlreadyExists)
                {
                    throw new ApplicationException(TeamExceptionCodes.AlreadyExist);
                }

                if (command.TeamId == null)
                {
                    var team = new Team
                    {
                        TeamName = command.TeamName,
                        TeamCity = city,
                        SchoolName = command.SchoolName
                    };

                    ageCategory.Teams.Add(team);
                    await _dataContext.SaveChangesAsync(ct);

                    city.CanBeRemoved = _cityService.CanBeRemoved(city);
                    await _dataContext.SaveChangesAsync(ct);
                }
                else
                {
                    var team = await _dataContext.Teams.FirstOrDefaultAsync(x => x.Id == command.TeamId, ct);
                    if (team == null)
                    {
                        throw new HttpResponseException(StatusCodes.Status404NotFound, TeamExceptionCodes.NotFound);
                    }

                    if (team.TeamStatus == TeamStatus.Published)
                    {
                        throw new HttpResponseException(StatusCodes.Status412PreconditionFailed, TeamExceptionCodes.CanNotBeUpdated);
                    }

                    team.TeamName = command.TeamName;
                    team.TeamCity = city;
                    team.SchoolName = command.SchoolName;

                    var lastCity = team.TeamCity;
                    await _dataContext.SaveChangesAsync(ct);

                    lastCity.CanBeRemoved = _cityService.CanBeRemoved(lastCity);
                    city.CanBeRemoved = _cityService.CanBeRemoved(city);
                    await _dataContext.SaveChangesAsync(ct);
                }
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
                Log.Error("An error occured while adding team", e);
                ModelState.AddModelError("AddTeam", e.Message);
                FillSelected();
                return View("AddOrUpdate", command);
            }

            return RedirectPermanent(command.ReturnUrl);
        }

        [HttpGet]
        public async Task<ActionResult> UpdateTeamStatus(int teamId, string returnUrl)
        {
            var team = await _dataContext.Teams
                .Include(x => x.Players)
                .Include(x => x.AgeCategory)
                .ThenInclude(x => x.Tournament)
                .FirstOrDefaultAsync(x => x.Id == teamId);

            if (team.AgeCategory.Tournament.IsTournamentOver)
            {
                throw new HttpResponseException(StatusCodes.Status412PreconditionFailed, TournamentExceptionCodes.CanNotBeUpdated);
            }

            switch (team.Players.Count)
            {
                case < 12 when team.TeamStatus != TeamStatus.Published:
                    throw new HttpResponseException(StatusCodes.Status412PreconditionFailed, TeamExceptionCodes.MinPlayerCount);
                case > 15 when team.TeamStatus != TeamStatus.Published:
                    throw new HttpResponseException(StatusCodes.Status412PreconditionFailed, TeamExceptionCodes.MaxPlayerCount);
            }

            team.TeamStatus = TeamStatus.Published;
            await _dataContext.SaveChangesAsync();
            return Redirect(returnUrl);
        }

        [HttpGet]
        public async Task<ActionResult> DeleteTeam(int teamId, string returnUrl)
        {
            var team = await _dataContext.Teams
                .Include(x => x.Players)
                .Include(x => x.TeamCity)
                .Include(x => x.AgeCategory)
                .ThenInclude(x => x.Tournament)
                .FirstOrDefaultAsync(x => x.Id == teamId);

            if (team.AgeCategory.Tournament.IsTournamentOver)
            {
                throw new HttpResponseException(StatusCodes.Status412PreconditionFailed, TournamentExceptionCodes.CanNotBeUpdated);
            }

            if (team == null)
            {
                throw new HttpResponseException(StatusCodes.Status404NotFound, TeamExceptionCodes.NotFound);
            }

            if (team.TeamStatus == TeamStatus.Published)
            {
                throw new HttpResponseException(StatusCodes.Status412PreconditionFailed, TeamExceptionCodes.CanNotBeRemoved);
            }

            var city = team.TeamCity;
            _dataContext.Teams.Remove(team);
            await _dataContext.SaveChangesAsync();

            city.CanBeRemoved = _cityService.CanBeRemoved(city);
            await _dataContext.SaveChangesAsync();

            return RedirectPermanent(returnUrl);
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
        }
    }
}