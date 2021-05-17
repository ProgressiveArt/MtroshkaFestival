namespace MetroshkaFestival.Application.Commands
{
    public record CommandResult(string ErrorCode = null)
    {
        public bool Success { get; init; } = ErrorCode == null;

        public override string ToString()
        {
            return $"{nameof(Success)}: {Success}, {nameof(ErrorCode)}: {ErrorCode}";
        }
    }
}