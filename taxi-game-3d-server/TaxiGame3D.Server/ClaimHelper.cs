using System.Security.Claims;

namespace TaxiGame3D.Server;

public static class ClaimHelper
{
    public static string? FindNameIdentifier(ClaimsPrincipal principal) =>
        FindNameIdentifier(principal.Claims);

    public static string? FindNameIdentifier(IEnumerable<Claim> claims) =>
        claims.FirstOrDefault(e => e.Type == ClaimTypes.NameIdentifier)?.Value;
}
