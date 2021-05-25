using MetroshkaFestival.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MetroshkaFestival.Data.EntityConfiguration
{
    public class MatchEntityConfiguration : IEntityTypeConfiguration<Match>
    {
        public void Configure(EntityTypeBuilder<Match> entity)
        {
            entity.Property("GroupId")
                .IsRequired();

            entity.Property("TournamentId")
                .IsRequired();

            entity.Property("FirstTeamId")
                .IsRequired();

            entity.Property("SecondTeamId")
                .IsRequired();
        }
    }
}