using System;
using MetroshkaFestival.Data.Entities;

namespace MetroshkaFestival.Application.Commands.Records.Matches
{
    public record UpdateMatchCommandRecord(string ReturnUrl, int MatchId,
        DateTime MatchDateTime,
        string FirstTeamName,
        string SecondTeamName,
        int FirstTeamGoalsScore,
        int FirstTeamPenaltyGoalsScore,
        int SecondTeamGoalsScore,
        int SecondTeamPenaltyGoalsScore,
        MatchFinalResult MatchFinalResult);
}