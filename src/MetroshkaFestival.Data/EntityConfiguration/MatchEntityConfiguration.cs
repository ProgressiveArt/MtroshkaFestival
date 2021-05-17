using MetroshkaFestival.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MetroshkaFestival.Data.EntityConfiguration
{
    public sealed class MatchEntityConfiguration : IEntityTypeConfiguration<Match>
    {
        public void Configure(EntityTypeBuilder<Match> entity)
        {
            entity.HasIndex(x => x.FirstTeamId)
                .IsUnique();

            entity.HasIndex(x => x.SecondTeamId)
                .IsUnique();

            entity.ToTable("Matches");
        }
    }
}