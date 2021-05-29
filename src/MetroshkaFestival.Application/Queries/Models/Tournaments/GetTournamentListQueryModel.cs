using System.Collections.Generic;
using Interfaces.Application;
using MetroshkaFestival.Application.WebModels.Filters;
using MetroshkaFestival.Application.WebModels.Tournaments;
using MetroshkaFestival.Core.Models.Common;
using MetroshkaFestival.Data.Entities;
using PagedList.Core;

namespace MetroshkaFestival.Application.Queries.Models.Tournaments
{
    public class GetTournamentListQueryModel : ReturnQueryModel, IQuery<GetTournamentListQueryResult>
    {
        public TournamentSortQueryModel Sort { get; init; }
        public TournamentFilterQueryModel Filter { get; init; }
        public PageQueryModel Page { get; set; } =  new();
    }

    public class GetTournamentListQueryResult : QueryResult<TournamentListModel>
    {
        public static GetTournamentListQueryResult BuildResult(TournamentListModel result = null, string error = null)
        {
            return error != null ? Failed<GetTournamentListQueryResult>(error) : Ok<GetTournamentListQueryResult>(result);
        }
    }

    public class TournamentListModel
    {
        public GetTournamentListQueryModel Query { get; set; }
        public PagedList<TournamentListItemModel> Tournaments { get; set; }
    }

    public class TournamentListItemModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int YearOfTour  { get; set; }
        public City City { get; set; }
        public bool CanBeRemoved { get; set; }
    }

    public class TournamentModel : TournamentListItemModel
    {
        public string Description  { get; set; }

        public ICollection<Group> Groups { get; set; } = new List<Group>();
    }
}