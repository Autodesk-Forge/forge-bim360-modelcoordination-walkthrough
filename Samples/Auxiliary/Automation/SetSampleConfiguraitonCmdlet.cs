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
using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Text;

namespace Forge.Automation
{
    [Cmdlet(VerbsCommon.Set, "SampleConfiguration")]
    [OutputType(typeof(SampleConfiguration))]
    public class SetSampleConfiguraitonCmdlet : ForgeCmdlet
    {
        [Parameter(
            Mandatory = false,
            Position = 0,
            ValueFromPipelineByPropertyName = true)]
        public string ClientId { get; set; }

        [Parameter(
            Mandatory = false,
            Position = 1,
            ValueFromPipelineByPropertyName = true)]
        public string Secret { get; set; }

        [Parameter(
            Mandatory = false,
            Position = 2,
            ValueFromPipelineByPropertyName = true)]
        public string AuthToken { get; set; }

        [Parameter(
            Mandatory = false,
            Position = 3,
            ValueFromPipelineByPropertyName = true)]
        public Uri CallbackUrl { get; set; }

        [Parameter(
           Mandatory = false,
           Position = 4,
           ValueFromPipelineByPropertyName = true)]
        public Guid AccountId { get; set; }

        [Parameter(
           Mandatory = false,
           Position = 5,
           ValueFromPipelineByPropertyName = true)]
        public Guid ProjectId { get; set; }

        [Parameter(
           Mandatory = false,
           Position = 6,
           ValueFromPipelineByPropertyName = true)]
        public SwitchParameter Save { get; set; }

        protected override void ProcessRecord()
        {
            Configuration.AuthToken = !string.IsNullOrWhiteSpace(AuthToken) ? AuthToken : Configuration.AuthToken;
            Configuration.ClientId = !string.IsNullOrWhiteSpace(ClientId) ? ClientId : Configuration.ClientId;
            Configuration.Secret = !string.IsNullOrWhiteSpace(Secret) ? Secret : Configuration.Secret;
            Configuration.CallbackUrl = CallbackUrl != null ? CallbackUrl.AbsoluteUri : Configuration.CallbackUrl;
            Configuration.AccountId = AccountId != Guid.Empty ? AccountId : Configuration.AccountId;
            Configuration.ProjectId = ProjectId != Guid.Empty ? ProjectId : Configuration.ProjectId;

            if (Save.IsPresent)
            {
                var fileManager = ServiceProvider.GetRequiredService<ILocalFileManager>();

                fileManager.WriteJson(Configuration);
            }

            WriteObject(Configuration);
        }
    }
}
