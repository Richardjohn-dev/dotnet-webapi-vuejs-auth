using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RoleBasedAuthIdentity.API.Data;
using RoleBasedAuthIdentity.API.Models;

namespace RoleBasedAuthIdentity.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class HomeController : ControllerBase
{
    private readonly UserManager<ApiUser> _userManager;
    private readonly SignInManager<ApiUser> _signInManager;
    private readonly ApplicationDbContext _applicationDbContext;

    public HomeController(UserManager<ApiUser> userManager, SignInManager<ApiUser> signInManager, ApplicationDbContext applicationDbContext)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _applicationDbContext = applicationDbContext;
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login(string username, string password)
    {
        await _signInManager.SignOutAsync();
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
        var user = new ApiUser
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

    // Think of this as a tag/attribute that we can give more information to.
    // Authorization Tri-fecta
    // Authorization Policy = Authorization Requirements => Processed By Authorization Handlers

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

    //////[AllowAnonymous]
    //[HttpPost]
    //public IActionResult Authenticate()
    //{
    //    var grandmaClaims = new List<Claim>()
    //    {
    //        new Claim(ClaimTypes.Name, "Bob"),
    //        new Claim(ClaimTypes.Email, "Bob@fmail.com"),
    //        new Claim("Grandma.Says", "Very nice boi."),
    //        //new Claim(ClaimTypes.DateOfBirth, "11/11/2000"),
    //        //new Claim(ClaimTypes.Role, "Admin"),
    //        //new Claim(ClaimTypes.Role, "AdminTwo"),
    //        //new Claim(DynamicPolicies.SecurityLevel, "7"),
    //    };

    //    var licenseClaims = new List<Claim>()
    //    {
    //        new Claim(ClaimTypes.Name, "Bob K Foo"),
    //        new Claim("DrivingLicense", "A+"),
    //    };

    //    // we trust These identities
    //    // Can have many identities ((FB,Twitter,Linked...)
    //    var grandmaIdentity = new ClaimsIdentity(grandmaClaims, "Grandma Identity");
    //    var licenseIdentity = new ClaimsIdentity(licenseClaims, "Government");

    //    var userPrincipal = new ClaimsPrincipal(new[] { grandmaIdentity, licenseIdentity });
    //    ////-----------------------------------------------------------
    //    HttpContext.SignInAsync(userPrincipal);

    //    return Ok(new
    //    {
    //        message = "Authenticated"
    //    });
    //}

    //public IActionResult Index()
    //{
    //    return View();
    //}

    //[Authorize]
    //public IActionResult Secret()
    //{
    //    return View();
    //}

    //[Authorize(Policy = "Claim.DoB")]
    //public IActionResult SecretPolicy()
    //{
    //    return View("Secret");
    //}

    //[Authorize(Roles = "Admin")]
    //public IActionResult SecretRole()
    //{
    //    return View("Secret");
    //}

    //[SecurityLevel(5)]
    //public IActionResult SecretLevel()
    //{
    //    return View("Secret");
    //}

    //[SecurityLevel(10)]
    //public IActionResult SecretHigherLevel()
    //{
    //    return View("Secret");
    //}



    //public async Task<IActionResult> DoStuff(
    //    [FromServices] IAuthorizationService authorizationService)
    //{
    //    // we are doing stuff here

    //    var builder = new AuthorizationPolicyBuilder("Schema");
    //    var customPolicy = builder.RequireClaim("Hello").Build();

    //    var authResult = await authorizationService.AuthorizeAsync(User, customPolicy);

    //    if (authResult.Succeeded)
    //    {
    //        return View("Index");
    //    }

    //    return View("Index");
    //}
}
