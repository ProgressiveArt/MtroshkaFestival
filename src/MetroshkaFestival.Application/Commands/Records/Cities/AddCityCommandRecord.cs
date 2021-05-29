using System.ComponentModel.DataAnnotations;

namespace MetroshkaFestival.Application.Commands.Records.Cities
{
    public record AddCityCommandRecord(string ReturnUrl,
        [Required(ErrorMessage = "Поле обязательно к заполнению")]
        string Name = null);

}