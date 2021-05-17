using System;
using Interfaces.Application;

namespace MetroshkaFestival.Application.Queries.Records.Tournaments
{
    public record GetTournamentListQueryRecord(int? Skip = null, int? Count = null,
        DateTime? StartDateAfter = null, DateTime? FinishDateBefore = null) : IQuery<GetTournamentListQueryResult>;

    public class GetTournamentListQueryResult : QueryResult<TournamentListModel>
    {
        public static GetTournamentListQueryResult BuildResult(TournamentListModel result = null, string error = null)
        {
            return error != null ? Failed<GetTournamentListQueryResult>(error) : Ok<GetTournamentListQueryResult>(result);
        }
    }

    public class TournamentModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime FinishDate { get; set; }
    }

    public class TournamentListModel
    {
        public int Total { get; set; }
        public TournamentModel[] Tournaments { get; set; } = Array.Empty<TournamentModel>();
    }
}