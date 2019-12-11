/////////////////////////////////////////////////////////////////////
// Copyright (c) Autodesk, Inc. All rights reserved
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
/////////////////////////////////////////////////////////////////////
using Autodesk.Forge.Bim360.ModelCoordination.Clash;
using Newtonsoft.Json;
using Sample.Forge.Coordination;
using System;
using System.Collections.Generic;
using System.IO;

namespace Sample.Forge
{
    [JsonObject]
    public class GetClashResultsSampleState
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
