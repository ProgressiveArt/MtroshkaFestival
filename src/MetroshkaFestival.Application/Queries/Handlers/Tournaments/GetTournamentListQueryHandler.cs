using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Interfaces.Application;
using MetroshkaFestival.Application.Factories;
using MetroshkaFestival.Application.Queries.Records.Tournaments;
using MetroshkaFestival.Data;
using MetroshkaFestival.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace MetroshkaFestival.Application.Queries.Handlers.Tournaments
{
    public class GetTournamentListQueryHandler : IQueryHandler<GetTournamentListQueryRecord, GetTournamentListQueryResult>
    {
        private readonly DataContext _dataContext;
        private readonly TournamentModelFactory _tournamentModelFactory;

        public GetTournamentListQueryHandler(DataContext dataContext, TournamentModelFactory tournamentModelFactory)
        {
            _dataContext = dataContext;
            _tournamentModelFactory = tournamentModelFactory;
        }

        public async Task<GetTournamentListQueryResult> Handle(GetTournamentListQueryRecord query, CancellationToken ct)
        {
            GetTournamentListQueryResult result;

            try
            {
                var total = await _dataContext.Tournaments.CountAsync(ct);
                IQueryable<Tournament> tournamentsQuery = _dataContext.Tournaments.OrderBy(x => x.Id);
                if (query.StartDateAfter.HasValue)
                {
                    tournamentsQuery = tournamentsQuery.Where(x => x.FinishDate <= query.FinishDateBefore.Value);
                }

                if (query.StartDateAfter.HasValue)
                {
                    tournamentsQuery = tournamentsQuery.Where(x => x.StartDate >= query.StartDateAfter.Value);
                }

                if (query.Skip.HasValue)
                {
                    tournamentsQuery = tournamentsQuery.Skip(query.Skip.Value);
                }

                if (query.Count.HasValue)
                {
                    tournamentsQuery = tournamentsQuery.Take(query.Count.Value);
                }

                var tournaments = await tournamentsQuery.ToListAsync(ct);

                var tournamentListModel = new TournamentListModel
                {
                    Total = total,
                    Tournaments = tournaments.Select(_tournamentModelFactory.CreateTournamentModel).ToArray()
                };

                result = GetTournamentListQueryResult.BuildResult(tournamentListModel);
            }
            catch (Exception e)
            {
                Log.Error("An error occured while getting tournament list", e);
                result = GetTournamentListQueryResult.BuildResult(error: e.Message);
            }

            return result;
        }
    }
}