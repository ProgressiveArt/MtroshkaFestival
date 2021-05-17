using System.Threading;
using System.Threading.Tasks;
using Interfaces.Application;
using MetroshkaFestival.Application.Commands.Records.Account;
using MetroshkaFestival.Application.Services;

namespace MetroshkaFestival.Application.Commands.Handlers.Account
{
    public class SignInCommandHandler : ICommandHandler<SignInCommandRecord, CommandResult>
    {
        private readonly UserService _userService;

        public SignInCommandHandler(UserService userService)
        {
            _userService = userService;
        }

        public async Task<CommandResult> Handle(SignInCommandRecord commandRecord, CancellationToken ct)
        {
            var (username, password) = commandRecord;
            var signInErrorCode = await _userService.SignInAsync(username, password, true);
            return SignInCommandResult.BuildResult(signInErrorCode);
        }
    }
}