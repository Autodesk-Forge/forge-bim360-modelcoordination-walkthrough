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
using Autodesk.Forge;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Sample.Forge.Data
{
    internal sealed class ForgeDerivativeClient : ForgeClientBase, IForgeDerivativeClient
    {
        public ForgeDerivativeClient(SampleConfiguration configuration)
            : base(configuration)
        {
        }

        public async Task<dynamic> GetDerivativeManifest(string urn)
        {
            var api = NewForgeApi<DerivativesApi>();

            var manifestResponse = await api.GetManifestAsyncWithHttpInfo(urn);

            dynamic manifest = null;

            if (manifestResponse.StatusCode == 200)
            {
                manifest = JObject.Parse(JsonConvert.SerializeObject(manifestResponse.Data));
            }

            return manifest;
        }

        public async Task GetDerivative(string urn, string derivativeUrn, Func<Stream, Task> streamProcessor)
        {
            var uri = Configuration.ModelDerivativePath($"{urn}/manifest/{derivativeUrn}");

            using (var httpStream = await uri.OpenHttpStream(CreateDefaultHttpRequestHeaders()))
            {
                await streamProcessor(httpStream);
            }
        }
    }
}
