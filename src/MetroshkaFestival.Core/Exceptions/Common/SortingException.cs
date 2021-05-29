using System;
using MetroshkaFestival.Core.Models.Common;

namespace MetroshkaFestival.Core.Exceptions.Common
{
    public static class SortingException
    {
        public static ApplicationException SortStateNotFound(SortState sortState)
        {
            return new ApplicationException($"Unknown sortState: {sortState:F}");
        }

        public static ApplicationException SortTypeNotFound(SortType sortType)
        {
            return new ApplicationException($"Unknown sortType: {sortType:F}");
        }

        public static ApplicationException UnknownCurrentSort<TEnumSort>(TEnumSort enumSort)
            where TEnumSort : struct, Enum
        {
            return new ApplicationException($"Unknown current sort: {enumSort:F}");
        }
    }
}