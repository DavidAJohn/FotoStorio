using System.Linq;
using System.Threading.Tasks;
using FotoStorio.Shared.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace FotoStorio.Server.Data.Identity
{
    public class SeedIdentity
    {
        private static IConfiguration _config;

        public static async Task SeedUsersAndRolesAsync(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration config)
        {
            _config = config;
            await SeedRolesAsync(roleManager);
            await SeedUsersAsync(userManager);
        }

        public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            if (!roleManager.Roles.Any())
            {
                if (!await roleManager.RoleExistsAsync("Administrator"))
                {
                    var role = new IdentityRole
                    {
                        Name = "Administrator"
                    };

                    await roleManager.CreateAsync(role);
                }

                if (!await roleManager.RoleExistsAsync("User"))
                {
                    var role = new IdentityRole
                    {
                        Name = "User"
                    };

                    await roleManager.CreateAsync(role);
                }
            }
        }

        public static async Task SeedUsersAsync(UserManager<AppUser> userManager)
        {
            if (!userManager.Users.Any())
            {
                // create the admin user
                string adminEmail = _config["AdminAuthentication:Email"];
                string adminPassword = _config["AdminAuthentication:Password"];
                
                var adminUser = new AppUser
                {
                    DisplayName = "Admin",
                    Email = adminEmail,
                    UserName = adminEmail,
                    Address = new Address
                    {
                        FirstName = "Admin",
                        LastName = "User",
                        Street = "1 Admin Avenue",
                        SecondLine = "-",
                        City = "-",
                        County = "-",
                        PostCode = "-"
                    }
                };

                var adminResult = await userManager.CreateAsync(adminUser, adminPassword);

                if (adminResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Administrator");
                }

                // // create a standard user for development
                // var user = new AppUser
                // {
                //     DisplayName = "David",
                //     Email = "david@test.com",
                //     UserName = "david@test.com",
                //     Address = new Address
                //     {
                //         FirstName = "David",
                //         LastName = "User",
                //         Street = "1 High Road",
                //         SecondLine = "-",
                //         City = "-",
                //         County = "London",
                //         PostCode = "SW9 1DJ"
                //     }
                // };

                // var userResult = await userManager.CreateAsync(user, "Pa$$w0rd");

                // if (userResult.Succeeded)
                // {
                //     await userManager.AddToRoleAsync(user, "User");
                // }
            }
        }
    }
}