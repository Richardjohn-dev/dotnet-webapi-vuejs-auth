using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RoleBasedIdentityAuthentication.API.Models.Dtos;
using System.Security.Claims;

namespace RoleBasedIdentityAuthentication.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = Constants.Roles.Administrator)]
public class UserController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;

    public UserController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpPost("user")]
    //[Authorize(Roles = Constants.Roles.Administrator)]
    public async Task<IActionResult> AddUser([FromQuery] UserRegisterDto dto)
    {
        var user = new IdentityUser
        {
            UserName = dto.Username,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        var claims = new List<Claim>
        {
            //new Claim(ClaimTypes.Name, dto.Username),
            new Claim(ClaimTypes.Role, Constants.Roles.User)
        };

        await _userManager.AddClaimsAsync(user, claims);


        //await _signInManager.PasswordSignInAsync(user, dto.Password, false, false);

        return Ok("registered");
    }
    //[HttpGet("users")]

    [AllowAnonymous]
    [HttpGet("claims")]
    public IActionResult Claims()
    {
        try
        {
            var claimsList = HttpContext.User.Claims
                .Where(c => c.Type.Contains("claims"))
                .Select(claim => new
                {
                    ClaimType = claim.Type,
                    Value = claim.Value
                });

            if (!claimsList.Any()) return Unauthorized(new { signedIn = false });

            return Ok(claimsList);
        }
        catch (Exception)
        {
            return Unauthorized();
        }
    }
}
