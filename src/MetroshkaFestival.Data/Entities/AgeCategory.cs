using System;
using System.ComponentModel.DataAnnotations.Schema;
using MetroshkaFestival.Core.Models.Common;

namespace MetroshkaFestival.Data.Entities
{
    public class AgeCategory
    {
        public int Id { get; set; }
        public AgeGroup AgeGroup { get; set; }
        public DateTime MinBirthDate { get; set; }
        public DateTime MaxBirthDate { get; set; }
        [NotMapped] public string RangeOfBirthYears => $"{MinBirthDate.Year} - {MaxBirthDate.Year} гг. р.";
    }
}