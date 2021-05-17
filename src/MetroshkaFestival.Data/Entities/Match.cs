using Interfaces.Core;

namespace MetroshkaFestival.Data.Entities
{
    public class Match : IEntity, IIdEntity
    {
        public int Id { get; set; }
        public int TournamentId { get; set; }
        public Tournament Tournament { get; set; }


        public int? FirstTeamId  { get; set; }
        public Team FirstTeam  { get; set; }

        public int? SecondTeamId  { get; set; }
        public Team SecondTeam  { get; set; }
    }
}