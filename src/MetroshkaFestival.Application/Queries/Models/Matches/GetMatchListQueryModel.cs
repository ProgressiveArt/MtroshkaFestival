using System;
using System.Collections.Generic;
using MetroshkaFestival.Data.Entities;

namespace MetroshkaFestival.Application.Queries.Models.Matches
{
    public class GetMatchListQueryModel
    {
        public string ReturnUrl { get; init; }
        public int TournamentId { get; init; }
        public string AgeGroupName { get; init; }
        public string TournamentNameAndCategory { get; init; }
        public bool IsAddMatches { get; set; } = false;
    }

    public class MatchListModel
    {
        public string Error { get; set; }
        public bool TournamentIsOver { get; set; }
        public GetMatchListQueryModel Query { get; set; }
        public ICollection<MatchListItemModel> Matches { get; set; }
    }

    public class MatchListItemModel{
        public int MatchId { get; set; }
        public DateTime MatchDateTime { get; set; }
        public FieldNumber FieldNumber { get; set; }
        public StageNumber StageNumber { get; set; }

        public string FirstTeamName  { get; set; }
        public int FirstTeamId { get; set; }
        public string SecondTeamName  { get; set; }
        public int SecondTeamId  { get; set; }

        public int FirstTeamGoalsScore { get; set; }
        public int FirstTeamPenaltyGoalsScore { get; set; }
        public int SecondTeamGoalsScore { get; set; }
        public int SecondTeamPenaltyGoalsScore { get; set; }
        public MatchFinalResult MatchFinalResult { get; set; }
    }
}