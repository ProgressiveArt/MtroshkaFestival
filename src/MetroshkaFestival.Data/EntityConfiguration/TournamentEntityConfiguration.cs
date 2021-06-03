using MetroshkaFestival.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MetroshkaFestival.Data.EntityConfiguration
{
    public class TournamentEntityConfiguration : IEntityTypeConfiguration<Tournament>
    {
        public void Configure(EntityTypeBuilder<Tournament> entity)
        {
            entity.Property(x => x.CanBeRemoved)
                .IsRequired()
                .HasDefaultValue(true);

            entity.Property(x => x.IsHiddenFromPublic)
                .IsRequired()
                .HasDefaultValue(true);

            entity.Property(x => x.IsTournamentOver)
                .IsRequired()
                .HasDefaultValue(false);

            entity.Property(x => x.Type)
                .IsRequired()
                .HasDefaultValue(TournamentType.Default);

            entity.Property("CityId")
                .IsRequired();

            entity
                .HasMany(e => e.AgeCategories)
                .WithOne(x => x.Tournament)
                .HasForeignKey(x => x.TournamentId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}