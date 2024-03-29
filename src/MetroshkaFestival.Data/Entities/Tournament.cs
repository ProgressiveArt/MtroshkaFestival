﻿using System;
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
        public DateTime IsSetOpenUntilDate { get; set; }
        public bool IsTournamentOver { get; set; }
        public bool IsHiddenFromPublic { get; set; }

        public ICollection<AgeCategory> AgeCategories { get; set; } = new List<AgeCategory>();

        [NotMapped] public string Name => $"{(Type == TournamentType.SuperCup ? "(" + Type.AsString(EnumFormat.Description) + ")" : "")}МЕТРОШКА-{YearOfTour}:{City.Name}";
    }

    public enum TournamentType
    {
        [Description("Обычный")]
        Default = 1,
        [Description("Суперкубок")]
        SuperCup = 2
    }
}