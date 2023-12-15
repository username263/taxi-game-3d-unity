using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TaxiGame3D.Server.Models;

public class TemplateModel
{
    [BsonId]
    public string? Name { get; set; }
    public BsonArray? Datas { get; set; }
}
