using Newtonsoft.Json;
using System;

namespace MCSample.Forge
{
    [JsonObject]
    public class UploadResult
    {
        [JsonProperty(PropertyName = "objectId")]
        public string ObjectId { get; set; }

        [JsonProperty(PropertyName = "bucketKey")]
        public string BucketKey { get; set; }

        [JsonProperty(PropertyName = "objectKey")]
        public string ObjectKey { get; set; }

        [JsonProperty(PropertyName = "size")]
        public int Size { get; set; }

        [JsonProperty(PropertyName = "contentType")]
        public string ContentType { get; set; }

        [JsonProperty(PropertyName = "location")]
        public Uri Location { get; set; }
    }
}
