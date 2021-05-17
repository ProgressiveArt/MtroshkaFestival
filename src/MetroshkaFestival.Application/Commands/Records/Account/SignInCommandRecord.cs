using System.ComponentModel.DataAnnotations;
using Interfaces.Application;

namespace MetroshkaFestival.Application.Commands.Records.Account
{
    public record SignInCommandRecord(
        [Required(ErrorMessage = "Не введен логин")]
        string Username,

        [Required(ErrorMessage = "Не введен пароль")]
        [DataType(DataType.Password)]
        string Password) : ICommand<CommandResult>;
}