using DeunaCall.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace DeunaCall.Data;

public static class DbSeeder
{
    public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        string[] roleNames = { "SuperAdmin", "Nurse", "Patient" };
        
        foreach (var roleName in roleNames)
        {
            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        // Crear Enfermera de Prueba
        var nurseUser = await userManager.FindByNameAsync("enfermera1");
        if (nurseUser == null)
        {
            var newNurse = new ApplicationUser
            {
                UserName = "enfermera1",
                Email = "enfermera1@deunacall.com",
                FullName = "Enfermera de Prueba"
            };
            var createPowerUser = await userManager.CreateAsync(newNurse, "DeunaCall2026*");
            if (createPowerUser.Succeeded)
            {
                await userManager.AddToRoleAsync(newNurse, "Nurse");
            }
        }
    }
}
