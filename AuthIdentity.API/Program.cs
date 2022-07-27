using RoleBasedAuthIdentity.API.AuthorizationRequirements;
using RoleBasedAuthIdentity.API.Data;
using RoleBasedAuthIdentity.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddCors();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

//builder.Services.AddAuthentication("CookieAuth")
//    .AddCookie("CookieAuth", config =>
//    {
//        config.Cookie.Name = "Grandmas.Cookie";
//        config.LoginPath = "/Home/Authenticate";
//    });

// AUth Trifecta
// Authorization Policy = Authorization Requirements + Authorization Handlers

builder.Services.AddAuthorization(options =>
{
    // --- What is happening under the hood default example

    // default

    // --  Creating an Authorization Policy with builder.
    //var defaultAuthBuilder = new AuthorizationPolicyBuilder();
    //var defaultAuthPolicy = defaultAuthBuilder
    //.RequireAuthenticatedUser()
    //.RequireClaim(ClaimTypes.DateOfBirth)
    //.Build();
    //options.DefaultPolicy = defaultAuthPolicy;



    //options.AddPolicy("Claim.DoB", policyBuilder =>
    //{
    //    policyBuilder.RequireAuthenticatedUser();
    //    policyBuilder.RequireClaim(ClaimTypes.DateOfBirth);
    //});


    // custom claim requirements
    // using our CustomRequireClaim.cs
    options.AddPolicy("Claim.DoB", policyBuilder =>
    {
        // adding custom claim type
        //policyBuilder.AddRequirements(new CustomRequireClaim(ClaimTypes.DateOfBirth));

        // adding using our extension method (builder pattern)
        policyBuilder.RequireCustomClaim(ClaimTypes.DateOfBirth);
    });

});

// Authorization middleware (app.UseAuthorization)
// brings up all the auth policies (builder.Services.AddAuthorization)
// looks at the requirements of these policies
// => take requirement, try to find service (In DI container) to process this requirement.
// looks inside requirements to see if requests (to authorize) have succeeded
// if succeeded can proceed

builder.Services.AddScoped<IAuthorizationHandler, CustomRequireClaimHandler>();

// AddIdentity registers the services
builder.Services.AddIdentity<ApiUser, IdentityRole>(config =>
{
    config.Password.RequiredLength = 1;
    config.Password.RequireDigit = false;
    config.Password.RequireNonAlphanumeric = false;
    config.Password.RequireUppercase = false;
    //config.SignIn.RequireConfirmedEmail = true;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// cookie for AddIdentity
builder.Services.ConfigureApplicationCookie(options =>
{
    // Cookie settings
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

    options.Cookie.Name = "AuthIdentity.Cookie";
    options.LoginPath = "/Auth/Login";
    options.LogoutPath = "/Auth/Logout";

    //options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.SlidingExpiration = true;
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Who are you?
app.UseAuthentication();

// Are you allowed?
app.UseAuthorization();

app.MapControllers();

app.Run();
