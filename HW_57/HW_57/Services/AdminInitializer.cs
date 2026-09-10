using HW_57.Models;
using Microsoft.AspNetCore.Identity;

namespace HW_57.Services;

public class AdminInitializer
{
    public static async Task SeedAdminUser(
        RoleManager<IdentityRole> roleManager,
        UserManager<User> userManager)
    {
        string[] roles = { "admin", "user" };

        foreach (string role in roles)
        {
            if (await roleManager.FindByNameAsync(role) == null)
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        string adminEmail = "admin@admin.com";
        string adminPassword = "Admin123";

        User? admin = await userManager.FindByEmailAsync(adminEmail);

        if (admin == null)
        {
            admin = new User
            {
                UserName = "Admin",
                Email = adminEmail
            };

            IdentityResult result = await userManager.CreateAsync(admin, adminPassword);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, "admin");
            }
        }
    }
}