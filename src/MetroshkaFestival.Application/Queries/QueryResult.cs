namespace MetroshkaFestival.Application.Queries
{
    public class QueryResult
    {
        public bool Success { get; set; }
        public string Error { get; set; }
    }

    public class QueryResult<TResult> : QueryResult
    {
        public TResult Result { get; set; }

        protected static TQueryResult Ok<TQueryResult>(TResult result) where TQueryResult : QueryResult<TResult>, new()
        {
            return new()
            {
                Result = result,
                Success = true
            };
        }

        protected static TQueryResult Failed<TQueryResult>(string error) where TQueryResult : QueryResult<TResult>, new()
        {
            return new()
            {
                Error = error,
                Success = false
            };
        }
    }
}