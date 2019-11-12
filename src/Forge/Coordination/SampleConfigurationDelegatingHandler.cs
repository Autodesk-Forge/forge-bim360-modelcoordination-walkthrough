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
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Sample.Forge.Coordination
{
    public sealed class SampleConfigurationDelegatingHandler : DelegatingHandler
    {
        private readonly SampleConfiguration _configuration;

        public SampleConfigurationDelegatingHandler(SampleConfiguration configuration) => _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Typically this is where you would call the service which is doing your token management
            // for these samples just use the token attached to the configuration.
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _configuration.AuthToken);

            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
