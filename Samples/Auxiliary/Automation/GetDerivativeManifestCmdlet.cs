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
using Newtonsoft.Json;
using Sample.Forge;
using Sample.Forge.Data;
using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Text;
using System.Text.RegularExpressions;

namespace Forge.Automation
{
    [Cmdlet(VerbsCommon.Get, "DerivativeManifest")]
    [OutputType(typeof(SampleConfiguration))]
    public class GetDerivativeManifestCmdlet : ForgeCmdlet
    {
        private readonly static Regex IsBase64Encoded = new Regex(@"^[a-zA-Z0-9\+/]+={0,3}$", RegexOptions.Compiled);

        [Parameter(
            Mandatory = true,
            Position = 0,
            ValueFromPipelineByPropertyName = true)]
        public string Manifest { get; set; }

        protected override void ProcessRecord()
        {
            string urn = IsBase64Encoded.IsMatch(Manifest) ? Manifest : Convert.ToBase64String(Encoding.UTF8.GetBytes(Manifest));

            var client = ServiceProvider.GetRequiredService<IForgeDerivativeClient>();

            var manifestTask = client.GetDerivativeManifest(urn);

            manifestTask.Wait();

            WriteObject(JsonConvert.SerializeObject(manifestTask.Result, Formatting.Indented));
        }
    }
}
