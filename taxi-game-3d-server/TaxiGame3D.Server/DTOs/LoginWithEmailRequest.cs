namespace TaxiGame3D.Server.DTOs;

public class LoginWithEmailRequest
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}
