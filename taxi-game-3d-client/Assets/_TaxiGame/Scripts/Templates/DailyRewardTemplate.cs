using Newtonsoft.Json;
using UnityEngine;

namespace TaxiGame3D
{
    public enum DailyRewardType
    {
        Undefined = 0,
        Coin = 1,
        Car = 2
    }

    public class DailyRewardTemplate
    {
        public int Index
        {
            get;
            set;
        }
        public DailyRewardType Type
        {
            get;
            set;
        }
        [JsonProperty("Icon")]
        public string IconPath
        {
            get;
            set;
        }
        [JsonIgnore]
        public Sprite Icon => Resources.Load<Sprite>(IconPath);
        public int Amount
        {
            get;
            set;
        }
    }
}
