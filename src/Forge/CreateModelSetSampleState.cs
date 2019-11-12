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
using Autodesk.Forge.Bim360.ModelCoordination.ModelSet;
using Sample.Forge.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sample.Forge
{
    public class CreateModelSetSampleState
    {
        public const string MC_SAMPLE_FOLDER_NAME = "MC_SAMPLE";

        public ForgeEntity PlansFolder { get; set; }

        public ForgeEntity TestFolderRoot { get; set; }

        public ForgeEntity TestFolder { get; set; }

        public List<ForgeUpload> Uploads { get; set; } = new List<ForgeUpload>();

        public ModelSet ModelSet { get; set; }
    }
}
