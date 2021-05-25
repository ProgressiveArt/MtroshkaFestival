namespace MetroshkaFestival.Data.Entities
{
    public class Match
    {
        public int Id { get; set; }
        public Tournament Tournament { get; set; }
        public Group Group { get; set; }

        public Team FirstTeam  { get; set; }
        public Team SecondTeam  { get; set; }
    }
}