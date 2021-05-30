namespace MetroshkaFestival.Core.Exceptions.ExceptionCodes
{
    public class TournamentExceptionCodes
    {
        public const string AlreadyExist = "Такой турнир уже существует";

        public const string NotFound = "Турнир не найден";
        public const string CanNotBeRemoved = "Невозможно удалить заполненный турнир";
        public const string UnknownType = "Указан не существующий тип турнира";
        public const string AgeCategoryNotFound = "Не найдена возрастная категория турнира";

        public const string YearOfTourIsRequired = "Не указан год проведения турнира";
        public const string TournamentTypeIsRequired = "Не указан тип турнира";
    }
}