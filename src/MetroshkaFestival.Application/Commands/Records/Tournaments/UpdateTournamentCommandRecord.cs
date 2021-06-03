using System;
using System.ComponentModel.DataAnnotations;
using Interfaces.Application;
using MetroshkaFestival.Core.Exceptions.ExceptionCodes;

namespace MetroshkaFestival.Application.Commands.Records.Tournaments
{
    public record UpdateTournamentCommandRecord(string ReturnUrl,
        int TournamentId,
        DateTime? IsSetOpenUntilDate,
        [Required(ErrorMessage = TournamentExceptionCodes.IsTournamentOverIsRequired)] bool IsTournamentOver,
        [Required(ErrorMessage = TournamentExceptionCodes.IsHiddenFromPublicIsRequired)] bool IsHiddenFromPublic) : ICommand<CommandResult>;
}