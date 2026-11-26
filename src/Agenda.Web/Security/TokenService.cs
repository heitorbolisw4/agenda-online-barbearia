using System.Text;
using Agenda.Application.Interface;
using System.Security.Claims;
using Agenda.Domain;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace Agenda.Web.Security;

public class TokenService(IConfiguration config, IOptions<JwtSettings> jwtOptions) : ITokenService
{
    private readonly IConfiguration _config = config;
    private readonly JwtSettings _settings = jwtOptions.Value;
    public string GenerateToken(Profissional profissional)
    {
        var secretKey = _settings.Key;
        var audience = _settings.Audience;
        var issuer = _settings.Issuer;
        var expiration = _settings.Expiration;


        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, profissional.Id.ToString()),
            new(ClaimTypes.Name, profissional.Nome),
            new(ClaimTypes.Role, profissional.Papel.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(expiration),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);



    }
}