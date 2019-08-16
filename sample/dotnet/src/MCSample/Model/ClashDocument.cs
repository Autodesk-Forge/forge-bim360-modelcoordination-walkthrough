using Newtonsoft.Json;

namespace MCSample.Model
{
    [JsonObject]
    public class ClashDocument
    {
        [JsonProperty(PropertyName = "id")]
        public int Index { get; set; }

        [JsonProperty(PropertyName = "urn")]
        public string Urn { get; set; }
    }
}
