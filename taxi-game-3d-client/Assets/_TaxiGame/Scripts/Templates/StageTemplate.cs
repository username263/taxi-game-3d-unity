using Newtonsoft.Json;

namespace TaxiGame3D
{
    public class StageTemplate
    {
        [JsonIgnore]
        public int Index
        {
            get;
            set;
        }
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
        [JsonProperty("Scene")]
        public string SceneName
        {
            get;
            set;
        }

        [JsonIgnore]
        public long MaxCoin => CalcCoin(Distance);

        public long CalcCoin(double diatance) => (long)(diatance * FareRate);
    }
}