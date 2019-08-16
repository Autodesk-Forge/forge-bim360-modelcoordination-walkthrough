using Newtonsoft.Json;
using System;

namespace MCSample.Model.Cosmo
{
    public class RevitClashReport
    {
        [JsonProperty(PropertyName = "container")]
        public Guid Container { get; set; }

        [JsonProperty(PropertyName = "modelSetId")]
        public Guid ModelSetId { get; set; }

        [JsonProperty(PropertyName = "version")]
        public int Version { get; set; }

        [JsonProperty(PropertyName = "test")]
        public Guid Test { get; set; }

        [JsonProperty(PropertyName = "testDate")]
        public DateTime TestDate { get; set; }

        [JsonProperty(PropertyName = "id")]
        public string ClashId { get; set; }

        [JsonProperty(PropertyName = "dist")]
        public double Distance { get; set; }

        [JsonProperty(PropertyName = "status")]
        public ClashStatus Status { get; set; }

        [JsonProperty(PropertyName = "ldoc")]
        public string LeftDocument { get; set; }

        [JsonProperty(PropertyName = "loid")]
        public int LeftSid { get; set; }

        [JsonProperty(PropertyName = "lvid")]
        public int LeftLmvId { get; set; }

        [JsonProperty(PropertyName = "lname")]
        public string LeftName { get; set; }

        [JsonProperty(PropertyName = "lcat")]
        public string LeftCategory { get; set; }

        [JsonProperty(PropertyName = "lfam")]
        public string LeftFamily { get; set; }

        [JsonProperty(PropertyName = "ltyp")]
        public string LeftType { get; set; }

        [JsonProperty(PropertyName = "rdoc")]
        public string RightDocument { get; set; }

        [JsonProperty(PropertyName = "roid")]
        public int RightSid { get; set; }

        [JsonProperty(PropertyName = "rvid")]
        public int RightLmvId { get; set; }

        [JsonProperty(PropertyName = "rname")]
        public string RightName { get; set; }

        [JsonProperty(PropertyName = "rcat")]
        public string RightCategory { get; set; }

        [JsonProperty(PropertyName = "rfam")]
        public string RightFamily { get; set; }

        [JsonProperty(PropertyName = "rtyp")]
        public string RightType { get; set; }

    }
}
