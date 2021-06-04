using MetroshkaFestival.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MetroshkaFestival.Data.EntityConfiguration
{
    public class AgeCategoryEntityConfiguration : IEntityTypeConfiguration<AgeCategory>
    {
        public void Configure(EntityTypeBuilder<AgeCategory> entity)
        {
            entity.HasKey(x => new {x.TournamentId, x.AgeGroup});

            entity
                .HasMany(e => e.Teams)
                .WithOne(x => x.AgeCategory)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            entity
                .HasMany(e => e.Matches)
                .WithOne(x => x.AgeCategory)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}