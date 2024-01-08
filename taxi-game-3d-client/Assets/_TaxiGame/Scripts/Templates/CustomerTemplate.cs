using Newtonsoft.Json;
using UnityEngine;

namespace TaxiGame3D
{
    public class CustomerTemplate
    {
        public string Id { get; set; }
        [JsonProperty("Icon")]
        public string IconPath { get; set; }
        [JsonIgnore]
        public Sprite Icon => Resources.Load<Sprite>(IconPath);
        [JsonProperty("Prefab")]
        public string PrefabPath { get; set; }
        [JsonIgnore]
        public GameObject Prefab => Resources.Load<GameObject>(PrefabPath);
    }
}
