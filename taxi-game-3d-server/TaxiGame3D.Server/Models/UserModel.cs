using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace TaxiGame3D.Server.Models;

public class UserModel
{
    [BsonId]
    public string? Id { get; set; }
    public string? Nickname { get; set; }
    [BsonElement("Device")]
    [JsonPropertyName("Device")]
    public string? DeviceId { get; set; }
    public long Coin { get; set; }
    public List<string>? Cars { get; set; }
    [BsonElement("CurrentCar")]
    [JsonPropertyName("CurrentCar")]
    public string? CurrentCarId { get; set; }
    [BsonElement("CurrentStage")]
    [JsonPropertyName("CurrentStage")]
    public int CurrentStageIndex { get; set; }
}
