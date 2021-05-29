using System.ComponentModel.DataAnnotations;
using MetroshkaFestival.Core.Models.Common;

namespace MetroshkaFestival.Application.Commands.Records.Groups
{
    public record AddGroupCommandRecord(string ReturnUrl, int TournamentId,
        [Required(ErrorMessage = "Поле обязательно к заполнению")]
        GroupNames? Name = null,
        [Required(ErrorMessage = "Поле обязательно к заполнению")]
        int? AgeCategoryId = null);
}