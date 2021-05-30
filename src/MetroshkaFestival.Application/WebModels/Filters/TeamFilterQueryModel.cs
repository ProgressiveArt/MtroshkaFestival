using System.Linq;
using MetroshkaFestival.Core.Models.Common;
using MetroshkaFestival.Data.Entities;

namespace MetroshkaFestival.Application.WebModels.Filters
{
    public class TeamFilterQueryModel : FilterQueryModel<Team>
    {
        public int? CityId { get; set; }

        public override IQueryable<Team> Apply(IQueryable<Team> source)
        {
            var query = source;

            if (CityId.HasValue)
            {
                query = query.Where(x => x.TeamCity.Id == CityId.Value);
            }

            return query;
        }
    }
}