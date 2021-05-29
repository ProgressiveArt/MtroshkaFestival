namespace MetroshkaFestival.Data.Entities
{
    public class City
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool CanBeRemoved { get; set; } = true;
    }
}