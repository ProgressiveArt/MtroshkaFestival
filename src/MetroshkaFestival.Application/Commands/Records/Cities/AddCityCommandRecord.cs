using System.ComponentModel.DataAnnotations;
using MetroshkaFestival.Core.Exceptions.ExceptionCodes;

namespace MetroshkaFestival.Application.Commands.Records.Cities
{
    public record AddCityCommandRecord(string ReturnUrl,
        [Required(ErrorMessage = CityExceptionCodes.CityNameIsRequired)] string Name = null);
}