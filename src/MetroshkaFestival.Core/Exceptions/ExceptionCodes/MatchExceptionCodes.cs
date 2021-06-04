namespace MetroshkaFestival.Core.Exceptions.ExceptionCodes
{
    public class MatchExceptionCodes
    {
        public const string MustBeEven = "Чтобы сгенерировать этап должно быть четное число команд";
        public const string PreviousStageNotComplited = "Предыдущий этап еще не завершен";
        public const string СurrentStageNotComplited = "Текущий этап еще не завершен";
        public const string FirstMatchStartDateTimeIsRequired = "Необходимо указать время начала первого матча";
    }
}