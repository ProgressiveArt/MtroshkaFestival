using System;
using System.Collections.Generic;
using System.Linq;
using MetroshkaFestival.Core.Extensions;

namespace MetroshkaFestival.Core.Models.Common
{
    public abstract class SortQueryModel<TEnumSort, TViewItemModel> : SortingModel<TEnumSort>
        where TEnumSort : struct, Enum
    {
        protected abstract SortingElementsModel<TEnumSort, TViewItemModel> SortingElementsModel { get; }

        public IQueryable<TViewItemModel> Apply(IEnumerable<TViewItemModel> source)
        {
            return source.SimpleSort(SortingElementsModel, this).AsQueryable();
        }
    }
}