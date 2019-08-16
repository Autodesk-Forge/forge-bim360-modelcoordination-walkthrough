using Newtonsoft.Json;

namespace MCSample.Model
{
    [JsonObject]
    public class Clash
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "clash")]
        public int[] Objects { get; set; }

        [JsonProperty(PropertyName = "dist")]
        public double Distance { get; set; }

        [JsonProperty(PropertyName = "status")]
        public ClashStatus Status { get; set; }
    }
}
