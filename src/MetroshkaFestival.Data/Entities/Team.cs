using System;
using MetroshkaFestival.Core.Models.Common;

namespace MetroshkaFestival.Data.Entities
{
    public class Team
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public City City { get; set; }
        public DateTime Year { get; set; }
        public TeamStatus TeamStatus { get; set; }
    }
}