using ApiAuth.Constants;
using ApiAuth.Data.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using WebWorker.Data;
using WebWorker.Data.Entities.Identity;

namespace ApiAuth.Data;

public static class WorkerDbSeeder
{
    public static async Task SeedData(this WebApplication webApplication)
    {
        using var scope = webApplication.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppWorkerDbContext>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<RoleEntity>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UserEntity>>();

        // Ensure the database is created and migrations are applied
        await dbContext.Database.MigrateAsync();

        // Seed roles
        foreach (var role in Roles.AllRoles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new RoleEntity(role));
            }
        }

        // Seed default admin user
        if (!dbContext.Users.Any())
        {
            var adminUser = new UserEntity
            {
                UserName = "admin@gmail.com",
                Email = "admin@gmail.com",
                FirstName = "Юхим",
                LastName = "Манько"
            };
            var result = await userManager.CreateAsync(adminUser, "123456");
            if (result.Succeeded)
            {
                // Assign the admin role to the user
                await userManager.AddToRoleAsync(adminUser, Constants.Roles.Admin);
            }
            else
            {
                throw new Exception($"Failed to create admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }

            var user = new UserEntity
            {
                UserName = "user@ukr.net",
                Email = "user@ukr.net",
                FirstName = "Петро",
                LastName = "Підкаблучник"
            };
            var userResult = await userManager.CreateAsync(user, "123456q");
            if (userResult.Succeeded)
            {
                // Assign the user role to the user
                await userManager.AddToRoleAsync(user, Constants.Roles.User);
            }
            else
            {
                throw new Exception($"Failed to create user: {string.Join(", ", userResult.Errors.Select(e => e.Description))}");
            }
        }

    }
}
