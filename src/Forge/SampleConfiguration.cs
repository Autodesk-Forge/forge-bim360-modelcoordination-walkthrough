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
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using System.Web;

namespace Sample.Forge
{
    [JsonObject]
    public class SampleConfiguration
    {
        /// <summary>
        /// The current BIM 360 Account (Hub) GUID
        /// </summary>
        public Guid AccountId { get; set; }

        /// <summary>
        /// The current BIM 360 project GUID
        /// </summary>
        public Guid ProjectId { get; set; }

        /// <summary>
        /// The forge application client ID
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// The Forge applicaiton secret
        /// </summary>
        public string Secret { get; set; }

        /// <summary>
        /// The Forge application callback URL
        /// </summary>
        public string CallbackUrl { get; set; } = "https://localhost:5001/signin/oauth/callback";

        /// <summary>
        /// The current authorisation token
        /// </summary>
        public string AuthToken { get; set; }

        /// <summary>
        /// True if valid BIM 360 project is set
        /// </summary>
        [JsonIgnore]
        public bool HasProject => AccountId != Guid.Empty && ProjectId != Guid.Empty;

        /// <summary>
        /// True if a forge applicaiton is configured
        /// </summary>
        [JsonIgnore]
        public bool HasForgeApp => !string.IsNullOrWhiteSpace(ClientId) && !string.IsNullOrWhiteSpace(Secret) && !string.IsNullOrWhiteSpace(CallbackUrl);

        /// <summary>
        /// The Autodesk host name
        /// </summary>
        [JsonIgnore]
        public string Host { get; set; } = "developer.api.autodesk.com";

        /// <summary>
        /// The Forge API base path
        /// </summary>
        [JsonIgnore]
        public Uri BasePath => new Uri($"https://{Host}/");

        /// <summary>
        /// The base path for the Forge model derivative API
        /// </summary>
        [JsonIgnore]
        public Uri ModelDerivativeBasePath => new Uri($"https://{Host}/modelderivative/v2/designdata/");

        /// <summary>
        /// The base path for the BIM 360 issue management API
        /// </summary>
        [JsonIgnore]
        public Uri IssueManagementBasePath => new Uri($"https://{Host}/issues/v1/containers/");

        /// <summary>
        /// The base path for the model set API
        /// </summary>
        [JsonIgnore]
        public Uri ModelSetApiBasePath => new Uri($"https://{Host}/bim360/modelset/");

        /// <summary>
        /// The base path for the model set index API
        /// </summary>
        [JsonIgnore]
        public Uri ModelSetIndexApiBasePath => new Uri($"https://{Host}/bim360/modelset-index/");

        /// <summary>
        /// The base path for the model set clash API
        /// </summary>
        [JsonIgnore]
        public Uri ModelSetClashApiBasePath => new Uri($"https://{Host}/bim360/clash/");

        /// <summary>
        /// The authenticaiton endpoint for a an OAuth 2.0 3LO token flow
        /// </summary>
        [JsonIgnore]
        public Uri AuthorizeUrlCode => new Uri($"https://{Host}/authentication/v1/authorize?response_type=code&client_id={ClientId}&redirect_uri={UrlSafeCallbackUrl}&scope=data:read%20data:write");

        /// <summary>
        /// The authentication endpoint for an OAuth 2.0 3LO grant flow
        /// </summary>
        [JsonIgnore]
        public Uri AuthorizeUrlGrant => new Uri($"https://{Host}/authentication/v1/authorize?response_type=grant&client_id={ClientId}&redirect_uri={UrlSafeCallbackUrl}&scope=data:read%20data:write");

        /// <summary>
        /// A URL safe version of the callback URL
        /// </summary>
        [JsonIgnore]
        public string UrlSafeCallbackUrl => HttpUtility.UrlEncode(this.CallbackUrl);

        /// <summary>
        /// The BIM 360 encoded account ID
        /// </summary>
        [JsonIgnore]
        public string ForgeBimHubId => $"b.{AccountId}";

        /// <summary>
        /// The BIM 360 encoded project ID
        /// </summary>
        [JsonIgnore]
        public string ForgeBimProjectId => $"b.{ProjectId}";
    }
}
