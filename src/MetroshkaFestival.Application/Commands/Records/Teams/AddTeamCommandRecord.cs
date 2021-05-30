using System.ComponentModel.DataAnnotations;
using MetroshkaFestival.Core.Exceptions.ExceptionCodes;

namespace MetroshkaFestival.Application.Commands.Records.Teams
{
    public record AddTeamCommandRecord(string ReturnUrl,
        int TournamentId, string AgeGroupName,
        int? TeamId = null,
        [Required(ErrorMessage = TeamExceptionCodes.NameIsRequired)] string TeamName = null,
        [Required(ErrorMessage = CityExceptionCodes.CityNameIsRequired)] int? TeamCityId = null,
        [Required(ErrorMessage = TeamExceptionCodes.SchoolNameIsRequired)] string SchoolName = null);
}