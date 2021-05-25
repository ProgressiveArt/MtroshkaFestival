using System.ComponentModel;

namespace MetroshkaFestival.Core.Models.Common
{
    public enum TeamStatus
    {
        [Description("Ожидает проверки")]
        AwaitConfirmation = 1,
        [Description("Опубликована")]
        Published = 2
    }
}