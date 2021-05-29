using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MetroshkaFestival.Core.Models.Common;
using MetroshkaFestival.Data.Authentication;
using MetroshkaFestival.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MetroshkaFestival.Data
{
    public static class Seeder
    {
        public static void Migrate(IServiceProvider serviceProvider)
        {
            var dataContext = serviceProvider.GetRequiredService<DataContext>();

            dataContext.Database.Migrate();
        }

        public static async Task CreateRoles(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
            var roleStore = serviceProvider.GetRequiredService<ApplicationRoleStore>();
            var roleNames = Enum.GetValues(typeof(ApplicationRole));

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName.ToString());
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole<int>(roleName.ToString()));
                }
            }

            await roleStore.Context.SaveChangesAsync();
        }

        public static async Task CreateSuperUser(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var userStore = scope.ServiceProvider.GetRequiredService<ApplicationUserStore>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var roleStore = scope.ServiceProvider.GetRequiredService<ApplicationRoleStore>();

            var email = Environment.GetEnvironmentVariable("SUPER_USER_EMAIL");
            var password = Environment.GetEnvironmentVariable("SUPER_USER_PASSWORD");

            await CreateUser(userStore, userManager, roleStore, "admin", "admin", email, password, ApplicationRole.Admin);
        }

        public static async Task CreateDefaultCities(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();

            var cities = new City[]
            {
                new() {Name = "Челябинск"},
                new() {Name = "Копейск"},
                new() {Name = "Пермь"},
                new() {Name = "Коркино"},
                new() {Name = "Уфа"}
            };
            if (!dataContext.Cities.Any())
            {
                foreach (var city in cities)
                {
                    await dataContext.Cities.AddAsync(city);
                    await dataContext.SaveChangesAsync();
                }
            }
        }

        private static async Task CreateUser(ApplicationUserStore userStore,
            UserManager<User> userManager, ApplicationRoleStore roleStore,
            string firstName, string lastName, string email,
            string password, ApplicationRole role)
        {
            var isRegistered = userManager.Users.Any(x => x.NormalizedUserName.Equals(email.ToUpper()));
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || isRegistered)
            {
                return;
            }

            var user = new User
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                UserName = email,
                PhoneNumber = string.Empty,
                RegistrationDate = DateTime.UtcNow,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, password);

            var identityRoleAdmin = roleStore.Roles.First(x => x.NormalizedName.Equals(role.ToString().ToUpper()));
            user.Roles.Add(new UserRole { RoleId = identityRoleAdmin.Id });

            if (!result.Succeeded)
            {
                var error = result.Errors.FirstOrDefault();
                throw new Exception(error.ToString());
            }

            await userStore.Context.SaveChangesAsync();
        }
    }
}