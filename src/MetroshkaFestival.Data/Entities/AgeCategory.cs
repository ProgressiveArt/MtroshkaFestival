using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using EnumsNET;
using MetroshkaFestival.Core.Models.Common;

namespace MetroshkaFestival.Data.Entities
{
    public class AgeCategory
    {
        public int TournamentId { get; set; }
        public Tournament Tournament { get; set; }
        public AgeGroup AgeGroup { get; set; }
        public DateTime MinBirthDate { get; set; }
        public DateTime MaxBirthDate { get; set; }

        public ICollection<Team> Teams { get; set; } = new List<Team>();
        public ICollection<Group> Groups { get; set; } = new List<Group>();

        [NotMapped] public string RangeOfBirthYears => $"{MinBirthDate.Year} - {MaxBirthDate.Year} гг. р.";
        [NotMapped] public string Name => $"{AgeGroup.AsString(EnumFormat.Description)}-{RangeOfBirthYears}";
    }
}