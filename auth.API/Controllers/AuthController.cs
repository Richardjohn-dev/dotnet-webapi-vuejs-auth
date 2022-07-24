using Auth.API.Data;
using Auth.API.Helpers;
using Auth.API.Models;
using Auth.API.Models.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace JewExample.Api.Controllers;

[Route("api")]
[ApiController]
public class AuthController : Controller
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;

    public AuthController(IUserRepository userRepository, IJwtService jwtService)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(UserRegisterDto userRegisterDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var user = new User
        {
            Email = userRegisterDto.Email,
            Name = userRegisterDto.Name,
            Password = BCrypt.Net.BCrypt.HashPassword(userRegisterDto.Password)
        };
        return Created("Success", await _userRepository.CreateAsync(user));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(UserLoginDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var user = await _userRepository.GetByEmail(dto.Email);

        if (user == null) return BadRequest(new { message = "Invalid Credentials" });

        if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
        {
            return BadRequest(new { message = "Invalid Credentials" });
        }

        var jwt = _jwtService.Generate(user.Id);

        // 
        Response.Cookies.Append("jwt", jwt, new CookieOptions
        {
            HttpOnly = true
        });

        return Ok(new
        {
            message = "success"
        });
    }


    [HttpGet("session-user")]
    public async Task<IActionResult> SessionUser()
    {
        try
        {
            var jwt = Request.Cookies["jwt"];

            if (jwt is null) return Unauthorized();

            var token = _jwtService.Verify(jwt);

            int userId = int.Parse(token.Issuer);

            var user = await _userRepository.GetById(userId);

            return Ok(user);
        }
        catch (Exception)
        {
            return Unauthorized();
        }
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("jwt");

        return Ok(new
        {
            message = "success"
        });
    }
}
