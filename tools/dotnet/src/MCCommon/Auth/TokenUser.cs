using Newtonsoft.Json;

namespace MCCommon.Auth
{
    [JsonObject]
    public class TokenUser
    {
        [JsonProperty(PropertyName = "userid")]
        public string UserId { get; set; }

        [JsonProperty(PropertyName = "username")]
        public string UserName { get; set; }

        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "firstname")]
        public string FirstName { get; set; }

        [JsonProperty(PropertyName = "lastname")]
        public string LastName { get; set; }
    }
}
