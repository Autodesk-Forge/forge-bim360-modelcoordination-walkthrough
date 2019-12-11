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
using System;
using System.Collections.Generic;
using System.Text;

namespace Sample.Forge.Coordination
{
    public class IndexDocumentObjects
    {
        public string SeedFileVersionUrn { get; set; }

        public string DocumentVersionUrn { get; set; }

        public string IndexDocumentKey { get; set; }

        public string IndexFileKey { get; set; }

        public List<int> Objects { get; } = new List<int>();

        public void AddObjectId(int id)
        {
            if (!Objects.Contains(id))
            {
                Objects.Add(id);
            }
        }
    }
}
