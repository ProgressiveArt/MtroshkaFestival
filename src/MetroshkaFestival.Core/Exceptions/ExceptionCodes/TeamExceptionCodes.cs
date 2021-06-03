namespace MetroshkaFestival.Core.Exceptions.ExceptionCodes
{
    public class TeamExceptionCodes
    {
        public const string AlreadyExist = "Такая команда уже существует";

        public const string NotFound = "Команда не найдена";
        public const string MinPlayerCount = "В команде должно быть не менее 12 игроков";
        public const string MaxPlayerCount = "В команде должно быть не более 15 игроков";
        public const string CanNotBeRemoved = "Нельзя удалить опубликованную команду";
        public const string CanNotBeUpdated = "Нельзя обновить опубликованную команду";
        // public const string AgeCategoryNotFound = "Не найдена возрастная категория турнира";

        public const string NameIsRequired = "Не указано название команды";
        public const string SchoolNameIsRequired = "Не указано название учебного заведения";
        public const string CallBackEmailIsRequired = "Email для обратной связи обязателен";
        public const string PlayersIsRequired = "Нельзя создать команду без игроков";
    }
}