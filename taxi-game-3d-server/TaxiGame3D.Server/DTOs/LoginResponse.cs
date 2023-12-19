namespace TaxiGame3D.Server.DTOs;

public class LoginResponse
{
    public string Token { get; set; }
    public DateTime ExpireUtc { get; set; }
}
