using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RoleBasedIdentityAuthentication.API.Constants;
using System.Security.Claims;

namespace RoleBasedIdentityAuthentication.API.Data
{
    public class ApplicationDbSeeder
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;
        public ApplicationDbSeeder(ApplicationDbContext dbContext, RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            _dbContext = dbContext;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task ManageDataAsync()
        {
            await _dbContext.Database.MigrateAsync(); // same as running local Update-database after migration
            await EnsureRolesAsync();
            await EnsureAdminAsync("admin", "password");
        }

        private async Task EnsureRolesAsync()
        {
            if (!_dbContext.Roles.Any())
            {
                foreach (var role in Enum.GetNames(typeof(Roles)))
                {
                    if (!await _roleManager.RoleExistsAsync(role))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(role));
                    }
                }
            }
        }

        private async Task EnsureAdminAsync(string userName, string password)
        {
            var adminUser = await _userManager.FindByNameAsync(userName);

            if (adminUser == null)
            {
                adminUser = new IdentityUser
                {
                    UserName = userName,
                    EmailConfirmed = true
                };
                await _userManager.CreateAsync(adminUser, password);
                await _userManager.AddToRoleAsync(adminUser, Roles.Administrator.ToString());
                await _userManager.AddClaimAsync(adminUser, new Claim("api.admin", "big.api.cookie"));
            }
        }
    }
}
