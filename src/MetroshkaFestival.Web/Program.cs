using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Extensions.DependencyInjection;
using MetroshkaFestival.Core.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace MetroshkaFestival.Web
{
    public class Program
    {
        private const string Template = "[[{Timestamp:HH:mm:ss} {Level:u3} {RequestId}] UserId: {UserId}] {Message:lj}{NewLine}{Exception}";

        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                .WriteTo.Console(outputTemplate: Template)
                .WriteTo.File("../../logs/logfile.log",
                    outputTemplate: Template,
                    rollingInterval: RollingInterval.Day)
                .Enrich.WithRequestId()
                .Enrich.WithUserId()
                .Enrich.WithProperty("ProjectName", "today")
                .CreateLogger();

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .UseUrls("http://localhost:5000")
                        .UseStartup<Startup>()
                        .UseKestrel(options =>
                        {
                            options.Limits.MaxRequestBodySize = null;
                            options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(1);
                        })
                        .UseSerilog();
                });
        }
    }
}