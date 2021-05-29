using System.ComponentModel.DataAnnotations;
using MetroshkaFestival.Core.Models.Common;

namespace MetroshkaFestival.Application.Commands.Records.AgeCatehories
{
    public record AddAgeCategoryCommandRecord(string ReturnUrl,
        [Required(ErrorMessage = "Поле обязательно к заполнению")]
        AgeGroup? AgeGroup = null,
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Год не может быть отрицательным")]
        int? MinBirthYear = null,
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Год не может быть отрицательным")]
        int? MaxBirthYear = null);
}