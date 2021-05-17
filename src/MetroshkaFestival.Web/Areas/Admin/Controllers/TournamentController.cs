using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MetroshkaFestival.Application.Commands.Handlers.Tournaments;
using MetroshkaFestival.Application.Commands.Records.Tournaments;
using MetroshkaFestival.Application.Queries.Handlers.Tournaments;
using MetroshkaFestival.Application.Queries.Records.Tournaments;
using MetroshkaFestival.Application.Services;
using MetroshkaFestival.Core.Exceptions.Common;
using MetroshkaFestival.Data;
using MetroshkaFestival.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        public async Task<IActionResult> Index([FromServices] GetTournamentListQueryHandler handler,
            [FromQuery] GetTournamentListQueryRecord query, CancellationToken ct)
        {
            var result = await handler.Handle(query, ct);
            if (result.Error != null)
            {
                result.Result = new TournamentListModel
                {
                    Total = 0,
                    Tournaments = Array.Empty<TournamentModel>()
                };
            }

            return View(result.Result);
        }

        [HttpGet]
        public async Task<ActionResult> TournamentSummary(int? tournamentId = null)
        {
            if (tournamentId == null)
            {
                var tournamentEditCommand = new AddOrUpdateTournamentCommandRecord();
                return View("Tournament", tournamentEditCommand);
            }

            var tournament = await _dataContext.Tournaments.FirstOrDefaultAsync(x => x.Id == tournamentId);
            var tournamentToEdit = new AddOrUpdateTournamentCommandRecord(tournament.Id, tournament.Name, tournament.StartDate, tournament.FinishDate);
            return View("Tournament", tournamentToEdit);
        }

        public async Task<ActionResult> AddOrUpdateTournament([FromServices] AddOrUpdateTournamentCommandHandler handler,
            [FromForm] AddOrUpdateTournamentCommandRecord commandRecord, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                return View("Tournament", commandRecord);
            }

            var result = await handler.Handle(commandRecord, ct);
            if (result.Error != null)
            {
                ModelState.AddModelError("AddOrUpdateTournament", result.Error);
                return View("Tournament", commandRecord);
            }

            if (!ModelState.IsValid)
            {
                ErrorService.Error(ViewData, UnprocessableEntity);
            }

            return RedirectToAction("Index", "Tournament");
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
    }
}