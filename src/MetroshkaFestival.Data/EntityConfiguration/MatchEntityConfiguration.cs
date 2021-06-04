using MetroshkaFestival.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MetroshkaFestival.Data.EntityConfiguration
{
    public class MatchEntityConfiguration : IEntityTypeConfiguration<Match>
    {
        public void Configure(EntityTypeBuilder<Match> entity)
        {
            entity.Property(x => x.FieldNumber)
                .IsRequired()
                .HasDefaultValue(FieldNumber.FieldOne);

            entity.Property(x => x.MatchFinalResult)
                .IsRequired()
                .HasDefaultValue(MatchFinalResult.Unknown);

            entity.Property(x => x.StageNumber)
                .IsRequired()
                .HasDefaultValue(StageNumber.StageOne);

            entity.Property(x => x.MatchDateTime)
                .IsRequired();

            entity.Property("FirstTeamId")
                .IsRequired();

            entity.Property("SecondTeamId")
                .IsRequired();
        }
    }
}