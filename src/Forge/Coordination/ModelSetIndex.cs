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
using Autodesk.Forge.Bim360.ModelCoordination.Index;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sample.Forge.Coordination
{
    internal sealed class ModelSetIndex : IModelSetIndex
    {
        private readonly IIndexFieldCache _fieldCache;

        private readonly IIndexClient _indexClient;

        private readonly ILocalFileManager _fileManager;

        private readonly Stopwatch _stopwatch = new Stopwatch();

        public ModelSetIndex(IIndexClient indexClient, IIndexFieldCache fieldCache, ILocalFileManager fileManager)
        {
            _indexClient = indexClient ?? throw new ArgumentNullException(nameof(indexClient));

            _fieldCache = fieldCache ?? throw new ArgumentNullException(nameof(fieldCache));

            _fileManager = fileManager ?? throw new ArgumentNullException(nameof(fileManager));
        }

        public async Task<FileInfo> Query(Guid containerId, Guid modelSetId, int version, string selectQuery)
        {
            var query = new IndexQuery
            {
                Statement = selectQuery
            };

            var resultFile = _fileManager.NewPath(query.GetThumbprint());

            if (!resultFile.Exists)
            {

                _stopwatch.Reset();
                _stopwatch.Start();

                var status = await _indexClient.QueryModelSetVersionIndexAsync(
                    containerId,
                    modelSetId,
                    (int)version,
                    query,
                    CancellationToken.None).CompleteJob(
                job => _indexClient.GetModelSetJobAsync(containerId, modelSetId, job.JobId),
                job => job.Status == IndexJobStatus.Running);

                _stopwatch.Stop();
                Debug.WriteLine($"Server-side query execution time (ms) : {_stopwatch.ElapsedMilliseconds}");

                if (status.Status != IndexJobStatus.Succeeded)
                {
                    throw new InvalidOperationException(JsonConvert.SerializeObject(status));
                }

                var resource = new Uri(status.Resources.Results.Url);

                using (var httpStream = await resource.OpenHttpStream(status.Resources.Results.Headers))
                using (var fout = resultFile.Open(FileMode.Create))
                {
                    _stopwatch.Reset();
                    _stopwatch.Start();

                    await httpStream.CopyToAsync(fout);

                    _stopwatch.Stop();
                    Debug.WriteLine($"Index query result download processing time (ms) : {_stopwatch.ElapsedMilliseconds}");
                }
            }

            resultFile.Refresh();

            Debug.WriteLine($"Index query result size (bytes) : {resultFile.Length}");

            return resultFile;
        }

        public async Task Query(Guid containerId, Guid modelSetId, int version, string selectQuery, Func<Stream, Task> resultStreamProcessor)
        {
            var res = await Query(containerId, modelSetId, version, selectQuery);

            using (var fout = res.OpenRead())
            {
                await resultStreamProcessor(fout);
            }
        }

        public async Task<IndexField[]> SearchFields(Guid containerId, Guid modelSetId, int version, string searchText)
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

        public async Task<IndexManifest> GetManifest(Guid containerId, Guid modelSetId, int version)
        {
            return await _indexClient.QueryModelSetVersionIndexManifestAsync(containerId, modelSetId, version);
        }

        public async Task<IReadOnlyDictionary<string, IndexField>> GetFields(Guid containerId, Guid modelSetId, int version)
        {
            if (!_fieldCache.IsCached(containerId, modelSetId, version))
            {
                var resource = await _indexClient.QueryModelSetVersionIndexFieldsAsync(containerId, modelSetId, version);

                var url = new Uri(resource.Url);

                var outputFile = _fieldCache.GetCachePath(containerId, modelSetId, version);

                using (var webStream = await url.OpenHttpStream(resource.Headers, CancellationToken.None))
                using (var fout = outputFile.Open(FileMode.Create))
                {
                    await webStream.CopyToAsync(fout);
                }

                await _fieldCache.Cache(containerId, modelSetId, version, outputFile);
            }

            return await _fieldCache.Get(containerId, modelSetId, version);
        }
    }
}
