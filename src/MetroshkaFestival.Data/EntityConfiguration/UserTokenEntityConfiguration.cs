using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MetroshkaFestival.Data.EntityConfiguration
{
    public sealed class UserTokenEntityConfiguration : IEntityTypeConfiguration<IdentityUserToken<int>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserToken<int>> entity)
        {
            entity.ToTable("UserTokens");
        }
    }
}