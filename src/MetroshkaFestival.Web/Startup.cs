using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using FluentValidation.AspNetCore;
using MetroshkaFestival.Application;
using MetroshkaFestival.Core;
using MetroshkaFestival.Core.Extensions;
using MetroshkaFestival.Core.Filters;
using MetroshkaFestival.Data;
using MetroshkaFestival.Data.Authentication;
using MetroshkaFestival.Data.Entities;
using MetroshkaFestival.Web.Middleware;
using MetroshkaFestival.Web.Swagger;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Serilog;

namespace MetroshkaFestival.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews(options => options.Filters.Add(new HttpResponseExceptionFilter()))
                .AddFluentValidation()
                .AddRazorRuntimeCompilation();

            services.AddIdentity<User, IdentityRole<int>>(o =>
                {
                    o.Password.RequireDigit = true;
                    o.Password.RequireLowercase = true;
                    o.Password.RequireUppercase = true;
                    o.Password.RequireNonAlphanumeric = false;
                    o.Password.RequiredLength = 8;
                    o.User.RequireUniqueEmail = true;
                })
                .AddEntityFrameworkStores<DataContext>()
                .AddUserStore<ApplicationUserStore>()
                .AddRoleStore<ApplicationRoleStore>()
                .AddDefaultTokenProviders();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme);
            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.SameSite = SameSiteMode.Strict;
                options.LoginPath = null;
                options.AccessDeniedPath = null;
                options.Events = new CookieAuthenticationEvents
                {
                    OnRedirectToLogin = context =>
                    {
                        context.Response.StatusCode = 401;
                        return Task.CompletedTask;
                    },
                    OnRedirectToAccessDenied = context =>
                    {
                        context.Response.StatusCode = 403;
                        return Task.CompletedTask;
                    }
                };
            });

            var sqlConnectionString = Configuration.GetConnectionString("DefaultConnection");
            services.UseApplicationData(sqlConnectionString);

            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "MetroshkaFestival API", Version = "v1" });
                c.DocumentFilter<ApiDocumentFilter>();
            });
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureContainer(ContainerBuilder builder)
        {
            var sqlConnectionString = Configuration.GetConnectionString("DefaultConnection");

            builder.RegisterModule(new DataModule(sqlConnectionString));
            builder.RegisterModule(new ApplicationModule());
            builder.RegisterModule<CoreModule>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            if (!env.IsProduction())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "MetroshkaFestival API"); });
            }

            app.ConfigureExceptionHandler();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseWorkContext();
            app.UseSerilogRequestLogging();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "/index",
                    pattern: "{controller=Tournament}/{action=Index}");

                endpoints.MapControllerRoute(
                    name: "/",
                    pattern: "{controller=Tournament}/{action=Index}");

                endpoints.MapControllerRoute(name: "defaults",
                    pattern: "{controller}/{action}");

                endpoints.MapControllers();
            });

            InitializeDatabase(app.ApplicationServices);
        }

        private void InitializeDatabase(IServiceProvider serviceProvider)
        {
            Seeder.Migrate(serviceProvider);
            Seeder.CreateRoles(serviceProvider).Wait();
            Seeder.CreateSuperUser(serviceProvider).Wait();
            Seeder.CreateDefaultCities(serviceProvider).Wait();
        }
    }
}