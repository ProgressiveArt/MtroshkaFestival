using System.Collections.Generic;
using MetroshkaFestival.Application.WebModels.Tournaments;
using MetroshkaFestival.Core.Models.Common;
using MetroshkaFestival.Data.Entities;

namespace MetroshkaFestival.Application.Queries.Models.Tournaments
{
    public class GetTournamentSummaryQueryModel : ReturnQueryModel
    {
        public string ReturnUrl { get; init; }
        public int TournamentId { get; init; }
        public TournamentSummaryTeamSortQueryModel Sort { get; init; }
    }

    public class TournamentModel : TournamentListItemModel
    {
        public string Error { get; set; }
        public GetTournamentSummaryQueryModel Query { get; set; }
        public string ReturnUrl { get; set; }
        public string Description  { get; set; }
        public bool IsHiddenFromPublic { get; set; }

        public ICollection<AgeCategory> AgeCategories { get; set; } = new List<AgeCategory>();
        public ICollection<Team> Teams { get; set; } = new List<Team>();
    }
}