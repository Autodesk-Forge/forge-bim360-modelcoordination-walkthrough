using Newtonsoft.Json;

namespace MCSample.Model
{
    [JsonObject]
    public class IndexField
    {
        [JsonProperty(PropertyName = "key")]
        public string Key { get; set; }

        [JsonProperty(PropertyName = "category")]
        public string Category { get; set; }

        [JsonProperty(PropertyName = "type")]
        public IndexFieldType Type { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "uom")]
        public string Uom { get; set; }
    }
}
