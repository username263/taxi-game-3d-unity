using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Localization;

namespace TaxiGame3D
{
    public class CarTemplate
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
        public LocalizationTemplate Name
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
        [JsonProperty("PlayerPrefab")]
        public string PlayerPrefabPath
        {
            get;
            set;
        }
        [JsonIgnore]
        public GameObject PlayerPrefab => Resources.Load<GameObject>(PlayerPrefabPath);
        [JsonProperty("UiPrefab")]
        public string UiPrefabPath
        {
            get;
            set;
        }
        [JsonIgnore]
        public GameObject UiPrefab => Resources.Load<GameObject>(UiPrefabPath);
        public int Cost
        {
            get;
            set;
        }
    }
}