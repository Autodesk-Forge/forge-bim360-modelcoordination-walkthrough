using Newtonsoft.Json;

namespace MCCommon
{
    [JsonObject]
    public class ForgeApp
    {
        [JsonIgnore]
        public ForgeEnvironment Environment { get; internal set; }

        [JsonProperty]
        public string ClientId { get; set; }

        [JsonProperty]
        public string Secret { get; set; }

        [JsonProperty]
        public string CallbackUrl { get; set; }

        [JsonProperty]
        public bool IsDefault { get; set; }
    }
}
