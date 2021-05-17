using Interfaces.Core;

namespace MetroshkaFestival.Data.Entities
{
    public class Player : IEntity, IIdEntity
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public int TeamId { get; set; }
        public Team Team { get; set; }
    }
}