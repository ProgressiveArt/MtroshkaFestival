using System;
using System.ComponentModel.DataAnnotations;
using Interfaces.Application;
using MetroshkaFestival.Core.Exceptions.ExceptionCodes;
using MetroshkaFestival.Data.Entities;

namespace MetroshkaFestival.Application.Commands.Records.Tournaments
{
    public record AddTournamentCommandRecord(string ReturnUrl,
        [Required(ErrorMessage = TournamentExceptionCodes.UnknownType)] TournamentType? TournamentType = null,
        [Required(ErrorMessage = TournamentExceptionCodes.YearOfTourIsRequired)] int? YearOfTour = null,
        [Required(ErrorMessage = CityExceptionCodes.UnknownCity),] int? CityId = null,
        [Required(ErrorMessage = TournamentExceptionCodes.IsSetOpenUntilDateIsRequired)] DateTime? IsSetOpenUntilDate = null,
        string Description = null) : ICommand<CommandResult>;
}