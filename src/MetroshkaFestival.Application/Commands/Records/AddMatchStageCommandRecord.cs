namespace MetroshkaFestival.Application.Commands.Records
{
    public record AddMatchStageCommandRecord(string ReturnUrl, int TournamentId, string AgeGroupName, string TournamentNameAndCategory);
}