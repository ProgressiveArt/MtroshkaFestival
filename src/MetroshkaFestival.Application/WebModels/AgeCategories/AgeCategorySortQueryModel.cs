using MetroshkaFestival.Application.Factories;
using MetroshkaFestival.Core.Models.Common;
using MetroshkaFestival.Data.Entities;

namespace MetroshkaFestival.Application.WebModels.AgeCategories
{
    public class AgeCategorySortQueryModel : SortQueryModel<AgeCategorySortingEnum, AgeCategory>
    {
        protected override SortingElementsModel<AgeCategorySortingEnum, AgeCategory> SortingElementsModel { get; } =
            SortingElementsFactory.Create<AgeCategorySortingEnum, AgeCategory>(
                (AgeCategorySortingEnum.Id, x => x.Id),
                (AgeCategorySortingEnum.AgeGroup, x => x.AgeGroup),
                (AgeCategorySortingEnum.Range, x => x.RangeOfBirthYears));
    }

    public enum AgeCategorySortingEnum
    {
        Id = 1,
        AgeGroup = 2,
        Range = 3
    }
}