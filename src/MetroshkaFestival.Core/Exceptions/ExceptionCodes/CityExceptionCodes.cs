namespace MetroshkaFestival.Core.Exceptions.ExceptionCodes
{
    public class CityExceptionCodes
    {
        public const string AlreadyExist = "Такой город уже существует";

        public const string NotFound = "Город не найден";
        public const string CanNotBeRemoved = "Невозможно удалить использующийся город";
        public const string UnknownCity = "Указан не существующий город";

        public const string CityNameIsRequired = "Не указано название города";
    }
}