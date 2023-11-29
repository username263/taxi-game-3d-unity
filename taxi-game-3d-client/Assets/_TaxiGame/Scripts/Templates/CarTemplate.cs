using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TaxiGame3D
{
    public class CarTemplate
    {
        public string Id { get; set; }
        public LocalizationTemplate Name { get; set; }
        [JsonProperty("Icon")]
        public string IconPath { get; set; }
        [JsonProperty("Prefab")]
        public string PrefabPath { get; set; }
        public int Cost { get; set; }
    }
}