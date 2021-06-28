using System;
using System.ComponentModel.DataAnnotations;
using MetroshkaFestival.Core.Exceptions.ExceptionCodes;

namespace MetroshkaFestival.Application.Commands.Records.Teams
{
    public record AddRequestToTourCommandRecord(string ReturnUrl,
        int TournamentId, string AgeGroupName,
        string Error = null,
        [Required(ErrorMessage = TeamExceptionCodes.NameIsRequired)] string TeamName = null,
        [Required(ErrorMessage = CityExceptionCodes.CityNameIsRequired)] int? TeamCityId = null,
        [Required(ErrorMessage = TeamExceptionCodes.SchoolNameIsRequired)] string SchoolName = null,
        AddPlayerCommandRecord[] Players = null);

    public record AddPlayerCommandRecord([Required(ErrorMessage = PlayerExceptionCodes.FirstNameIsRequired)] string FirstName = null,
        [Required(ErrorMessage = PlayerExceptionCodes.LastNameIsRequired)] string LastName = null,
        [Required(ErrorMessage = PlayerExceptionCodes.DateOfBirthIsRequired)] DateTime? DateOfBirth = null,
        [Required(ErrorMessage = PlayerExceptionCodes.NumberInTeamIsRequired)] int? NumberInTeam = null);
}