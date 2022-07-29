using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace RoleBasedIdentityAuthentication.API.Authentication;

// custom middleware to ensure cookie is removed after set period and not indefinitely extended
//https://brokul.dev/authentication-cookie-lifetime-and-sliding-expiration

public class CustomCookieAuthenticationEvents : CookieAuthenticationEvents
{
    private const string _ticketIssuedTicks = nameof(_ticketIssuedTicks);

    public override async Task SigningIn(CookieSigningInContext context)
    {
        context.Properties.SetString(
            _ticketIssuedTicks,
            DateTimeOffset.UtcNow.Ticks.ToString());

        await base.SigningIn(context);
    }

    public override async Task ValidatePrincipal(
        CookieValidatePrincipalContext context)
    {
        var ticketIssuedTicksValue = context
            .Properties.GetString(_ticketIssuedTicks);

        if (ticketIssuedTicksValue is null ||
            !long.TryParse(ticketIssuedTicksValue, out var ticketIssuedTicks))
        {
            await RejectPrincipalAsync(context);
            return;
        }

        var ticketIssuedUtc =
            new DateTimeOffset(ticketIssuedTicks, TimeSpan.FromHours(0));

        if (DateTimeOffset.UtcNow - ticketIssuedUtc > TimeSpan.FromHours(6))
        {
            await RejectPrincipalAsync(context);
            return;
        }

        await base.ValidatePrincipal(context);
    }

    private static async Task RejectPrincipalAsync(
        CookieValidatePrincipalContext context)
    {
        context.RejectPrincipal();
        await context.HttpContext.SignOutAsync();
    }
}
