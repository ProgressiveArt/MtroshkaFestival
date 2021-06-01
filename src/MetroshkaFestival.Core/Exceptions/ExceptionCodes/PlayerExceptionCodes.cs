namespace MetroshkaFestival.Core.Exceptions.ExceptionCodes
{
    public class PlayerExceptionCodes
    {
        public const string AlreadyExist = "Такой игрок уже существует";

        public const string NotFound = "Игрок не найден";
        public const string NumberIsBusy = "Номер уже занят другим игроком в команде";

        public const string FirstNameIsRequired = "Не указано имя игрока";
        public const string LastNameIsRequired = "Не указана фамилия игрока";
        public const string DateOfBirthIsRequired = "Не указана дата раждения игрока";
        public const string NumberInTeamIsRequired = "Не указан номер игрока в команде";
    }
}