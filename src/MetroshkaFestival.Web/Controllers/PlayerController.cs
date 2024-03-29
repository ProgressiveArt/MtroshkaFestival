﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MetroshkaFestival.Application.Commands.Records.Players;
using MetroshkaFestival.Application.Queries.Models.Players;
using MetroshkaFestival.Core.Exceptions.Common;
using MetroshkaFestival.Core.Exceptions.ExceptionCodes;
using MetroshkaFestival.Core.Models.Common;
using MetroshkaFestival.Data;
using MetroshkaFestival.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace MetroshkaFestival.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("[controller]/[action]")]
    public class PlayerController : Controller
    {
        private readonly DataContext _dataContext;

        public PlayerController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult GetPlayerSummaryPage(GetPlayerSummaryQueryModel query)
        {
            var team = _dataContext.Teams
                .Include(x => x.Players)
                .Include(x => x.AgeCategory)
                .ThenInclude(x => x.Tournament)
                .FirstOrDefault(x => x.Id == query.TeamId);

            if (team == null)
            {
                throw new HttpResponseException(StatusCodes.Status404NotFound, TeamExceptionCodes.NotFound);
            }

            var player = team.Players.FirstOrDefault(x => x.Id == query.PlayerId);
            if (player == null)
            {
                throw new HttpResponseException(StatusCodes.Status404NotFound, PlayerExceptionCodes.NotFound);
            }

            var playerModel = new PlayerModel
            {
                ReturnUrl = query.ReturnUrl,
                PlayerId = player.Id,
                FirstName = player.FirstName,
                LastName = player.LastName,
                DateOfBirth = player.DateOfBirth,
                NumberInTeam = player.NumberInTeam,
                TeamId = player.TeamId,
                TournamentIsOver = team.AgeCategory.Tournament.IsTournamentOver
            };

            return View("Summary", playerModel);
        }

        [HttpGet]
        public ActionResult GetAddOrUpdatePlayerPage(string returnUrl, int teamId,
            int? playerId = null)
        {
            var command = new AddOrUpdatePlayerCommandRecord(returnUrl, teamId, playerId);
            if (playerId != null)
            {
                var team = _dataContext.Teams
                    .Include(x => x.Players)
                    .Include(x => x.AgeCategory)
                    .ThenInclude(x => x.Tournament)
                    .FirstOrDefault(x => x.Id == teamId);

                if (team == null)
                {
                    throw new HttpResponseException(StatusCodes.Status404NotFound, TeamExceptionCodes.NotFound);
                }

                if (team.AgeCategory.Tournament.IsTournamentOver)
                {
                    throw new HttpResponseException(StatusCodes.Status412PreconditionFailed, TournamentExceptionCodes.CanNotBeUpdated);
                }

                var player = team.Players.FirstOrDefault(x => x.Id == playerId);
                if (player == null)
                {
                    throw new HttpResponseException(StatusCodes.Status404NotFound, PlayerExceptionCodes.NotFound);
                }

                command = command with
                {
                    FirstName = player.FirstName,
                    LastName = player.LastName,
                    DateOfBirth = player.DateOfBirth,
                    NumberInTeam = player.NumberInTeam
                };
            }

            return View("AddOrUpdate", command);
        }

        [HttpPost]
        public async Task<ActionResult> AddOrUpdatePlayer(AddOrUpdatePlayerCommandRecord command, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                return View("AddOrUpdate", command);
            }

            try
            {
                var team = await _dataContext.Teams
                    .Include(x => x.Players)
                    .Include(x => x.AgeCategory)
                    .ThenInclude(x => x.Tournament)
                    .FirstOrDefaultAsync(x => x.Id == command.TeamId, ct);

                if (team == null)
                {
                    throw new HttpResponseException(StatusCodes.Status404NotFound, TeamExceptionCodes.NotFound);
                }

                if (team.AgeCategory.Tournament.IsTournamentOver)
                {
                    throw new HttpResponseException(StatusCodes.Status412PreconditionFailed, TournamentExceptionCodes.CanNotBeUpdated);
                }

                var isAlreadyExists = await _dataContext.Players
                    .AnyAsync(x => x.FirstName.ToUpper() == command.FirstName.ToUpper()
                              && x.LastName.ToUpper() == command.LastName.ToUpper()
                              && x.DateOfBirth == command.DateOfBirth, ct);

                var numberIsAlreadyInUse = team.Players
                    .Where(x => x.Id != command.PlayerId)
                    .Any(x => x.NumberInTeam == command.NumberInTeam);

                if (isAlreadyExists)
                {
                    throw new ApplicationException(PlayerExceptionCodes.AlreadyExist);
                }

                if (command.PlayerId == null)
                {
                    if (numberIsAlreadyInUse )
                    {
                        throw new ApplicationException(PlayerExceptionCodes.NumberIsBusy);
                    }

                    var player = new Player
                    {
                        FirstName = command.FirstName,
                        LastName = command.LastName,
                        DateOfBirth = command.DateOfBirth.Value,
                        NumberInTeam = command.NumberInTeam.Value,
                    };

                    team.Players.Add(player);
                    await _dataContext.SaveChangesAsync(ct);
                }
                else
                {
                    var player = team.Players.FirstOrDefault(x => x.Id == command.PlayerId);
                    if (player == null)
                    {
                        throw new HttpResponseException(StatusCodes.Status404NotFound, PlayerExceptionCodes.NotFound);
                    }

                    if (numberIsAlreadyInUse && command.NumberInTeam != player.NumberInTeam)
                    {
                        throw new ApplicationException(PlayerExceptionCodes.NumberIsBusy);
                    }

                    player.FirstName = command.FirstName;
                    player.LastName = command.LastName;
                    player.DateOfBirth = command.DateOfBirth.Value;
                    player.NumberInTeam = command.NumberInTeam.Value;
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
                Log.Error("An error occured while adding player", e);
                ModelState.AddModelError("AddPlayer", e.Message);
                return View("AddOrUpdate", command);
            }

            return RedirectPermanent(command.ReturnUrl);
        }

        [HttpGet]
        public async Task<ActionResult> DeletePlayer(int teamId, int playerId, string returnUrl)
        {
            var team = await _dataContext.Teams
                .Include(x => x.Players)
                .Include(x => x.AgeCategory)
                .ThenInclude(x => x.Tournament)
                .FirstOrDefaultAsync(x => x.Id == teamId);

            if (team == null)
            {
                throw new HttpResponseException(StatusCodes.Status404NotFound, TeamExceptionCodes.NotFound);
            }

            if (team.AgeCategory.Tournament.IsTournamentOver)
            {
                throw new HttpResponseException(StatusCodes.Status412PreconditionFailed, TournamentExceptionCodes.CanNotBeUpdated);
            }

            if (team.TeamStatus == TeamStatus.Published)
            {
                throw new HttpResponseException(StatusCodes.Status412PreconditionFailed, TeamExceptionCodes.CanNotBeUpdated);
            }

            var player = team.Players.FirstOrDefault(x => x.Id == playerId);
            if (player == null)
            {
                throw new HttpResponseException(StatusCodes.Status404NotFound, PlayerExceptionCodes.NotFound);
            }

            team.Players.Remove(player);
            await _dataContext.SaveChangesAsync();

            return RedirectPermanent(returnUrl);
        }
    }
}