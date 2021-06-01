using System;
using System.ComponentModel.DataAnnotations;
using MetroshkaFestival.Core.Exceptions.ExceptionCodes;

namespace MetroshkaFestival.Application.Commands.Records.Players
{
    public record AddOrUpdatePlayerCommandRecord(string ReturnUrl, int TeamId,
        int? PlayerId = null,
        [Required(ErrorMessage = PlayerExceptionCodes.FirstNameIsRequired)] string FirstName = null,
        [Required(ErrorMessage = PlayerExceptionCodes.LastNameIsRequired)] string LastName = null,
        [Required(ErrorMessage = PlayerExceptionCodes.DateOfBirthIsRequired)] DateTime? DateOfBirth = null,
        [Required(ErrorMessage = PlayerExceptionCodes.NumberInTeamIsRequired)] int? NumberInTeam = null);
}