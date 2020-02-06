using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pumox.Domain;
using Serilog;

namespace Pumox.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = BuildWebHost(args);
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json")
                        .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            Log.Debug("Starting app...");

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<PumoxDbContext>();

                    context.Database.Migrate();
                    //TODO
                    //DbInitializer.SeedInitialData(context, configuration);

                    //if (configuration.GetValue<bool>("DbInitializer:SeedData"))
                    //    DbInitializer.SeedExampleData(context, configuration);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "An error occured while initilizing database.");
                }
            }

            host.Run();
        }
        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseKestrel()
                .UseStartup<Startup>()
                .UseSerilog()
                .UseUrls("http://*:5000/")
                .Build();

    }
}
