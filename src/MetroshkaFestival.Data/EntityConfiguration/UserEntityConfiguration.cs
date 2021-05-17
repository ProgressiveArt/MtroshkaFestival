using MetroshkaFestival.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MetroshkaFestival.Data.EntityConfiguration
{
  public sealed class UserEntityConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> entity)
        {
            entity
                .HasMany(x => x.Roles)
                .WithOne()
                .HasForeignKey(x => x.UserId);

            entity.ToTable("Users");
        }
    }
}