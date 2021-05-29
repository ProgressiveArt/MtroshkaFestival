using System;
using System.Collections.Generic;
using MetroshkaFestival.Core.Models.Common;

namespace MetroshkaFestival.Application.Factories
{
    public static class SortingElementsFactory
    {
        public static SortingElementsModel<TEnumSort, TTargetElem> Create<TEnumSort, TTargetElem>(params (TEnumSort columnKey, Func<TTargetElem, object> selector)[] elements)
            where TEnumSort : struct, Enum
        {
            var sortingDictionary = new Dictionary<TEnumSort, Func<TTargetElem, object>>();
            foreach (var (columnKey, selector) in elements)
            {
                sortingDictionary.Add(columnKey, selector);
            }
            return new SortingElementsModel<TEnumSort, TTargetElem>(sortingDictionary);
        }
    }
}