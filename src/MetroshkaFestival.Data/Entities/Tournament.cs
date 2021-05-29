using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MetroshkaFestival.Data.Entities
{
    public class Tournament
    {
        public int Id { get; set; }
        public City City { get; set; }

        public int YearOfTour  { get; set; }
        public string Description  { get; set; }
        public bool CanBeRemoved { get; set; } = true;
        public ICollection<Group> Groups { get; set; } = new List<Group>();

        [NotMapped] public string Name => $"МЕТРОШКА-{YearOfTour}:{City.Name}";
    }
}