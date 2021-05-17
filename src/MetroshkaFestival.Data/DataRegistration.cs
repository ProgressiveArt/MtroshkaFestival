using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MetroshkaFestival.Data
{
    public static class DataRegistration
    {
        public static void UseApplicationData(this IServiceCollection services, string sqlConnectionString)
        {
            services.AddDbContext<DataContext>(options =>
                options.UseNpgsql(
                    sqlConnectionString,
                    b => b.MigrationsAssembly("MetroshkaFestival.Data")
                )
            );
        }
    }
}