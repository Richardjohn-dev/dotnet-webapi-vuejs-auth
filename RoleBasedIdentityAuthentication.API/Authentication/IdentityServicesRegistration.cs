
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Identity;
using RoleBasedIdentityAuthentication.API.Data;
using RoleBasedIdentityAuthentication.API.Models;

namespace RoleBasedIdentityAuthentication.API.Authentication;

public static class IdentityServicesRegistration
{
    public static IServiceCollection ConfigureIdentityServices(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
                .AddNegotiate();

        services.AddAuthorization(options =>
        {
            options.AddPolicy(Constants.Policies.UserAccess, policy =>
                   policy.RequireAssertion(context =>
                               context.User.IsInRole(Constants.Roles.Administrator)
                               || context.User.IsInRole(Constants.Roles.User)));

            options.FallbackPolicy = options.DefaultPolicy;
            //Todo : why does this make CORS issue? probably https
        });

        services.AddIdentity<User, IdentityRole>(config =>
        {
            config.Password.RequiredLength = 5;
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
            options.Cookie.Name = "RDI.Identity.Cookie";
            options.LogoutPath = "/Auth/Logout";

            options.Cookie.HttpOnly = true;
            options.SlidingExpiration = true;

            options.ExpireTimeSpan = TimeSpan.FromMinutes(120);
            options.Cookie.MaxAge = options.ExpireTimeSpan;
            options.EventsType = typeof(CustomCookieAuthenticationEvents);
        });

        services.AddTransient<CustomCookieAuthenticationEvents>();
        services.Configure<AdminSeed>(options =>
                  configuration.GetSection("AdminSeed").Bind(options));
        return services;
    }
}
