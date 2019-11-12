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
using System.Threading.Tasks;

namespace Sample.Forge.Issue
{
    internal sealed class ForgeIssueClient : ForgeClientBase, IForgeIssueClient
    {
        public ForgeIssueClient(SampleConfiguration configuration)
            : base(configuration)
        {
        }

        public async Task<dynamic> GetIssue(Guid containerId, Guid issueId)
        {
            var uri = Configuration.IssueManagementPath($"{containerId}/quality-issues/{issueId}");

            return await uri.HttpGetDynamicJson(CreateDefaultHttpRequestHeaders(true));
        }
    }
}
