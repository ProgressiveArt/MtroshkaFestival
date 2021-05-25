using MetroshkaFestival.Core.Models.Common;

namespace MetroshkaFestival.Data.Entities
{
    public class AgeCategory
    {
        public int Id { get; set; }
        public AgeGroup AgeGroup { get; set; }
        public string RangeOfBirthYears { get; set; }
    }
}