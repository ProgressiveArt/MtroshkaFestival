namespace MetroshkaFestival.Application.Commands
{
    public class CommandResult
    {
        public bool Success { get; set; }
        public string Error { get; set; }

        public override string ToString()
        {
            return $"{nameof(Success)}: {Success}, {nameof(Error)}: {Error}";
        }
        public static CommandResult Ok()
        {
            return new()
            {
                Success = true
            };
        }

        public static CommandResult Failed(string error)
        {
            return new()
            {
                Success = false,
                Error = error
            };
        }
    }

    public class CommandResult<TResult> : CommandResult
    {
        public TResult Result { get; set; }

        public override string ToString()
        {
            return $"{base.ToString()} {nameof(Result)}: {Result}";
        }

        protected static TCommandResult Ok<TCommandResult>(TResult result) where TCommandResult : CommandResult<TResult>, new()
        {
            return new()
            {
                Result = result,
                Success = true
            };
        }

        protected static TCommandResult Failed<TCommandResult>(string error) where TCommandResult : CommandResult<TResult>, new()
        {
            return new()
            {
                Error = error,
                Success = false
            };
        }
    }
}