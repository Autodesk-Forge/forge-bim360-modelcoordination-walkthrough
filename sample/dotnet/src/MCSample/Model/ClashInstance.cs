using Newtonsoft.Json;

namespace MCSample.Model
{
    [JsonObject]
    public class ClashInstance
    {
        [JsonProperty(PropertyName = "cid")]
        public int ClashId { get; set; }

        [JsonProperty(PropertyName = "ldid")]
        public int LeftDocumentIndex { get; set; }

        [JsonProperty(PropertyName = "loid")]
        public int LeftStableObjectId { get; set; }

        [JsonProperty(PropertyName = "lvid")]
        public int LeftLmvObjectId { get; set; }

        [JsonProperty(PropertyName = "rdid")]
        public int RightDocumentIndex { get; set; }

        [JsonProperty(PropertyName = "roid")]
        public int RightStableObjectId { get; set; }

        [JsonProperty(PropertyName = "rvid")]
        public int RightLmvObjectId { get; set; }
    }
}
