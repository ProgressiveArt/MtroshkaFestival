using System;
using System.Linq;
using System.Reflection;
using MetroshkaFestival.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MetroshkaFestival.Data
{
    public class DataContext : IdentityDbContext<User, IdentityRole<int>, int, IdentityUserClaim<int>, UserRole, IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Tournament> Tournaments { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<AgeCategory> AgeCategories { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<File> Files { get; set; }
        // public DbSet<Match> Matches { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            ApplyConfigurations(builder);
        }

        private void ApplyConfigurations(ModelBuilder modelBuilder)
        {
            var applyGenericMethod = typeof(ModelBuilder)
                .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .Single(m => m.Name == nameof(ModelBuilder.ApplyConfiguration)
                             && m.GetParameters().Length == 1
                             && m.GetParameters().Single().ParameterType.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>));

            var entityTypeConfiguration = typeof(IEntityTypeConfiguration<>);
            var configurations = GetType().GetTypeInfo().Assembly
                .GetTypes()
                .Where(c => c.IsClass && !c.IsAbstract && c.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == entityTypeConfiguration))
                .ToArray();

            foreach (var type in configurations)
            {
                var @interface = type.GetInterfaces()
                    .First(x => x.IsGenericType && x.GetGenericTypeDefinition() == entityTypeConfiguration);
                var applyConcreteMethod = applyGenericMethod.MakeGenericMethod(@interface.GenericTypeArguments[0]);
                applyConcreteMethod.Invoke(modelBuilder, new[] { Activator.CreateInstance(type) });
            }
        }
    }
}