using Newtonsoft.Json;

namespace TaxiGame3D
{
    public class StageTemplate
    {
        public string Id { get; set; }
        public double Distance { get; set; }
        public double FareRate { get; set; }
        [JsonProperty("Scene")]
        public string SceneName { get; set; }
    }
}