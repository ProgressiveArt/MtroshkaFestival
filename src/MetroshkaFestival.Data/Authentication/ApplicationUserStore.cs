using MetroshkaFestival.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace MetroshkaFestival.Data.Authentication
{
    public class ApplicationUserStore : UserStore<User, IdentityRole<int>, DataContext, int, IdentityUserClaim<int>, UserRole, IdentityUserLogin<int>, IdentityUserToken<int>, IdentityRoleClaim<int>>
    {
        public ApplicationUserStore(DataContext context, IdentityErrorDescriber describer = null) : base(context, describer)
        {
            AutoSaveChanges = false;
        }
    }
}