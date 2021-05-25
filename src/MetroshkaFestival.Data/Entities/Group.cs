namespace MetroshkaFestival.Data.Entities
{
    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsPlayOff { get; set; }
        public AgeCategory AgeCategory { get; set; }
    }
}