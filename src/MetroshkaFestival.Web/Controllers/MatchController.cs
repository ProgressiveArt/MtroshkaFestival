using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnumsNET;
using MetroshkaFestival.Application.Commands.Records;
using MetroshkaFestival.Application.Commands.Records.Matches;
using MetroshkaFestival.Application.Queries.Models.Matches;
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
    public class MatchController : Controller
    {
        private readonly DataContext _dataContext;

        public MatchController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index([FromQuery] GetMatchListQueryModel query)
        {
            var result = new MatchListModel
            {
                Query = query,
                Matches = new List<MatchListItemModel>(),
                TournamentIsOver = false
            };

            try
            {
                if (query.IsAddMatches)
                {
                    if (!query.MatchStartDateTime.HasValue)
                    {
                        throw new ApplicationException(MatchExceptionCodes.FirstMatchStartDateTimeIsRequired);
                    }

                    AddMatchStage(new AddMatchStageCommandRecord(query.ReturnUrl, query.TournamentId,
                        query.AgeGroupName,
                        query.TournamentNameAndCategory, query.MatchStartDateTime.Value));
                }

                Enum.TryParse<AgeGroup>(query.AgeGroupName, ignoreCase: true, out var ageGroup);
                var ageCategory = _dataContext.AgeCategories
                    .Include(x => x.Tournament)
                    .Include(x => x.Matches)
                    .ThenInclude(x => x.FirstTeam)
                    .Include(x => x.Matches)
                    .ThenInclude(x => x.SecondTeam)
                    .Include(x => x.Teams)
                    .FirstOrDefault(x => x.TournamentId == query.TournamentId && x.AgeGroup == ageGroup);

                if (ageCategory == null)
                {
                    throw new HttpResponseException(StatusCodes.Status412PreconditionFailed,
                        TournamentExceptionCodes.AgeCategoryNotFound);
                }

                var matches = ageCategory.Matches.Select(x => new MatchListItemModel
                {
                    MatchId = x.Id,
                    MatchDateTime = x.MatchDateTime,
                    FieldNumber = x.FieldNumber,
                    FirstTeamName = x.FirstTeam.TeamName,
                    FirstTeamId = x.FirstTeam.Id,
                    SecondTeamName = x.SecondTeam.TeamName,
                    SecondTeamId = x.SecondTeam.Id,
                    StageNumber = x.StageNumber,
                    FirstTeamGoalsScore = x.FirstTeamGoalsScore,
                    FirstTeamPenaltyGoalsScore = x.FirstTeamPenaltyGoalsScore,
                    SecondTeamGoalsScore = x.SecondTeamGoalsScore,
                    SecondTeamPenaltyGoalsScore = x.SecondTeamPenaltyGoalsScore,
                    MatchFinalResult = x.MatchFinalResult
                });

                query.IsAddMatches = false;
                result = new MatchListModel
                {
                    Query = query,
                    Matches = matches.OrderBy(x => x.StageNumber).ThenBy(x => x.MatchDateTime).ThenBy(x => x.FieldNumber).ToArray(),
                    TournamentIsOver = ageCategory.Tournament.IsTournamentOver,
                    TeamsCount = ageCategory.Teams.Count,
                    PublishedTeamsCount = ageCategory.Teams.Count(x => x.TeamStatus == TeamStatus.Published)
                };
            }
            catch (HttpResponseException)
            {
                throw;
            }
            catch (Exception e)
            {
                Log.Error("An error occured while adding matches", e);
                result.Error = e.Message;
                return View(result);
            }

            return View(result);
        }

        [HttpGet]
        public IActionResult GetUpdateMatchPage(int matchId, string returnUrl)
        {
            var match = _dataContext.Matches
                .Include(x => x.FirstTeam)
                .Include(x => x.SecondTeam)
                .FirstOrDefault(x => x.Id == matchId);

            if (match == null)
            {
                throw new HttpResponseException(StatusCodes.Status404NotFound, MatchExceptionCodes.NotFound);
            }

            var command = new UpdateMatchCommandRecord(returnUrl, matchId,
                match.MatchDateTime, match.FirstTeam.TeamName, match.SecondTeam.TeamName,
                match.FirstTeamGoalsScore, match.FirstTeamPenaltyGoalsScore,
                match.SecondTeamGoalsScore, match.SecondTeamPenaltyGoalsScore,
                match.MatchFinalResult);

            FillSelected();
            return View("Update", command);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateMatch(UpdateMatchCommandRecord command)
        {
            var match = await _dataContext.Matches.FirstOrDefaultAsync(x => x.Id == command.MatchId);
            if (match == null)
            {
                throw new HttpResponseException(StatusCodes.Status412PreconditionFailed, MatchExceptionCodes.NotFound);
            }

            try
            {
                match.FirstTeamGoalsScore = command.FirstTeamGoalsScore;
                match.FirstTeamPenaltyGoalsScore = command.FirstTeamPenaltyGoalsScore;
                match.SecondTeamGoalsScore = command.SecondTeamGoalsScore;
                match.SecondTeamPenaltyGoalsScore = command.SecondTeamPenaltyGoalsScore;

                var firstTeamScore = command.FirstTeamGoalsScore + command.FirstTeamPenaltyGoalsScore;
                var secondTeamScore = command.SecondTeamGoalsScore + command.SecondTeamPenaltyGoalsScore;
                if (firstTeamScore == 0 && secondTeamScore == 0)
                {
                    match.MatchFinalResult = command.MatchFinalResult;
                }
                else
                {
                    match.MatchFinalResult = firstTeamScore > secondTeamScore
                        ? MatchFinalResult.WinFirst
                        : MatchFinalResult.WinSecond;
                }

                await _dataContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Log.Error("An error occured while update match", e);
                ModelState.AddModelError("UpdateMatch", e.Message);
                FillSelected();
                return View("Update", command);
            }

            var returnUrl = command.ReturnUrl.Contains("&Query.IsAddMatches") ? command.ReturnUrl.Substring(0, command.ReturnUrl.LastIndexOf("&Query.IsAddMatches")) : command.ReturnUrl;
            return Redirect(returnUrl);
        }

        private void AddMatchStage([FromForm] AddMatchStageCommandRecord command)
        {
            Enum.TryParse<AgeGroup>(command.AgeGroupName, ignoreCase: true, out var ageGroup);
            var ageCategory = _dataContext.AgeCategories
                .Include(x => x.Matches)
                .Include(x => x.Teams)
                .Include(x => x.Tournament)
                .FirstOrDefault(x => x.TournamentId == command.TournamentId && x.AgeGroup == ageGroup);

            if (ageCategory == null)
            {
                throw new HttpResponseException(StatusCodes.Status412PreconditionFailed, TournamentExceptionCodes.AgeCategoryNotFound);
            }

            if (ageCategory.Teams.Where(x => x.TeamStatus == TeamStatus.Published).Count(x => x.TeamStatus == TeamStatus.Published) != 32)
            {
                throw new ApplicationException(MatchExceptionCodes.MustBe32Teams);
            }

            if (ageCategory.Matches.Any())
            {
                if (ageCategory.Matches.Any(x => x.MatchFinalResult == MatchFinalResult.Unknown))
                {
                    throw new Exception(MatchExceptionCodes.PreviousStageNotComplited);
                }

                var currentStageNumber = ageCategory.Matches.OrderBy(x => x.StageNumber).Last().StageNumber;
                if (currentStageNumber == StageNumber.Final)
                {
                    return;
                }

                var lastMatchDate = ageCategory.Matches.OrderBy(x => x.StageNumber).ThenBy(x => x.MatchDateTime).Last().MatchDateTime;

                var teams = ageCategory.Matches.Where(x => x.StageNumber == currentStageNumber).Select(x =>
                {
                    if (x.MatchFinalResult == MatchFinalResult.WinFirst)
                    {
                        return x.FirstTeam;
                    }

                    if (x.MatchFinalResult == MatchFinalResult.WinSecond)
                    {
                        return x.SecondTeam;
                    }

                    throw new Exception(MatchExceptionCodes.СurrentStageNotComplited);
                }).ToArray();

                var nextStageNumber =  (StageNumber) ((int) currentStageNumber + 1);
                var nextStartDate = new DateTime(lastMatchDate.Year,
                    lastMatchDate.Month,
                    lastMatchDate.AddDays(1).Day,
                    12, 30, 0);

                GenerateMatches(ageCategory, teams, nextStageNumber, nextStartDate);
            }
            else
            {
                var teams = ageCategory.Teams.Where(x => x.TeamStatus == TeamStatus.Published).ToArray();
                GenerateMatches(ageCategory, teams, StageNumber.StageOne, command.MatchStartDateTime);
            }
        }

        private void GenerateMatches(AgeCategory ageCategory, Team[] teams, StageNumber stageNumber, DateTime startDate)
        {
            if (ageCategory.Teams.Count(x => x.TeamStatus == TeamStatus.Published) % 2 == 1)
            {
                throw new Exception(MatchExceptionCodes.MustBeEven);
            }

            stageNumber = teams.Length switch
            {
                8 => StageNumber.PlayOffOneEight,
                4 => StageNumber.Semifinal,
                2 => StageNumber.Final,
                _ => stageNumber
            };

            var random = new Random();
            var teamsList = teams.Where(x => x.TeamStatus == TeamStatus.Published).ToList();

            var countPlaysOnField = 1;
            while (teamsList.Count != 0)
            {
                foreach (var fieldNumber in Enum.GetValues(typeof(FieldNumber)))
                {
                    if (teamsList.Count == 0)
                    {
                        break;
                    }

                    var firstRandomTeamIndex = random.Next(teamsList.Count);
                    var firstTeam = teamsList[firstRandomTeamIndex];
                    teamsList.Remove(firstTeam);

                    var secondRandomTeamIndex = random.Next(teamsList.Count);
                    var secondTeam = teamsList[secondRandomTeamIndex];
                    teamsList.Remove(secondTeam);

                    var match = new Match
                    {
                        MatchDateTime = startDate,
                        FieldNumber = (FieldNumber) fieldNumber,
                        FirstTeam = firstTeam,
                        SecondTeam = secondTeam,
                        StageNumber = stageNumber,
                        AgeCategory = ageCategory,
                        MatchFinalResult = MatchFinalResult.WinFirst
                    };

                    ageCategory.Matches.Add(match);
                    _dataContext.SaveChanges();
                }

                if (countPlaysOnField == 4)
                {
                    startDate = startDate.AddDays(1);
                    countPlaysOnField = 0;
                }
                else
                {
                    startDate = startDate.AddMinutes(40);
                    countPlaysOnField++;
                }
            }

            ageCategory.Matches.OrderBy(x => x.StageNumber).ThenBy(x => x.MatchDateTime).Last().MatchFinalResult = MatchFinalResult.Unknown;
            _dataContext.SaveChanges();
        }

        private void FillSelected()
        {
            var matchResults = Enum.GetValues(typeof(MatchFinalResult))
                .Cast<int>()
                .ToArray();

            var tournamentTypes = matchResults.Select(x => new SelectListItem
            {
                Value = x.ToString(),
                Text = $"{((MatchFinalResult) x).AsString(EnumFormat.Description)}"
            });
            ViewBag.MatchFinalResults = tournamentTypes;
        }
    }
}