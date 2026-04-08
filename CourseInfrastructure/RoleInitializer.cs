using CourseDomain.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CourseInfrastructure.Services
{
    public static class RoleInitializer
    {
        public static async Task InitializeAsync(
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            DbCourseContext context)
        {
            string[] roles = { "Admin", "Teacher", "Student" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            string adminEmail = "olga.vereshchuk@courses.com";
            string adminPassword = "Admin123!";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new User
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    Name = "Ольга Верещук"
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);

                if (!result.Succeeded)
                {
                    var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                    throw new Exception("Не вдалося створити адміністратора: " + errors);
                }
            }

            if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
                await userManager.AddToRoleAsync(adminUser, "Admin");

            if (!await userManager.IsInRoleAsync(adminUser, "Teacher"))
                await userManager.AddToRoleAsync(adminUser, "Teacher");

            if (!await userManager.IsInRoleAsync(adminUser, "Student"))
                await userManager.AddToRoleAsync(adminUser, "Student");

            var adminAccount = await context.Accounts
                .FirstOrDefaultAsync(a => a.IdentityUserId == adminUser.Id || a.Email == adminUser.Email);

            if (adminAccount == null)
            {
                adminAccount = new Account
                {
                    Name = adminUser.Name,
                    Email = adminUser.Email!,
                    IdentityUserId = adminUser.Id
                };

                context.Accounts.Add(adminAccount);
            }
            else
            {
                adminAccount.Name = adminUser.Name;
                adminAccount.Email = adminUser.Email!;
                adminAccount.IdentityUserId = adminUser.Id;
            }

            await context.SaveChangesAsync();
        }
    }
}