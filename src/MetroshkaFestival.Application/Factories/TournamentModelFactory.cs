using Interfaces.Application;
using MetroshkaFestival.Application.Queries.Models.Tournaments;
using MetroshkaFestival.Data.Entities;

namespace MetroshkaFestival.Application.Factories
{
    public class TournamentModelFactory : IFactory
    {
        public TournamentListItemModel CreateTournamentListItemModel(Tournament tournament)
        {
            return new()
            {
                Id = tournament.Id,
                Name = tournament.Name
            };
        }
        // public TournamentModel CreateTournamentModel(Tournament tournament)
        // {
        //     return new()
        //     {
        //         Id = tournament.Id,
        //         Name = tournament.Name,
        //         City = tournament.City,
        //         Description = tournament.Description,
        //         YearOfTour = tournament.YearOfTour,
        //         Groups = tournament.Groups
        //     };
        // }
    }
}