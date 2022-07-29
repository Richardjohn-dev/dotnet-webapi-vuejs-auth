using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RoleBasedIdentityAuthentication.API.Models.Dtos;

namespace RoleBasedIdentityAuthentication.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;

    public AuthController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromQuery] UserLoginDto dto)
    {
        var signInResult = await _signInManager.PasswordSignInAsync(dto.Username, dto.Password, false, false);
        return (HandleSignInResult(signInResult));
    }

    private IActionResult HandleSignInResult(Microsoft.AspNetCore.Identity.SignInResult signInResult)
    {
        if (signInResult.Succeeded) return Ok("signed in");

        var failedReason = "Credentials invalid.";

        if (signInResult.IsNotAllowed)
        {
            failedReason = "Email not confirmed.";
        }
        else if (signInResult.IsLockedOut)
        {
            failedReason = "Your account is locked.";
        }
        else if (signInResult.RequiresTwoFactor)
        {
            failedReason = "Requires two factor authorization";
        }

        return Unauthorized(failedReason);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> LogOut()
    {
        await _signInManager.SignOutAsync();
        // is it okay to leave a cookie around?
        return Ok("signed out");
    }

    [HttpGet("bothroles")]
    [Authorize(Policy = Constants.Policies.UserAccess)]
    public IActionResult BothRoles()
    {
        return Ok(new
        {
            message = "customers and admins welcome"
        });
    }

    [HttpGet("admins")]
    [Authorize(Roles = Constants.Roles.Administrator)]
    public IActionResult SecretRole()
    {
        return Ok(new
        {
            message = "admins only super secret info"
        });
    }

}
