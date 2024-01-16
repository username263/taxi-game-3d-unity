using System.Text.Json.Serialization;

namespace TaxiGame3D.Server.Templates;

public class StageTemplate
{
    public string Id
    {
        get;
        set;
    }
    public double Distance
    {
        get;
        set;
    }
    public double FareRate
    {
        get;
        set;
    }
    public int MaxCollect
    {
        get;
        set;
    }
    [JsonIgnore]
    public long MaxCoin => (long)(Distance * FareRate);
}
