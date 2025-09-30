using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.DataProtection;
using UcakRezervasyon.Entities.Models;
using UcakRezervasyon.DataAccess.Repositories;

namespace UcakRezervasyon.Business.Services;

public class AuthService : IAuthService
{
    private readonly IConfiguration _config;
    private readonly IUnitOfWork _uow;
    private readonly IDataProtector _protector;

    public AuthService(IConfiguration config, IUnitOfWork uow, IDataProtectionProvider provider)
    {
        _config = config;
        _uow = uow;
        _protector = provider.CreateProtector("UserPasswordsProtector");
    }

    public async Task<string> GenerateTokenAsync(User user)
    {
        var jwt = _config.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: jwt["Issuer"],
            audience: jwt["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(double.Parse(jwt["ExpiresMinutes"])),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<bool> ValidateCredentialsAsync(string email, string password)
    {
        var user = (await _uow.Users.FindAsync(u => u.Email.ToLower() == email.ToLower())).FirstOrDefault();
        if (user == null) return false;
        var plain = _protector.Unprotect(user.Password);
        return plain == password;
    }
}
