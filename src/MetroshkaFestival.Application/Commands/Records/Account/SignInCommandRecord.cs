using System.ComponentModel.DataAnnotations;
using Interfaces.Application;

namespace MetroshkaFestival.Application.Commands.Records.Account
{
    public record SignInCommandRecord(
        [Required(ErrorMessage = "Логин обязателен для ввода")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Пожалуйста введите корректный адрес электронной почты")]
        string Username,

        [Required(ErrorMessage = "Пароль обязателен для ввода")]
        [DataType(DataType.Password, ErrorMessage = "Пожалуйста введите корректный адрес электронной почты")]
        string Password) : ICommand<CommandResult>;

    public sealed class SignInCommandResult : CommandResult
    {
        public static CommandResult BuildResult(string error = null)
        {
            return error != null ? Failed(error) : Ok();
        }
    }
}