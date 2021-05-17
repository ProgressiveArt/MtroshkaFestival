using MetroshkaFestival.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace MetroshkaFestival.Data.Authentication
{
    public class ApplicationRoleStore : RoleStore<IdentityRole<int>, DataContext, int, UserRole, IdentityRoleClaim<int>>
    {
        public ApplicationRoleStore(DataContext context, IdentityErrorDescriber describer = null) : base(context, describer)
        {
            AutoSaveChanges = false;
        }
    }
}