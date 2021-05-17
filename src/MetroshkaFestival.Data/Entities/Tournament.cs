using System;
using Interfaces.Core;

namespace MetroshkaFestival.Data.Entities
{
    public class Tournament : IEntity, IIdEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public DateTime StartDate  { get; set; }
        public DateTime FinishDate  { get; set; }
    }
}