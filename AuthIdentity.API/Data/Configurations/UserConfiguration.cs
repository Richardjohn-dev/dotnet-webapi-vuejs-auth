using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RoleBasedAuthIdentity.API.Models;

namespace RoleBasedAuthIdentity.API.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<ApiUser>
{
    // to do
    // inject config from here TOPTIONS
    public void Configure(EntityTypeBuilder<ApiUser> builder)
    {
        var hasher = new PasswordHasher<ApiUser>();
        builder.HasData(
            new ApiUser
            {
                Id = "8e445865-a24d-4543-a6c6-9443d048cdb9",
                Email = "admin@localhost.com",
                NormalizedEmail = "ADMIN@LOCALHOST.COM",
                FirstName = "System",
                LastName = "Admin",
                UserName = "admin",
                NormalizedUserName = "ADMIN",
                PasswordHash = hasher.HashPassword(null, "admin"),
                EmailConfirmed = true
            },
            new ApiUser
            {
                Id = "9e224968-33e4-4652-b7b7-8574d048cdb9",
                Email = "user@localhost.com",
                NormalizedEmail = "USER@LOCALHOST.COM",
                FirstName = "System",
                LastName = "User",
                UserName = "user",
                NormalizedUserName = "USER",
                PasswordHash = hasher.HashPassword(null, "user"),
                EmailConfirmed = true
            }
        );
    }
}
