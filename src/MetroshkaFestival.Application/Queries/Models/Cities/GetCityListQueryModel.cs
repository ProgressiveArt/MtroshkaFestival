using MetroshkaFestival.Data.Entities;

namespace MetroshkaFestival.Application.Queries.Models.Cities
{
    public class GetCityListQueryModel { }

    public class CityListModel
    {
        public City[] Cities { get; set; }
        public string Error { get; set; } = null;
    }
}