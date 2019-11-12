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
using Autodesk.Forge.Bim360.ModelCoordination.Clash;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sample.Forge.Coordination
{
    public static class ClashTestResourceExtensions
    {
        public static async Task DownloadClashTestResource(this ClashTestResource resource, FileInfo destination, bool decompress = false)
        {
            await DownloadClashTestResource(
                resource,
                async resultStream =>
                {
                    using (var fout = destination.Open(FileMode.Create))
                    {
                        await resultStream.CopyToAsync(fout);
                    }
                },
                decompress);
        }

        public async static Task DownloadClashTestResource(this ClashTestResource resource, Func<Stream, Task> downloadStreamProcessor, bool decompress = false)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, resource.Url))
            {
                foreach (var kvp in resource.Headers)
                {
                    request.Headers.TryAddWithoutValidation(kvp.Key, kvp.Value);
                }

                using (var client = new HttpClient())
                {
                    var response = await client.SendAsync(request, CancellationToken.None);

                    response.EnsureSuccessStatusCode();

                    using (var res = await response.Content.ReadAsStreamAsync())
                    {
                        if (decompress)
                        {
                            using (var dc = new GZipStream(res, CompressionMode.Decompress))
                            {
                                await downloadStreamProcessor(dc);
                            }
                        }
                        else
                        {
                            await downloadStreamProcessor(res);
                        }
                    }
                }
            }
        }
    }
}
