using Newtonsoft.Json;
using System;

namespace TaxiGame3D
{
    public class LoginResponse
    {
        public string Token { get; set; }
        public DateTime ExpireUtc { get; set; }

        [JsonIgnore]
        public string BearerToken => $"Bearer {Token}";
    }
}