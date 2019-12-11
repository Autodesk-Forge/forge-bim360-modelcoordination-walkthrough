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
using AuthWeb;
using Sample.Forge;
using System;

namespace AuthWeb.Data
{
    public class ForgeAppModel
    {
        public SampleConfiguration Binding { get; set; }

        public string Account
        {
            get => Binding.AccountId.ToString();
            set => Binding.AccountId = Guid.Parse(value);
        }

        public string Project
        {
            get => Binding.ProjectId.ToString();
            set => Binding.ProjectId = Guid.Parse(value);
        }
    }
}
