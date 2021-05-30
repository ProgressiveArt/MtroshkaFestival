using MetroshkaFestival.Application.Factories;
using MetroshkaFestival.Core.Models.Common;
using MetroshkaFestival.Data.Entities;

namespace MetroshkaFestival.Application.WebModels.Tournaments
{
    public class TournamentSummaryTeamSortQueryModel : SortQueryModel<TournamentSummaryTeamSortingEnum, Team>
    {
        protected override SortingElementsModel<TournamentSummaryTeamSortingEnum, Team> SortingElementsModel { get; } =
            SortingElementsFactory.Create<TournamentSummaryTeamSortingEnum, Team>(
                (TournamentSummaryTeamSortingEnum.Id, x => x.Id),
                (TournamentSummaryTeamSortingEnum.Name, x => x.Name),
                (TournamentSummaryTeamSortingEnum.TeamName, x => x.TeamName),
                (TournamentSummaryTeamSortingEnum.SchoolName, x => x.SchoolName),
                (TournamentSummaryTeamSortingEnum.City, x => x.TeamCity.Name),
                (TournamentSummaryTeamSortingEnum.CountMembers, x => x.Players.Count));
    }

    public enum TournamentSummaryTeamSortingEnum
    {
        Id = 1,
        Name = 2,
        TeamName = 3,
        SchoolName = 4,
        City = 5,
        CountMembers = 6
    }
}