using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace RoleBasedIdentityAuthentication.API.Data
{
    public class ApplicationDbSeeder
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AdminSeed _options;

        public ApplicationDbSeeder(
            ApplicationDbContext dbContext,
            UserManager<IdentityUser> userManager,
            IOptions<AdminSeed> options)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _options = options.Value;
        }

        public async Task ManageDataAsync()
        {
            await _dbContext.Database.MigrateAsync();
            await EnsureAdminAsync(_options.Username, _options.Password);
        }

        private async Task EnsureAdminAsync(string userName, string password)
        {

            if (_options.Update == false)
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
                    await _userManager.AddClaimAsync(adminUser, new Claim(ClaimTypes.Role, "Admin"));
                    await _userManager.AddClaimAsync(adminUser, new Claim(ClaimTypes.Role, "User"));
                }
            }
            else if (_options.Update == true && !string.IsNullOrEmpty(_options.PasswordUpdate) || !string.IsNullOrEmpty(_options.UsernameUpdate))
            {
                var updateAdmin = await _userManager.FindByNameAsync(_options.Username);
                if (updateAdmin == null) return;

                if (!string.IsNullOrEmpty(_options.PasswordUpdate))
                {
                    var updated = await UpdatePassword(updateAdmin);
                    if (!updated)
                    {
                        //to do log something
                    }
                }

                if (!string.IsNullOrEmpty(_options.UsernameUpdate))
                {
                    var updated = await UpdateUsername(updateAdmin);
                    if (!updated)
                    {
                        //to do log something
                    }
                }
            }
        }

        private async Task<bool> UpdateUsername(IdentityUser oldAdmin)
        {
            oldAdmin.UserName = _options.UsernameUpdate;
            var userNameUpdatedResult = await _userManager.UpdateAsync(oldAdmin);
            return userNameUpdatedResult.Succeeded;
        }
        private async Task<bool> UpdatePassword(IdentityUser oldAdmin)
        {
            var newPassword = _options.PasswordUpdate;
            var passwordUpdatedResult = await _userManager.ChangePasswordAsync(oldAdmin, _options.Password, newPassword);
            return passwordUpdatedResult.Succeeded;
        }
    }
}
