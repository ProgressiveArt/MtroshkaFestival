using System;
using System.Collections.Generic;

namespace MetroshkaFestival.Data.Entities
{
    public class Tournament
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public City City { get; set; }

        public DateTime YearOfTour  { get; set; }
        public string Description  { get; set; }

        public ICollection<Group> Groups { get; set; } = new List<Group>();
    }
}