using FotoStorio.Server.Data;
using FotoStorio.Server.Data.Identity;
using FotoStorio.Shared.Models.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace FotoStorio.Server
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                try
                {
                    var context = services.GetRequiredService<ApplicationDbContext>();
                    await context.Database.MigrateAsync();
                    
                    await SeedData.SeedBrandsDataAsync(context);
                    await SeedData.SeedCategoriesDataAsync(context);
                    await SeedData.SeedProductsDataAsync(context);

                    // Identity seeding
                    var userManager = services.GetRequiredService<UserManager<AppUser>>();
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                    var identityContext = services.GetRequiredService<IdentityDbContext>();
                    var config = services.GetRequiredService<IConfiguration>();

                    await identityContext.Database.MigrateAsync();
                    await SeedIdentity.SeedUsersAndRolesAsync(userManager, roleManager, config);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while seeding initial data");
                }
            }

            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
