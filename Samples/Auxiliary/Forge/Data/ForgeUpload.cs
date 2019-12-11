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
using System.IO;

namespace Sample.Forge.Data
{
    [JsonObject]
    public class ForgeUpload
    {
        private FileInfo _file;

        [JsonIgnore]
        public FileInfo File
        {
            get
            {
                return _file;
            }

            set
            {
                _file = value;

                Path = _file.FullName;
            }
        }

        [JsonProperty]
        public uint Version { get; set; }

        [JsonProperty]
        public string Path { get; set; }

        [JsonProperty]
        public ForgeEntity Storage { get; set; }

        [JsonProperty]
        public UploadResult Result { get; set; }
    }
}
