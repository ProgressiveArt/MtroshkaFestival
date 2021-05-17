using System;
using System.Threading;
using System.Threading.Tasks;
using Interfaces.Application;
using MetroshkaFestival.Application.Commands.Records.Tournaments;
using MetroshkaFestival.Data;
using MetroshkaFestival.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace MetroshkaFestival.Application.Commands.Handlers.Tournaments
{
    public class AddOrUpdateTournamentCommandHandler  : ICommandHandler<AddOrUpdateTournamentCommandRecord, CommandResult>
    {
        private readonly DataContext _dataContext;

        public AddOrUpdateTournamentCommandHandler(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<CommandResult> Handle(AddOrUpdateTournamentCommandRecord commandRecord, CancellationToken ct)
        {
            try
            {
                var tournament = await _dataContext.Tournaments.FirstOrDefaultAsync(x => x.Id == commandRecord.Id, ct);

                if (tournament == null)
                {
                    var newTournament = new Tournament
                    {
                        Name = commandRecord.Name,
                        StartDate = commandRecord.StartDate.Value,
                        FinishDate = commandRecord.FinishDate.Value
                    };

                    await _dataContext.Tournaments.AddAsync(newTournament, ct);
                    await _dataContext.SaveChangesAsync(ct);
                    return AddOrUpdateTournamentCommandResult.BuildResult();
                }

                tournament.Name = commandRecord.Name;
                tournament.StartDate = commandRecord.StartDate.Value;
                tournament.FinishDate = commandRecord.FinishDate.Value;

                await _dataContext.SaveChangesAsync(ct);
                return AddOrUpdateTournamentCommandResult.BuildResult();
            }
            catch (Exception e)
            {
                Log.Error("An error occured while adding or updating tournament", e);
                return AddOrUpdateTournamentCommandResult.BuildResult(e.Message);
            }
        }
    }
}