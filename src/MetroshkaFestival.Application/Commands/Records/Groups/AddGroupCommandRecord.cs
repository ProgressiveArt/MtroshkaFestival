using System.ComponentModel.DataAnnotations;
using MetroshkaFestival.Core.Models.Common;

namespace MetroshkaFestival.Application.Commands.Records.Groups
{
    public record AddGroupCommandRecord(string ReturnUrl, int TournamentId,
        [Required]
        GroupNames? Name = null,
        [Required]
        int? AgeCategoryId = null);
}