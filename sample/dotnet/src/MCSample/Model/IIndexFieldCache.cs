using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MCSample.Model
{
    public interface IIndexFieldCache
    {
        FileInfo GetCachePath(Guid containerId, Guid modelSetId, uint version);

        bool IsCached(Guid containerId, Guid modelSetId, uint version);

        Task Cache(Guid containerId, Guid modelSetId, uint version, FileInfo compressedFields);

        Task<IReadOnlyDictionary<string, IndexField>> Get(Guid containerId, Guid modelSetId, uint version);
    }
}
