using System.Collections.Generic;
using MetroshkaFestival.Application.WebModels.Filters;
using MetroshkaFestival.Application.WebModels.Tournaments;
using MetroshkaFestival.Core.Models.Common;
using MetroshkaFestival.Data.Entities;
using PagedList.Core;

namespace MetroshkaFestival.Application.Queries.Models.Tournaments
{
    public class GetTournamentListQueryModel : ReturnQueryModel
    {
        public TournamentSortQueryModel Sort { get; init; }
        public TournamentFilterQueryModel Filter { get; init; }
        public PageQueryModel Page { get; set; } = new();
    }

    public class TournamentListModel
    {
        public GetTournamentListQueryModel Query { get; set; }
        public PagedList<TournamentListItemModel> Tournaments { get; set; }
        public string Error { get; set; } = null;
    }

    public class TournamentListItemModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int YearOfTour  { get; set; }
        public City City { get; set; }
        public TournamentType TournamentType { get; set; }
        public bool CanBeRemoved { get; set; }
    }
}