using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Composition;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;

namespace MCSample.Model
{
    [Shared]
    [Export(typeof(IIndexFieldCache))]
    internal sealed class IndexFieldCache : IIndexFieldCache
    {
        private readonly Dictionary<string, IReadOnlyDictionary<string, IndexField>> _cache = new Dictionary<string, IReadOnlyDictionary<string, IndexField>>(StringComparer.OrdinalIgnoreCase);

        public async Task<IReadOnlyDictionary<string, IndexField>> Get(Guid containerId, Guid modelSetId, uint version)
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

        public async Task Cache(Guid containerId, Guid modelSetId, uint version, FileInfo compressedFields)
        {
            var key = ToKey(containerId, modelSetId, version);

            var fieldMap = new Dictionary<string, IndexField>(StringComparer.OrdinalIgnoreCase)
            {
                { "file", new IndexField { Key = "file", Name  = "file", Type = IndexFieldType.Integer, Category = "Nucleus" } },
                { "docs", new IndexField { Key = "docs", Name  = "docs", Type = IndexFieldType.BLOB, Category = "Nucleus" } },
                { "db", new IndexField { Key = "db", Name  = "db", Type = IndexFieldType.Integer, Category = "Nucleus" } },
                { "id", new IndexField { Key = "id", Name  = "id", Type = IndexFieldType.Integer, Category = "Nucleus" } },
                { "checksum", new IndexField { Key = "checksum", Name  = "checksum", Type = IndexFieldType.Integer, Category = "Nucleus" } }
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

        public FileInfo GetCachePath(Guid containerId, Guid modelSetId, uint version)
        {
            var key = ToKey(containerId, modelSetId, version);

            return SampleFileManager.NewStatePath(key);
        }

        public bool IsCached(Guid containerId, Guid modelSetId, uint version)
        {
            var key = ToKey(containerId, modelSetId, version);

            var path = GetCachePath(containerId, modelSetId, version);

            return _cache.ContainsKey(key) || path.Exists;
        }

        private static string ToKey(Guid containerId, Guid modelSetId, uint version) => $"{containerId}~{modelSetId}~{version}";
    }
}
