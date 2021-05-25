using Interfaces.Application;
using MetroshkaFestival.Application.Queries.Records.Tournaments;
using MetroshkaFestival.Data.Entities;

namespace MetroshkaFestival.Application.Factories
{
    public class TournamentModelFactory : IFactory
    {
        public TournamentModel CreateTournamentModel(Tournament tournament)
        {
            return new()
            {
                Id = tournament.Id,
                Name = tournament.Name,
                FinishDate = tournament.FinishDate,
                StartDate = tournament.StartDate
            };
        }
    }
}