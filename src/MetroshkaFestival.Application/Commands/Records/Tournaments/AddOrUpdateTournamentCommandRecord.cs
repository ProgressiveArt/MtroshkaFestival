using System.ComponentModel.DataAnnotations;
using Interfaces.Application;

namespace MetroshkaFestival.Application.Commands.Records.Tournaments
{
    public record AddOrUpdateTournamentCommandRecord(string ReturnUrl,
        int? Id = null,
        string Name = null,
        [Required(ErrorMessage = "Не указан год проведения турнира")] int? YearOfTour = null,
        [Required(ErrorMessage = "Не указан город проведения турнира")] int? CityId = null,
        string Description = null) : ICommand<CommandResult>;

    public sealed class AddOrUpdateTournamentCommandResult : CommandResult
    {
        public static CommandResult BuildResult(string error = null)
        {
            return error != null ? Failed(error) : Ok();
        }
    }
}