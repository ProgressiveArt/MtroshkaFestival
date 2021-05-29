using System;
using System.Collections.Generic;

namespace MetroshkaFestival.Application.WebModels.Common
{
    public class SortingModel<TEnumSort>
        where TEnumSort : struct, Enum
    {
        public TEnumSort NumSort { get; set; }
        public SortState SortState { get; set; }
    }

    public class SortingElementsModel<TEnumSort, TTargetElem>
        where TEnumSort : struct, Enum
    {
        public Dictionary<TEnumSort, Func<TTargetElem, object>> SortingDictionary { get; }

        public SortingElementsModel(Dictionary<TEnumSort, Func<TTargetElem, object>> sortingDictionary)
        {
            SortingDictionary = sortingDictionary;
        }
    }

    public enum SortState
    {
        Asc = 1,
        Desc = 2
    }

    public enum SortType
    {
        Simple = 1,
        SingleStateCascade = 2
    }
}