using Newtonsoft.Json;
using UnityEngine;

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
        [JsonProperty("Prefab")]
        public string PrefabPath
        {
            get;
            set;
        }
        [JsonIgnore]
        public GameObject Prefab => Resources.Load<GameObject>(PrefabPath);
        public int Cost
        {
            get;
            set;
        }
    }
}