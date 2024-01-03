using Newtonsoft.Json;

namespace TaxiGame3D
{
    public class EndStageRequest
    {
        [JsonProperty("Stage")]
        public int StageIndex { get; set; }
        public bool IsGoal { get; set; }
        public long Coin { get; set; }
    }
}