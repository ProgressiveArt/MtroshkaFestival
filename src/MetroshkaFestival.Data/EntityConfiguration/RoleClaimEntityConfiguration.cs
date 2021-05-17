using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MetroshkaFestival.Data.EntityConfiguration
{
    public sealed class RoleClaimEntityConfiguration : IEntityTypeConfiguration<IdentityRoleClaim<int>>
    {
        public void Configure(EntityTypeBuilder<IdentityRoleClaim<int>> entity)
        {
            entity.ToTable("RoleClaims");
        }
    }
}