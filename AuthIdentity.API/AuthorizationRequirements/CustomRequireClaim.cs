using Microsoft.AspNetCore.Authorization;

namespace RoleBasedAuthIdentity.API.AuthorizationRequirements;


// Requirement = Request (to the Authorization Middleware) get AUthorizaed
// 'Do you have this Claim Type?'
public class CustomRequireClaim : IAuthorizationRequirement
{
    public CustomRequireClaim(string claimType)
    {
        ClaimType = claimType;
    }
    public string ClaimType { get; }
}

// Authorized by the Handler
public class CustomRequireClaimHandler : AuthorizationHandler<CustomRequireClaim>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CustomRequireClaim requirement)
    {
        var hasClaim = context.User.Claims.Any(x => x.Type == requirement.ClaimType);
        if (hasClaim)
        {
            context.Succeed(requirement);
        }
        return Task.CompletedTask;
    }
}

// custom extension method
public static class AuthorizationPolocyBuilderExtensions
{
    public static AuthorizationPolicyBuilder RequireCustomClaim(
        this AuthorizationPolicyBuilder builder,
        string claimType)
    {
        builder.AddRequirements(new CustomRequireClaim(claimType));
        return builder;
    }
}