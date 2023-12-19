using System.Text.Json.Serialization;

namespace TaxiGame3D.Server.DTOs;

public class EndStageRequest
{
    [JsonPropertyName("Stage")]
    public int StageIndex { get; set; }
    public int Coin { get; set; }
}
