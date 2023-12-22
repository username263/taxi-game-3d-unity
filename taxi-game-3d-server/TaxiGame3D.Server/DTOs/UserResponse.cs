namespace TaxiGame3D.Server.DTOs;

public class UserResponse
{
    public string? Nickname { get; set; }
    public long Coin { get; set; }
    public List<string>? Cars { get; set; }
    public string? CurrentCar { get; set; }
    public int CurrentStage { get; set; }
}
