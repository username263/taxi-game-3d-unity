using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace TaxiGame3D.Server.Settings;

public class JwtSettings
{
    public required string Issuer { get; set; }
    public required string Audience { get; set; }
    public required string Key { get; set; }
    public double Lifetime { get; set; }

    public SymmetricSecurityKey GenerateKey() =>
        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));

    public static implicit operator TokenValidationParameters(JwtSettings settings) => new()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidIssuer = settings.Issuer,
        ValidAudience = settings.Audience,
        IssuerSigningKey = settings.GenerateKey()
    };
}
