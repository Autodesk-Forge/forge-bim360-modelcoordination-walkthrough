﻿/////////////////////////////////////////////////////////////////////
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
using Newtonsoft.Json.Linq;

namespace Sample.Forge.Coordination
{
    public class IndexRow
    {
        public IndexRow(JObject rowObject) => Data = rowObject;

        public JObject Data { get; }

        public string FileId => (string)Data["file"];

        public string DatabaseId => (string)Data["db"];

        public string[] DocumentIds => Data["docs"].ToObject<string[]>();

        public int Id => (int)Data["id"];

        public string Checksum => (string)Data["checksum"];
    }
}
