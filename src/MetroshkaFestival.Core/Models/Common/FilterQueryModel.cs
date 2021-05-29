using System.Linq;

namespace MetroshkaFestival.Core.Models.Common
{
    public abstract class FilterQueryModel<TViewItemModel>
    {
        public abstract IQueryable<TViewItemModel> Apply(IQueryable<TViewItemModel> source);
    }
}