using System.Collections.Generic;
using MetroshkaFestival.Application.WebModels.Filters;
using MetroshkaFestival.Application.WebModels.Tournaments.Teams;
using MetroshkaFestival.Core.Models.Common;
using MetroshkaFestival.Data.Entities;

namespace MetroshkaFestival.Application.Queries.Models.Teams
{
    public class GetTeamListQueryModel : ReturnQueryModel
    {
        public string ReturnUrl { get; init; }
        public string TournamentNameAndCategory { get; init; }
        public int TournamentId { get; init; }
        public string AgeGroupName { get; init; }
        public TeamSortQueryModel Sort { get; init; }
        public TeamFilterQueryModel Filter { get; init; }
    }

    public class TeamListModel
    {
        public string Error { get; set; }
        public GetTeamListQueryModel Query { get; set; }
        public TeamListItemModel[] Teams { get; set; }
    }

    public class TeamListItemModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public City TeamCity { get; set; }
        public string SchoolName { get; set; }
        public TeamStatus TeamStatus { get; set; }
        public int CountMembers { get; set; } = 0;
    }

    public class TeamModel : TeamListItemModel
    {
        public string Error { get; set; }
        public string ReturnUrl { get; set; }
        public string TeamName { get; set; }
        public int TournamentId { get; set; }
        public string AgeGroupName { get; set; }
        public string TournamentNameAndCategory { get; init; }
        public ICollection<Player> Players { get; set; } = new List<Player>();
    }
}