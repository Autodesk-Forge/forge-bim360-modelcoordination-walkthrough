using Newtonsoft.Json;
using System.Collections.Generic;

namespace MCSample.Model.Cosmo
{
    /*
     * {
     *      "file":1,
     *      "db":1,
     *      "id":3483,
     *      "viewable_in":"0d02ae48-482f-497f-a6e9-773e28c5fab8-0032f400",
     *      "name":"Basic Wall [899614]",
     *      "_RC":"Walls",
     *      "_RFN":"Basic Wall",
     *      "_RFT":"Exterior - Wood Panel on Metal Stud"
     * }
     */
    public class RevitObject
    {
        [JsonProperty(PropertyName = "file")]
        public string File { get; set; }

        [JsonProperty(PropertyName = "db")]
        public string DbIndex { get; set; }

        [JsonProperty(PropertyName = "docs")]
        public string[] DocumentIds { get; set; }

        [JsonProperty(PropertyName = "id")]
        public int ObjectId { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "cat")]
        public string Category { get; set; }

        [JsonProperty(PropertyName = "fam")]
        public string Family { get; set; }

        [JsonProperty(PropertyName = "typ")]
        public string Type { get; set; }

        [JsonIgnore]
        public IReadOnlyDictionary<string, string> ViewableMap { get; set; }
    }
}
