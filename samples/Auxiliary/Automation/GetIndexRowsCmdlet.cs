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
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace Forge.Automation
{
    [Cmdlet(VerbsCommon.Get, "IndexRows")]
    [OutputType(typeof(IndexRow))]
    public class GetIndexRowsCmdlet : ForgeCmdlet
    {
        [Parameter(
            Mandatory = true,
            Position = 0,
            ValueFromPipelineByPropertyName = true)]
        public FileInfo Path { get; set; }

        [Parameter(
            Mandatory = false,
            Position = 1,
            ValueFromPipelineByPropertyName = true)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "Powershell dynamic binding")]
        public IndexField[] Fields { get; set; }

        protected override void ProcessRecord()
        {
            IReadOnlyDictionary<string, IndexField> fields = Fields != null ? new ReadOnlyDictionary<string, IndexField>(Fields.ToDictionary(f => f.Key, StringComparer.OrdinalIgnoreCase)) : null;

            var reader = new IndexResultReader(Path, fields);

            var rows = new List<IndexRow>();

            var task = reader.ReadToEndAsync(
                row =>
                {
                    rows.Add(row);

                    return Task.FromResult(true);
                }, 
                fields != null);

            task.Wait();

            rows.ForEach(r => WriteObject(r));
        }
    }
}
