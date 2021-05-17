using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using MetroshkaFestival.Core.Models.Common;
using Microsoft.AspNetCore.Identity;

namespace MetroshkaFestival.Data.Entities
{
    public class User : IdentityUser<int>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
        public ICollection<UserRole> Roles { get; set; } = new List<UserRole>();

        [NotMapped]
        public ApplicationRole? Role => Enum.TryParse<ApplicationRole>(Roles.FirstOrDefault()?.Role?.Name, out var role)
            ? role
            : default(ApplicationRole?);
    }
}