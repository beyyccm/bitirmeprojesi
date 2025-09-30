using Microsoft.AspNetCore.Mvc;
using UcakRezervasyon.Entities.Models;
using UcakRezervasyon.Business.Services;

namespace UcakRezervasyon.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IAuthService _authService;

    public AuthController(IUserService userService, IAuthService authService)
    {
        _userService = userService;
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var exists = await _userService.GetByEmailAsync(dto.Email);
        if (exists != null) return BadRequest(new { message = "Email already in use" });

        var user = new User {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            Role = dto.Role
        };
        await _userService.CreateAsync(user, dto.Password);
        return Ok(new { message = "Registered" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var valid = await _authService.ValidateCredentialsAsync(dto.Email, dto.Password);
        if (!valid) return Unauthorized();
        var user = await _userService.GetByEmailAsync(dto.Email);
        var token = await _authService.GenerateTokenAsync(user!);
        return Ok(new { token });
    }
}

public record RegisterDto(string FirstName, string LastName, string Email, string PhoneNumber, string Password, UcakRezervasyon.Entities.Models.Role Role);
public record LoginDto(string Email, string Password);
