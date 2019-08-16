using Autodesk.Nucleus.Index.Client.V1;
using Autodesk.Nucleus.Index.Entities.V1;
using MCSample.Service;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MCSample.Model
{
    [Export(typeof(IIndexClient))]
    internal sealed class IndexClient : ClientBase, IIndexClient
    {
        private readonly IIndexFieldCache _fieldCache;

        [ImportingConstructor]
        public IndexClient(IModelCoordinationServiceCollectionFactory serviceCollecitonFactory, IIndexFieldCache fieldCache)
            : base(serviceCollecitonFactory) => _fieldCache = fieldCache ?? throw new ArgumentNullException(nameof(fieldCache));

        public async Task<FileInfo> QueryIndex(Guid containerId, Guid modelSetId, uint version, string selectQuery)
        {
            var query = new IndexQuery
            {
                Statement = selectQuery
            };

            var resultFile = SampleFileManager.NewStatePath(query.GetThumbprint());

            if (!resultFile.Exists)
            {
                using (var sc = await CreateServiceProvider())
                {
                    var client = sc.GetRequiredService<IIndexClientV1>();

                    this.ResetStopwatch();
                    this.StartStopwatch();
                    var status = await RunJob(
                        () => client.QueryModelSetVersionIndexAsync(
                            containerId,
                            modelSetId,
                            (int)version,
                            query,
                            CancellationToken.None),
                        job => client.GetModelSetJobAsync(containerId, modelSetId, job.JobId),
                        job => job.Status == IndexJobStatus.Running);
                    this.StopStopwatch();
                    Debug.WriteLine($"Server-side query execution time (ms) : {ElapsedMilliseconds}");

                    if (status.Status != IndexJobStatus.Succeeded)
                    {
                        throw new InvalidOperationException(JsonConvert.SerializeObject(status));
                    }

                    var resource = new Uri(status.Resources.Results.Url);

                    using (var httpStream = await resource.OpenHttpStream(status.Resources.Results.Headers))
                    using (var fout = resultFile.Open(FileMode.Create))
                    {
                        this.ResetStopwatch();
                        this.StartStopwatch();
                        await httpStream.CopyToAsync(fout);
                        this.StopStopwatch();
                        Debug.WriteLine($"Index query result download processing time (ms) : {ElapsedMilliseconds}");
                    }
                }
            }

            resultFile.Refresh();
            Debug.WriteLine($"Index query result size (bytes) : {resultFile.Length}");

            return resultFile;
        }

        public async Task QueryIndex(Guid containerId, Guid modelSetId, uint version, string selectQuery, Func<Stream, Task> resultStreamProcessor)
        {
            var res = await QueryIndex(containerId, modelSetId, version, selectQuery);

            using (var fout = res.OpenRead())
            {
                await resultStreamProcessor(fout);
            }
        }

        public async Task<IndexField[]> SearchFields(Guid containerId, Guid modelSetId, uint version, string searchText)
        {
            IndexField[] res = null;

            var fields = await GetFields(containerId, modelSetId, version);

            if (!string.IsNullOrWhiteSpace(searchText) && fields?.Count > 0)
            {
                res = fields.Values.Where(
                    f => !string.IsNullOrWhiteSpace(f.Key) && f.Key.ToUpperInvariant().Contains(searchText.ToUpperInvariant()) ||
                    !string.IsNullOrWhiteSpace(f.Name) && f.Name.ToUpperInvariant().Contains(searchText.ToUpperInvariant()) ||
                    !string.IsNullOrWhiteSpace(f.Category) && f.Category.ToUpperInvariant().Contains(searchText.ToUpperInvariant()) ||
                    !string.IsNullOrWhiteSpace(f.Uom) && f.Uom.ToUpperInvariant().Contains(searchText.ToUpperInvariant()) ||
                    f.Type.ToString() == searchText.ToUpperInvariant()).ToArray();
            }

            return res;
        }

        public async Task<IndexManifest> GetIndexManifest(Guid containerId, Guid modelSetId, uint version)
        {
            using (var sc = await CreateServiceProvider())
            {
                var client = sc.GetRequiredService<IIndexClientV1>();

                return await client.QueryModelSetVersionIndexManifestAsync(containerId, modelSetId, (int)version);
            }
        }

        public async Task<IReadOnlyDictionary<string, IndexField>> GetFields(Guid containerId, Guid modelSetId, uint version)
        {
            if (!_fieldCache.IsCached(containerId, modelSetId, version))
            {
                using (var sc = await CreateServiceProvider())
                {
                    var client = sc.GetRequiredService<IIndexClientV1>();

                    var resource = await client.QueryModelSetVersionIndexFieldsAsync(containerId, modelSetId, (int)version);

                    var url = new Uri(resource.Url);

                    var outputFile = _fieldCache.GetCachePath(containerId, modelSetId, version);

                    using (var webStream = await url.OpenHttpStream(resource.Headers, CancellationToken.None))
                    using (var fout = outputFile.Open(FileMode.Create))
                    {
                        await webStream.CopyToAsync(fout);
                    }

                    await _fieldCache.Cache(containerId, modelSetId, version, outputFile);
                }
            }

            return await _fieldCache.Get(containerId, modelSetId, version);
        }
    }
}
