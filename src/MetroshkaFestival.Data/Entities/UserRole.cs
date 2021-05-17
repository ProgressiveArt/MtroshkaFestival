using Microsoft.AspNetCore.Identity;

namespace MetroshkaFestival.Data.Entities
{
    public class UserRole : IdentityUserRole<int>
    {
        public virtual IdentityRole<int> Role { get; set; }
    }
}