using Autodesk.Forge.Bim360.ModelCoordination.Clash;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MCSample.Model
{
    public interface IForgeClashClient
    {
        Task<ClashTestSummaryCollection> GetModelSetClashTests(Guid containerId, Guid modelSetId, int? pageLimit = default, string continuationToken = default);

        Task<ClashTestSummaryCollection> GetModelSetClashTests(Guid containerId, Guid modelSetId, uint version, int? pageLimit = default, string continuationToken = default);

        Task<ClashTest> GetModelSetClashTest(Guid containerId, Guid clashTestId);

        Task<ClashTestResourceCollection> GetModelSetClashTestResources(Guid containerId, Guid clashTestId);

        Task DownloadClashTestResource(ClashTestResource resource, FileInfo destination, bool decompress = false);

        Task DownloadClashTestResource(ClashTestResource resource, Func<Stream, Task> downloadStreamProcessor, bool decompress = false);

        Task<ClashGroupClashIntersectionCollection> GetAssignedClashGroups(Guid containerId, Guid testId, int? pageLimit = default, string continuationToken = default);

        Task<IReadOnlyCollection<AssignedClashGroupClashData>> GetAssignedClashGroupDetailBatch(Guid containerId, Guid testId, IEnumerable<Guid> identities, bool useIssueId = false);

        Task<ClashGroupClashIntersectionCollection> GetClosedClashGroups(Guid containerId, Guid testId, int? pageLimit = default, string continuationToken = default);

        Task<IReadOnlyCollection<ClosedClashGroupClashData>> GetClosedClashGroupDetailBatch(Guid containerId, Guid testId, IEnumerable<Guid> identities);

        Task<FileResponse> GetScreenShotAsync(Guid containerId, Guid modelSetId, Guid screenShotId);
    }
}
