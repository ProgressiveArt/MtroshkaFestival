using System.Linq;
using MetroshkaFestival.Core.Models.Common;
using MetroshkaFestival.Data.Entities;

namespace MetroshkaFestival.Application.WebModels.Filters
{
    public class TournamentFilterQueryModel : FilterQueryModel<Tournament>
    {
        public string Name { get; set; }
        public int? YearOfTour { get; set; }
        public int? CityId { get; set; }

        public override IQueryable<Tournament> Apply(IQueryable<Tournament> source)
        {
            var query = source;

            if (Name != null)
            {
                var name = Name.ToUpper();
                query = query.Where(x => x.Name.ToUpper().Contains(name));
            }

            if (YearOfTour.HasValue)
            {
                query = query.Where(x => x.YearOfTour == YearOfTour.Value);
            }

            if (CityId.HasValue)
            {
                query = query.Where(x => x.City.Id == CityId.Value);
            }

            return query;
        }
    }
}