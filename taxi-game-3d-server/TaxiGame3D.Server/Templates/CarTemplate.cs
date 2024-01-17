using System.Text.Json;
using System.Text.Json.Serialization;
using TaxiGame3D.Server.Converters;

namespace TaxiGame3D.Server.Templates;

public class CarTemplate
{
    public string Id
    {
        get;
        set;
    }
    public int Cost
    {
        get;
        set;
    }
    [JsonConverter(typeof(BooleanConverter))]
    public bool EnableReward
    {
        get;
        set;
    }
}
