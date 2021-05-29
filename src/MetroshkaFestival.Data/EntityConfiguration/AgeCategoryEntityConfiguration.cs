using MetroshkaFestival.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MetroshkaFestival.Data.EntityConfiguration
{
    public class AgeCategoryEntityConfiguration : IEntityTypeConfiguration<AgeCategory>
    {
        public void Configure(EntityTypeBuilder<AgeCategory> entity)
        {
            entity.Property(x => x.CanBeRemoved)
                .IsRequired()
                .HasDefaultValue(true);
        }
    }
}