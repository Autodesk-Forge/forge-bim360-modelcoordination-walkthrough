using Autodesk.Forge.Bim360.ModelCoordination.Index;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MCSample.Model
{
    public interface IForgeIndexClient
    {
        Task<IReadOnlyDictionary<string, IndexField>> GetFields(Guid containerId, Guid modelSetId, uint version);

        Task<IndexField[]> SearchFields(Guid containerId, Guid modelSetId, uint version, string searchText);

        Task<FileInfo> QueryIndex(Guid containerId, Guid modelSetId, uint version, string selectQuery);

        Task QueryIndex(Guid containerId, Guid modelSetId, uint version, string selectQuery, Func<Stream, Task> resultStreamProcessor);

        Task<IndexManifest> GetIndexManifest(Guid containerId, Guid modelSetId, uint version);
    }
}
