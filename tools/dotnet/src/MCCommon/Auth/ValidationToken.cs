using Newtonsoft.Json;

namespace MCCommon.Auth
{
    [JsonObject]
    public class ValidationToken
    {
        [JsonProperty(PropertyName = "scope")]
        public string Scope { get; set; }

        [JsonProperty(PropertyName = "token_type")]
        public string TokenType { get; set; }

        [JsonProperty(PropertyName = "expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty(PropertyName = "client_id")]
        public string ClientId { get; set; }

        [JsonProperty(PropertyName = "access_token")]
        public TokenUser User { get; set; }
    }
}
