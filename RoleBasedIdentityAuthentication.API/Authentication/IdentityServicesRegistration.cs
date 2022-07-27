
using Microsoft.AspNetCore.Identity;
using RoleBasedIdentityAuthentication.API.Data;
using System.Security.Claims;

namespace RoleBasedIdentityAuthentication.API.Authentication;

public static class IdentityServicesRegistration
{
    public static IServiceCollection ConfigureIdentityServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy("Admin", policyBuilder => policyBuilder.RequireClaim(ClaimTypes.Role, "Admin"));
            options.AddPolicy("User", policyBuilder => policyBuilder.RequireClaim(ClaimTypes.Role, "User"));

        });

        services.AddIdentity<IdentityUser, IdentityRole>(config =>
        {
            config.Password.RequiredLength = 6;
            config.Password.RequireDigit = false;
            config.Password.RequireNonAlphanumeric = false;
            config.Password.RequireUppercase = false;
            config.SignIn.RequireConfirmedEmail = true;
        })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();


        // cookie for AddIdentity
        services.ConfigureApplicationCookie(options =>
        {
            // Cookie settings
            //options.Cookie.HttpOnly = true;
            options.LoginPath = "/Auth/Login";
            options.LogoutPath = "/Auth/Logout";
            //options.ExpireTimeSpan = TimeSpan.FromMinutes(120);
            ////options.Cookie.Expiration.Equals(TimeSpan.Zero);
            options.ExpireTimeSpan = TimeSpan.FromMinutes(120);
            options.Cookie.MaxAge = options.ExpireTimeSpan;
            options.SlidingExpiration = true;
            options.Cookie.Name = "RDrive.Integrator.Identity.Cookie";
            options.EventsType = typeof(CustomCookieAuthenticationEvents);
        });

        services.AddTransient<CustomCookieAuthenticationEvents>();

        return services;
    }
}
