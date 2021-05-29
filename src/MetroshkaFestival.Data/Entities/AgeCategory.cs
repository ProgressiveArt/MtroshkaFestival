using System;
using System.ComponentModel.DataAnnotations.Schema;
using EnumsNET;
using MetroshkaFestival.Core.Models.Common;

namespace MetroshkaFestival.Data.Entities
{
    public class AgeCategory
    {
        public int Id { get; set; }
        public AgeGroup AgeGroup { get; set; }
        public DateTime MinBirthDate { get; set; }
        public DateTime MaxBirthDate { get; set; }
        public bool CanBeRemoved { get; set; } = true;
        [NotMapped] public string RangeOfBirthYears => $"{MinBirthDate.Year} - {MaxBirthDate.Year} гг. р.";
        [NotMapped] public string Name => $"{AgeGroup.AsString(EnumFormat.Description)}-{RangeOfBirthYears}";
    }
}