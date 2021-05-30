using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using EnumsNET;

namespace MetroshkaFestival.Data.Entities
{
    public class Tournament
    {
        public int Id { get; set; }
        public TournamentType Type { get; set; } = TournamentType.Default;
        public City City { get; set; }

        public int YearOfTour  { get; set; }
        public string Description  { get; set; }
        public bool CanBeRemoved { get; set; } = true;

        public ICollection<AgeCategory> AgeCategories { get; set; } = new List<AgeCategory>();

        // public ICollection<Match> Matches { get; set; } = new List<Match>();

        [NotMapped] public string Name => $"({Type.AsString(EnumFormat.Description)})МЕТРОШКА-{YearOfTour}:{City.Name}";
    }

    public enum TournamentType
    {
        [Description("Обычный")]
        Default = 1,
        [Description("Суперкубок")]
        SuperCup = 2
    }
}