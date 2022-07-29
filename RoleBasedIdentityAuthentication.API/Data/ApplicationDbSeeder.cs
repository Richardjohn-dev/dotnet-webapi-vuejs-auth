using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RoleBasedIdentityAuthentication.API.Models;
using System.Security.Claims;

namespace RoleBasedIdentityAuthentication.API.Data
{
    public class ApplicationDbSeeder
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<User> _userManager;
        private readonly AdminSeed _options;

        public ApplicationDbSeeder(
            ApplicationDbContext dbContext,
            UserManager<User> userManager,
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
            if (string.IsNullOrEmpty(userName) && string.IsNullOrEmpty(password)) return;

            if (_options.Update == false)
            {
                var adminUser = await _userManager.FindByNameAsync(userName);
                if (adminUser == null)
                {
                    adminUser = new User
                    {
                        UserName = userName,
                        EmailConfirmed = true
                    };
                    await _userManager.CreateAsync(adminUser, password);

                    var claims = new List<Claim>
                        {
                            //new Claim(ClaimTypes.Name, userName),
                            new Claim(ClaimTypes.Role, Constants.Roles.Administrator),
                            new Claim(ClaimTypes.Role, Constants.Roles.User)
                        };

                    await _userManager.AddClaimsAsync(adminUser, claims);
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

        private async Task<bool> UpdateUsername(User updateAdmin)
        {
            var oldClaim = new Claim(ClaimTypes.NameIdentifier, updateAdmin.UserName);
            await _userManager.RemoveClaimAsync(updateAdmin, oldClaim);
            updateAdmin.UserName = _options.UsernameUpdate;
            await _userManager.AddClaimAsync(updateAdmin, new Claim(ClaimTypes.NameIdentifier, _options.UsernameUpdate));
            return (await _userManager.UpdateAsync(updateAdmin)).Succeeded;
        }
        private async Task<bool> UpdatePassword(User oldAdmin)
        {
            return (await _userManager.ChangePasswordAsync(oldAdmin, _options.Password, _options.PasswordUpdate)).Succeeded;
        }
    }
}
