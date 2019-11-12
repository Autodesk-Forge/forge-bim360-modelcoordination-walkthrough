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
using Newtonsoft.Json;

namespace Sample.Forge.Coordination
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
