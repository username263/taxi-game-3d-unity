using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TaxiGame3D
{
    public class StageTamplate
    {
        public string Id { get; set; }
        [JsonProperty("Scene")]
        public string SceneName { get; set; }
    }
}