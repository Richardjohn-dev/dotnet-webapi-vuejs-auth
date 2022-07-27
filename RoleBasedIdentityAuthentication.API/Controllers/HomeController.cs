using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RoleBasedIdentityAuthentication.API.Data;

namespace RoleBasedIdentityAuthentication.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class HomeController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly ApplicationDbContext _applicationDbContext;

    public HomeController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, ApplicationDbContext applicationDbContext)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _applicationDbContext = applicationDbContext;
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login(string username, string password)
    {
        //await _signInManager.SignOutAsync();
        var user = await _userManager.FindByNameAsync(username);
        if (user != null)
        {
            // sign in
            var signInResult = await _signInManager.PasswordSignInAsync(user, password, false, false);
            if (signInResult.Succeeded)
            {

                return Ok("signed in");
            }
        }

        return BadRequest();
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(string username, string password, string email)
    {
        var user = new IdentityUser
        {
            UserName = username,
            Email = email,
        };

        var result = await _userManager.CreateAsync(user, password);

        await _userManager.AddToRoleAsync(user, "User");

        if (!result.Succeeded)
            return BadRequest(result.Errors);


        var signInResult = await _signInManager.PasswordSignInAsync(user, password, false, false);
        if (signInResult.Succeeded)
        {
            return Ok("signed in");
        }

        return Ok(new
        {
            message = "registered"
        });
    }


    [HttpPost("logout")]
    public async Task<IActionResult> LogOut()
    {
        await _signInManager.SignOutAsync();
        // does this remove the cookie?
        // what are the security risks here?
        return Ok(new
        {
            message = "signed out"
        });
    }


    [HttpPost("bothroles")]
    [Authorize(Roles = "User, Administrator")]
    public IActionResult BothRoles()
    {
        return Ok(new
        {
            message = "customers and admins welcome"
        });
    }

    [HttpPost("admins")]
    [Authorize(Roles = "Administrator")]
    public IActionResult SecretRole()
    {
        return Ok(new
        {
            message = "admins only super secret info"
        });
    }

    [HttpGet("session-user")]
    public async Task<IActionResult> SessionUser()
    {
        try
        {
            var name = HttpContext.User.Identity.Name;
            var user = await _userManager.FindByNameAsync(name);
            return Ok(new { Roles = await _userManager.GetRolesAsync(user) });

        }
        catch (Exception)
        {
            return Unauthorized();
        }
    }
}
