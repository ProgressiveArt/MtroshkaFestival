using System;

namespace MetroshkaFestival.Data.Entities
{
    public class Player
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        // public File Photo { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int NumberInTeam { get; set; }
        public int TeamId { get; set; }
        public Team Team { get; set; }
    }
}