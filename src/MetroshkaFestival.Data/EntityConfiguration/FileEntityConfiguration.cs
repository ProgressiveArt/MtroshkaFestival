using MetroshkaFestival.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MetroshkaFestival.Data.EntityConfiguration
{
    public class FileEntityConfiguration : IEntityTypeConfiguration<File>
    {
        public void Configure(EntityTypeBuilder<File> entity)
        {
            entity.Property(x => x.OriginalFileName)
                .IsRequired();

            entity.Property(x => x.Path)
                .IsRequired();

            entity.Property(x => x.ContentType)
                .IsRequired();
        }
    }
}