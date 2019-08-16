using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MCAuth.Auth
{
    [JsonObject]
    internal class Token
    {
        [JsonProperty("token_type")]
        public string Type { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty("expires_on")]
        public DateTime ExpiresOn { get; set; }

        [JsonProperty("refresh_token")]
        public string Refresh { get; set; }

        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

    }
}
