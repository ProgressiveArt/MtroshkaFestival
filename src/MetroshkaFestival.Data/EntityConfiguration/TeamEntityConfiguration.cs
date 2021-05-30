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
            entity.HasKey(x => x.Id);

            entity.Property(x => x.TeamName)
                .IsRequired();

            entity.Property(x => x.SchoolName)
                .IsRequired();

            entity.Property(x => x.TeamStatus)
                .HasDefaultValue(TeamStatus.AwaitConfirmation);

            entity.Property("TeamCityId")
                .IsRequired();

            entity
                .HasMany(e => e.Players)
                .WithOne(x => x.Team)
                .HasForeignKey(x => x.TeamId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}