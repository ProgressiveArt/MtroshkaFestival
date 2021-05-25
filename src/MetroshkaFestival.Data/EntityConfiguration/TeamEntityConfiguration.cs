using MetroshkaFestival.Core.Models.Common;
using MetroshkaFestival.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MetroshkaFestival.Data.EntityConfiguration
{
    public class TeamEntityConfiguration : IEntityTypeConfiguration<Team>
    {
        public void Configure(EntityTypeBuilder<Team> entity)
        {
            entity.Property(x => x.Name)
                .IsRequired();

            entity.Property("CityId")
                .IsRequired();

            entity.Property(x => x.TeamStatus)
                .HasDefaultValue(TeamStatus.AwaitConfirmation);

            entity.HasIndex(x => x.Name)
                .IsUnique();
        }
    }
}