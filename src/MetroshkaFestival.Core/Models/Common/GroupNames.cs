using System.ComponentModel;

namespace MetroshkaFestival.Core.Models.Common
{
    public enum GroupNames
    {
        [Description("Группа А")]
        GroupA = 1,
        [Description("Группа B")]
        GroupB = 2,
        [Description("Группа C")]
        GroupC = 3,
        [Description("Группа D")]
        GroupD = 4,
        [Description("Плей-офф")]
        PlayOff = 5,
    }
}