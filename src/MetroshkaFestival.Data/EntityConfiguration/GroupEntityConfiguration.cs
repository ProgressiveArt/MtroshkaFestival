using MetroshkaFestival.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MetroshkaFestival.Data.EntityConfiguration
{
    public class GroupEntityConfiguration : IEntityTypeConfiguration<Group>
    {
        public void Configure(EntityTypeBuilder<Group> entity)
        {
            entity.Property(x => x.Name)
                .IsRequired();

            entity.Property("AgeCategoryTournamentId").IsRequired();
            entity.Property("AgeCategoryAgeGroup").IsRequired();
        }
    }
}