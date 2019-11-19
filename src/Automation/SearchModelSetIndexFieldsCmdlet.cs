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
using Microsoft.Extensions.DependencyInjection;
using Sample.Forge;
using Sample.Forge.Coordination;
using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Text;

namespace Forge.Automation
{
    [Cmdlet(VerbsCommon.Search, "ModelSetIndexFields")]
    [OutputType(typeof(IndexField[]))]
    public class SearchModelSetIndexFieldsCmdlet : ModelSetVersionCmdlet
    {
        [Parameter(
            Mandatory = false,
            Position = 3,
            ValueFromPipelineByPropertyName = true)]
        public string SearchText { get; set; }

        protected override void ProcessRecord()
        {
            var indexClient = ServiceProvider.GetRequiredService<IModelSetIndex>();

            WriteObjectAsync(indexClient.SearchFields(Container, ModelSet, Version, SearchText));
        }
    }
}
