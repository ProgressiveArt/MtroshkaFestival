using MetroshkaFestival.Core.Models.Common;

namespace MetroshkaFestival.Data.Entities
{
    public class Group
    {
        public int Id { get; set; }
        public GroupNames Name { get; set; }
        public AgeCategory AgeCategory { get; set; }
    }
}