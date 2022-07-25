using AuthIdentity.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AuthIdentity.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class HomeController : ControllerBase
{
    private readonly UserManager<ApiUser> _userManager;
    private readonly SignInManager<ApiUser> _signInManager;

    public HomeController(UserManager<ApiUser> userManager, SignInManager<ApiUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login(string username, string password)
    {
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
        return Ok(new
        {

            message = "signed out"
        });
    }

    [Authorize]
    [HttpPost("secret")]
    public IActionResult Secret()
    {
        return Ok(new
        {
            message = "super secret info"
        });
    }

    ////[AllowAnonymous]
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
