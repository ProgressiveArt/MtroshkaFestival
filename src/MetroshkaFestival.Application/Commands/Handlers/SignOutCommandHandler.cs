using System.Threading;
using System.Threading.Tasks;
using Interfaces.Application;
using MetroshkaFestival.Application.Commands.Records.Account;
using MetroshkaFestival.Application.Services;

namespace MetroshkaFestival.Application.Commands.Handlers
{
    public class SignOutCommandHandler : ICommandHandler<SignOutCommandRecord, CommandResult>
    {
        private readonly UserService _userService;

        public SignOutCommandHandler(UserService userService)
        {
            _userService = userService;
        }

        public async Task<CommandResult> Handle(SignOutCommandRecord commandRecord, CancellationToken ct)
        {
            await _userService.SignOut();
            return new CommandResult();
        }
    }
}