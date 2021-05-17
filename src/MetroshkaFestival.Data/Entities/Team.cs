using System.Collections.Generic;
using Interfaces.Core;

namespace MetroshkaFestival.Data.Entities
{
    public class Team : IEntity, IIdEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Player> Players { get; set; } = new List<Player>();
        public ICollection<Trainer> Trainers { get; set; } = new List<Trainer>();
    }
}