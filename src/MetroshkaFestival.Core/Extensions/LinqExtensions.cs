using System;
using System.Collections.Generic;
using System.Linq;
using MetroshkaFestival.Core.Exceptions.Common;
using MetroshkaFestival.Core.Models.Common;

namespace MetroshkaFestival.Core.Extensions
{
    public static class LinqExtensions
    {
        public static IOrderedEnumerable<TTargetElem> SimpleSort<TEnumSort, TTargetElem>(this IEnumerable<TTargetElem> elements,
            SortingElementsModel<TEnumSort, TTargetElem> sortingElementsModel,
            SortingModel<TEnumSort> sortingModel)
            where TEnumSort : struct, Enum
        {
            if (sortingModel == null)
            {
                return elements.OrderBy(x => true);
            }

            if (!sortingElementsModel.SortingDictionary.ContainsKey(sortingModel.NumSort))
            {
                throw SortingException.UnknownCurrentSort(sortingModel.NumSort);
            }

            var sortingElements = new [] {sortingElementsModel.SortingDictionary[sortingModel.NumSort]};
            return Sort(SortType.Simple, sortingModel.SortState, elements, sortingElements);
        }

        private static IOrderedEnumerable<TTargetElem> Sort<TTargetElem>(
            SortType sortType,
            SortState sortState,
            IEnumerable<TTargetElem> elements,
            Func<TTargetElem,object>[] sortingElements)
        {
            var firstSortingElement = sortingElements.First();
            return sortType switch
            {
                SortType.Simple => sortState switch
                {
                    SortState.Asc => elements.OrderBy(firstSortingElement),
                    SortState.Desc => elements.OrderByDescending(firstSortingElement),
                    _ => throw SortingException.SortStateNotFound(sortState)
                },

                SortType.SingleStateCascade => sortState switch
                {
                    SortState.Asc => sortingElements.Aggregate(elements.OrderBy(firstSortingElement),
                        (enumerable, curSorting) => enumerable.ThenBy(curSorting)),
                    SortState.Desc => sortingElements.Aggregate(elements.OrderByDescending(firstSortingElement),
                        (enumerable, curSorting) => enumerable.ThenByDescending(curSorting)),
                    _ => throw SortingException.SortStateNotFound(sortState)
                },

                _ => throw SortingException.SortTypeNotFound(sortType)
            };
        }
    }
}