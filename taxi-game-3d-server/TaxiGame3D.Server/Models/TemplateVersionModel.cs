using MongoDB.Bson.Serialization.Attributes;

namespace TaxiGame3D.Server.Models;

public class TemplateVersionModel
{
    [BsonId]
    public string? Name { get; set; }
    public ulong Version { get; set; }
}
