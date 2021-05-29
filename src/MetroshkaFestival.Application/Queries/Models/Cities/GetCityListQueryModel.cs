using Interfaces.Application;
using MetroshkaFestival.Data.Entities;

namespace MetroshkaFestival.Application.Queries.Models.Cities
{
    public class GetCityListQueryModel : IQuery<GetCityCategoryListQueryResult> { }

    public class GetCityCategoryListQueryResult : QueryResult<CityListModel>
    {
        public static GetCityCategoryListQueryResult BuildResult(CityListModel result = null, string error = null)
        {
            return error != null ? Failed<GetCityCategoryListQueryResult>(error) : Ok<GetCityCategoryListQueryResult>(result);
        }
    }

    public class CityListModel
    {
        public City[] Cities { get; set; }
    }
}