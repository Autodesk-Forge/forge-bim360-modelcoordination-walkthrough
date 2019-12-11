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
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Sample.Forge
{
    public static class UriExtensions
    {
        public static async Task<Stream> OpenHttpStream(this Uri address, IDictionary<string, string> headers, CancellationToken cancellationToken = default)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, address))
            {
                foreach (var kvp in headers)
                {
                    request.Headers.TryAddWithoutValidation(kvp.Key, kvp.Value);
                }

                using (var client = new HttpClient())
                {
                    var response = await client.SendAsync(request, cancellationToken);

                    response.EnsureSuccessStatusCode();

                    return await response.Content.ReadAsStreamAsync();
                }
            }
        }

        public static async Task<dynamic> HttpGetDynamicJson(this Uri address, IDictionary<string, string> headers, CancellationToken cancellationToken = default)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, address))
            {
                foreach (var kvp in headers)
                {
                    request.Headers.TryAddWithoutValidation(kvp.Key, kvp.Value);
                }

                using (var client = new HttpClient())
                {
                    var response = await client.SendAsync(request, cancellationToken);

                    response.EnsureSuccessStatusCode();

                    return JObject.Parse((await response.Content.ReadAsStringAsync()));
                }
            }
        }
    }
}
