using System.Collections.Generic;
using EnumsNET;
using MetroshkaFestival.Core.Models.Common;

namespace MetroshkaFestival.Data.Entities
{
    public class Team
    {
        public int Id { get; set; }
        public string TeamName { get; set; }
        public City TeamCity { get; set; }
        public string SchoolName { get; set; }
        public AgeCategory AgeCategory { get; set; }
        public TeamStatus TeamStatus { get; set; } = TeamStatus.AwaitConfirmation;

        public ICollection<Player> Players { get; set; } = new List<Player>();

        public string Name => $"{TeamName}-{(SchoolName != string.Empty ? SchoolName + "-" : string.Empty)}{AgeCategory.AgeGroup.AsString(EnumFormat.Description)}";
    }
}