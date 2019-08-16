using Autodesk.Nucleus.Clash.Entities.V3;
using MCSample.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace MCSample
{
    [JsonObject]
    public class QueryClashTestResultsState
    {
        [JsonIgnore]
        public bool HasLatest => Latest != null;

        [JsonIgnore]
        public bool HasResources => ResourceCollection != null;

        [JsonProperty]
        public Guid Container { get; set; }

        [JsonProperty]
        public ClashTestSummary Latest { get; set; }

        [JsonIgnore]
        public ClashTestResourceCollection ResourceCollection { get; set; }

        [JsonIgnore]
        public Dictionary<string, FileInfo> LocalResourcePaths { get; } = new Dictionary<string, FileInfo>(StringComparer.OrdinalIgnoreCase);

        [JsonIgnore]
        public Dictionary<int, ClashDocument> DocumentIndex { get; set; }
    }
}
