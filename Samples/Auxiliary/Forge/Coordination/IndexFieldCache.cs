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
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Forge.Coordination
{
    internal sealed class IndexFieldCache : IIndexFieldCache
    {
        private readonly Dictionary<string, IReadOnlyDictionary<string, IndexField>> _cache = new Dictionary<string, IReadOnlyDictionary<string, IndexField>>(StringComparer.OrdinalIgnoreCase);

        private readonly ILocalFileManager _fileManager;

        public IndexFieldCache(ILocalFileManager fileManager) => _fileManager = fileManager ?? throw new ArgumentNullException(nameof(fileManager));

        public async Task<IReadOnlyDictionary<string, IndexField>> Get(Guid containerId, Guid modelSetId, int version)
        {
            var key = ToKey(containerId, modelSetId, version);

            if (!_cache.ContainsKey(key))
            {
                var path = GetCachePath(containerId, modelSetId, version);

                if (path.Exists)
                {
                    await Cache(containerId, modelSetId, version, path);
                }
            }

            return _cache.ContainsKey(key) ? _cache[key] : null;
        }

        public async Task Cache(Guid containerId, Guid modelSetId, int version, FileInfo compressedFields)
        {
            var key = ToKey(containerId, modelSetId, version);

            var fieldMap = new Dictionary<string, IndexField>(StringComparer.OrdinalIgnoreCase)
            {
                { "file", new IndexField { Key = "file", Name  = "file", Type = IndexFieldType.Integer, Category = "file" } },
                { "docs", new IndexField { Key = "docs", Name  = "docs", Type = IndexFieldType.BLOB, Category = "docs" } },
                { "db", new IndexField { Key = "db", Name  = "db", Type = IndexFieldType.Integer, Category = "db" } },
                { "id", new IndexField { Key = "id", Name  = "id", Type = IndexFieldType.Integer, Category = "id" } },
                { "checksum", new IndexField { Key = "checksum", Name  = "checksum", Type = IndexFieldType.Integer, Category = "checksum" } }
            };

            using (var fin = compressedFields.OpenRead())
            using (var gzs = new GZipStream(fin, CompressionMode.Decompress))
            using (var sr = new StreamReader(gzs, Encoding.UTF8))
            {
                string line = null;

                while ((line = await sr.ReadLineAsync()) != null)
                {
                    var field = JsonConvert.DeserializeObject<IndexField>(line);

                    fieldMap[field.Key] = field;
                }
            }

            _cache[key] = fieldMap;
        }

        public FileInfo GetCachePath(Guid containerId, Guid modelSetId, int version)
        {
            var key = ToKey(containerId, modelSetId, version);

            return _fileManager.NewPath(key);
        }

        public bool IsCached(Guid containerId, Guid modelSetId, int version)
        {
            var key = ToKey(containerId, modelSetId, version);

            var path = GetCachePath(containerId, modelSetId, version);

            return _cache.ContainsKey(key) || path.Exists;
        }

        private static string ToKey(Guid containerId, Guid modelSetId, int version) => $"{containerId}~{modelSetId}~{version}";
    }
}
