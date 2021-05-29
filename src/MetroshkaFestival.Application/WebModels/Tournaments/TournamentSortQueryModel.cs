using MetroshkaFestival.Application.Factories;
using MetroshkaFestival.Core.Models.Common;
using MetroshkaFestival.Data.Entities;

namespace MetroshkaFestival.Application.WebModels.Tournaments
{
    public class TournamentSortQueryModel : SortQueryModel<TournamentSortingEnum, Tournament>
    {
        protected override SortingElementsModel<TournamentSortingEnum, Tournament> SortingElementsModel { get; } =
            SortingElementsFactory.Create<TournamentSortingEnum, Tournament>(
                (TournamentSortingEnum.Id, x => x.Id),
                (TournamentSortingEnum.Name, x => x.Name),
                (TournamentSortingEnum.City, x => x.City.Name),
                (TournamentSortingEnum.YearOfTour, x => x.YearOfTour));
    }

    public enum TournamentSortingEnum
    {
        Id = 1,
        Name = 2,
        City = 3,
        YearOfTour = 4
    }
}