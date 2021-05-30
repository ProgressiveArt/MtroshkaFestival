using MetroshkaFestival.Application.Factories;
using MetroshkaFestival.Core.Models.Common;
using MetroshkaFestival.Data.Entities;

namespace MetroshkaFestival.Application.WebModels.Tournaments.Teams
{
    public class TeamSortQueryModel : SortQueryModel<TeamSortingEnum, Team>
    {
        protected override SortingElementsModel<TeamSortingEnum, Team> SortingElementsModel { get; } =
            SortingElementsFactory.Create<TeamSortingEnum, Team>(
                (TeamSortingEnum.Id, x => x.Id),
                (TeamSortingEnum.Name, x => x.Name),
                (TeamSortingEnum.TeamName, x => x.TeamName),
                (TeamSortingEnum.SchoolName, x => x.SchoolName),
                (TeamSortingEnum.City, x => x.TeamCity.Name),
                (TeamSortingEnum.CountMembers, x => x.Players.Count),
                (TeamSortingEnum.TeamStatus, x => x.TeamStatus));
    }

    public enum TeamSortingEnum
    {
        Id = 1,
        Name = 2,
        TeamName = 3,
        SchoolName = 4,
        City = 5,
        CountMembers = 6,
        TeamStatus = 7
    }
}