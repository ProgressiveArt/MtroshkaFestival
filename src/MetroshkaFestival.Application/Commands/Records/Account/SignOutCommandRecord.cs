using Interfaces.Application;

namespace MetroshkaFestival.Application.Commands.Records.Account
{
    public record SignOutCommandRecord : ICommand<CommandResult>;
}