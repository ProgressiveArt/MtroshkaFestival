using System;
using System.ComponentModel;

namespace MetroshkaFestival.Data.Entities
{
    public class Match
    {
        public int Id { get; set; }
        public AgeCategory AgeCategory { get; set; }
        public DateTime MatchDateTime { get; set; }

        public FieldNumber FieldNumber { get; set; }
        public StageNumber StageNumber { get; set; }

        public Team FirstTeam  { get; set; }
        public Team SecondTeam  { get; set; }

        public int FirstTeamGoalsScore { get; set; }
        public int FirstTeamPenaltyGoalsScore { get; set; }

        public int SecondTeamGoalsScore { get; set; }
        public int SecondTeamPenaltyGoalsScore { get; set; }

        public MatchFinalResult MatchFinalResult { get; set; }
    }

    public enum MatchFinalResult
    {
        [Description("Победила команда 1")]
        WinFirst = 1,
        [Description("Поле команда 2")]
        WinSecond = 2,
        [Description("Ведется определение победителя")]
        Unknown= 3
    }

    public enum FieldNumber
    {
        [Description("Поле 1")]
        FieldOne = 1,
        [Description("Поле 2")]
        FieldTwo = 2,
        [Description("Поле 3")]
        FieldThree= 3,
        [Description("Поле 4")]
        FieldFour = 4
    }

    public enum StageNumber
    {
        [Description("Этап 1")]
        StageOne = 1,
        [Description("Этап 2")]
        StageTwo = 2,
        [Description("Этап 3")]
        StageThree= 3,
        [Description("Этап 4")]
        StageFour = 4,
        [Description("Этап 5")]
        StageFive = 5,
        [Description("Этап 6")]
        StageSix = 6,
        [Description("Этап 7")]
        StageSeven = 7,
        [Description("Этап 8")]
        StageEight = 8,
        [Description("Плей-офф(1/8)")]
        PlayOffOneEight = 9,
        [Description("Полуфинал")]
        Semifinal = 10,
        [Description("Финал")]
        Final = 11
    }
}