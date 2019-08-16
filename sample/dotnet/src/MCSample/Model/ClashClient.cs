using Autodesk.Nucleus.Clash.Client.V3;
using Autodesk.Nucleus.Clash.Entities.V3;
using MCSample.Service;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Composition;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace MCSample.Model
{
    [Export(typeof(IClashClient))]
    internal sealed class ClashClient : ClientBase, IClashClient
    {
        [ImportingConstructor]
        public ClashClient(IModelCoordinationServiceCollectionFactory serviceCollecitonFactory)
            : base(serviceCollecitonFactory)
        {
        }

        public async Task<ClashTestSummaryCollection> GetModelSetClashTests(Guid containerId, Guid modelSetId, int? pageLimit = default, string continuationToken = default)
        {
            using (var sc = await CreateServiceProvider())
            {
                var client = sc.GetRequiredService<IClashClientV3>();

                return await client.GetModelSetClashTestsAsync(containerId, modelSetId, pageLimit, continuationToken);
            }
        }

        public async Task<ClashTestSummaryCollection> GetModelSetClashTests(Guid containerId, Guid modelSetId, uint version, int? pageLimit = default, string continuationToken = default)
        {
            using (var sc = await CreateServiceProvider())
            {
                var client = sc.GetRequiredService<IClashClientV3>();

                return await client.GetModelSetVersionClashTestsAsync(containerId, modelSetId, (int)version, pageLimit, continuationToken);
            }
        }

        public async Task<ClashTest> GetModelSetClashTest(Guid containerId, Guid clashTestId)
        {
            using (var sc = await CreateServiceProvider())
            {
                var client = sc.GetRequiredService<IClashClientV3>();

                return await client.GetClashTestAsync(containerId, clashTestId);
            }
        }

        public async Task<ClashTestResourceCollection> GetModelSetClashTestResources(Guid containerId, Guid clashTestId)
        {
            using (var sc = await CreateServiceProvider())
            {
                var client = sc.GetRequiredService<IClashClientV3>();

                return await client.GetClashTestResourcesAsync(containerId, clashTestId);
            }
        }

        public async Task DownloadClashTestResource(ClashTestResource resource, FileInfo destination, bool decompress = false)
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

        public async Task DownloadClashTestResource(ClashTestResource resource, Func<Stream, Task> downloadStreamProcessor, bool decompress = false)
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

        public async Task<ClashGroupClashIntersectionCollection> GetAssignedClashGroups(Guid containerId, Guid testId, int? pageLimit = default, string continuationToken = default)
        {
            using (var sc = await CreateServiceProvider())
            {
                var client = sc.GetRequiredService<IClashClientV3>();

                return await client.GetClashTestAssignedClashGroupIntersectionAsync(containerId, testId, pageLimit, continuationToken);
            }
        }

        public async Task<IReadOnlyCollection<AssignedClashGroupClashData>> GetAssignedClashGroupDetailBatch(Guid containerId, Guid testId, IEnumerable<Guid> identities, bool useIssueId = false)
        {
            using (var sc = await CreateServiceProvider())
            {
                var client = sc.GetRequiredService<IClashClientV3>();

                return await client.GetAssignedClashGroupBatchAsync(containerId, testId, useIssueId, identities);
            }
        }

        public async Task<ClashGroupClashIntersectionCollection> GetClosedClashGroups(Guid containerId, Guid testId, int? pageLimit = default, string continuationToken = default)
        {
            using (var sc = await CreateServiceProvider())
            {
                var client = sc.GetRequiredService<IClashClientV3>();

                return await client.GetClashTestClosedClashGroupIntersectionAsync(containerId, testId, pageLimit, continuationToken);
            }
        }

        public async Task<IReadOnlyCollection<ClosedClashGroupClashData>> GetClosedClashGroupDetailBatch(Guid containerId, Guid testId, IEnumerable<Guid> identities)
        {
            using (var sc = await CreateServiceProvider())
            {
                var client = sc.GetRequiredService<IClashClientV3>();

                return await client.GetClosedClashGroupDataBatchAsync(containerId, testId, identities);
            }
        }

        public async Task<Autodesk.Nucleus.Clash.Client.V3.FileResponse> GetScreenShotAsync(Guid containerId, Guid modelSetId, Guid screenShotId)
        {
            using (var sc = await CreateServiceProvider())
            {
                var client = sc.GetRequiredService<IClashClientV3>();

                return await client.GetScreenShotAsync(containerId, modelSetId, screenShotId);
            }
        }
    }
}
