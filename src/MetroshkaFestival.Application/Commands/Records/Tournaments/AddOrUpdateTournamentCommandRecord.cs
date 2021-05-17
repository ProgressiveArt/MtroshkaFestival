using System;
using System.ComponentModel.DataAnnotations;
using Interfaces.Application;

namespace MetroshkaFestival.Application.Commands.Records.Tournaments
{
    public record AddOrUpdateTournamentCommandRecord(
        int? Id = null,

        [Required(ErrorMessage = "Не введено название турнира")]
        string Name = null,

        [Required(ErrorMessage = "Не указана дата начала турнира")]
        DateTime? StartDate = null,

        [Required(ErrorMessage = "Не указана дата завершения турнира")]
        DateTime? FinishDate = null) : ICommand<CommandResult>;

    public sealed class AddOrUpdateTournamentCommandResult : CommandResult
    {
        public static CommandResult BuildResult(string error = null)
        {
            return error != null ? Failed(error) : Ok();
        }
    }
}