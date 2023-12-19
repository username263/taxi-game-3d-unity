using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TaxiGame3D.Server.Models;
using TaxiGame3D.Server.Settings;

namespace TaxiGame3D.Server.Services;

public class TokenService
{
    readonly JwtSettings jwtSettings;

    public TokenService(IOptions<JwtSettings> options)
    {
        jwtSettings = options.Value;
    }

    public (string token, DateTime expireUtc) Generate(UserModel user)
    {
        var securityKey = jwtSettings.GenerateKey();
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var claims = new Claim[]
        {
            new(JwtRegisteredClaimNames.Sub, user.Id!),
            new(JwtRegisteredClaimNames.Email, user.Email)
        };
        var expires = DateTime.Now.AddSeconds(jwtSettings.Lifetime);
        var token = new JwtSecurityToken(
            jwtSettings.Issuer,
            jwtSettings.Audience,
            claims,
            expires: expires,
            signingCredentials: credentials
        );
        return (
            new JwtSecurityTokenHandler().WriteToken(token),
            expires.ToUniversalTime()
        );
    }

    /*public string Refresh(IHeaderDictionary headers)
    {
        var authHeader = headers["Authorization"].FirstOrDefault();
        var token = new JwtSecurityTokenHandler().ReadJwtToken(
            authHeader?.Substring("Bearer ".Length)
        );
        
    }*/
}
