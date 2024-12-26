using CabRosterApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace CabRosterApp.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ILogger logger)
        {
            var adminEmail = "admin@gmail.com";
            var adminPassword = "Admin@12345";  // Could be moved to appsettings for security reasons
            var adminName = "Admin";
            var mobileNumber = "1234567890";

            try
            {
                // Check if the admin user already exists
                var user = await userManager.FindByEmailAsync(adminEmail);
                if (user == null)
                {
                    // Create the admin user
                    user = new ApplicationUser
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        Name = adminName,
                        MobileNumber = mobileNumber,
                        IsApproved = true  // Admin is auto-approved
                    };

                    var result = await userManager.CreateAsync(user, adminPassword);
                    if (result.Succeeded)
                    {
                        // Check if Admin role exists and create if not
                        if (!await roleManager.RoleExistsAsync("Admin"))
                        {
                            await roleManager.CreateAsync(new IdentityRole("Admin"));
                        }

                        await userManager.AddToRoleAsync(user, "Admin");
                        logger.LogInformation($"Admin user created successfully with email: {adminEmail}");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            logger.LogError($"Error creating admin user: {error.Description}");
                        }
                    }
                }

                // Ensure the "User" role exists
                if (!await roleManager.RoleExistsAsync("User"))
                {
                    await roleManager.CreateAsync(new IdentityRole("User"));
                }

                logger.LogInformation("Roles and admin user setup completed.");
            }
            catch (Exception ex)
            {
                logger.LogError($"An error occurred while seeding the database: {ex.Message}");
            }
        }
    }
}
