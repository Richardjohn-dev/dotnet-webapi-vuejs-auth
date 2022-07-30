using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RoleBasedIdentityAuthentication.API.Models;
using RoleBasedIdentityAuthentication.API.Models.Dtos;

namespace RoleBasedIdentityAuthentication.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public AuthController(UserManager<User> userManager, SignInManager<User> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpPost("login")]
    [AllowAnonymous]
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
    [AllowAnonymous]
    public async Task<IActionResult> LogOut()
    {
        await _signInManager.SignOutAsync();
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
