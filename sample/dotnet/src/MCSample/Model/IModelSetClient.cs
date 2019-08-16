using Autodesk.Nucleus.Scopes.Entities.V3;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MCSample.Model
{
    public interface IModelSetClient
    {
        Task<Container> GetContainer(Guid containerId);

        Task<ModelSet> CreateModelSet(Guid containerId, string name, ModelSetFolder[] folders);

        Task DeleteModelSet(Guid containerId, Guid modelSetId);

        Task<ModelSet> GetModelSet(Guid containerId, Guid modelSetId);

        Task<ModelSetVersionSummaryCollection> GetModelSetVersions(Guid containerId, Guid modelSetId, int? pageLimit = default, string continuationToken = default);

        Task<ModelSetVersion> GetModelSetVersion(Guid containerId, Guid modelSetId, uint version);

        Task<ModelSetVersion> GetLatestModelSetVersion(Guid containerId, Guid modelSetId);

        Task<ModelSetViewCollection> GetModelSetViews(
            Guid containerId,
            Guid modelSetId,
            int? pageLimit = default,
            string continuationToken = default,
            string createdBy = default,
            string modifiedBy = default,
            DateTimeOffset? after = default,
            DateTimeOffset? before = default,
            bool? isPrivate = default,
            IEnumerable<ViewSortFields> sortBy = default,
            SortDirection? sortDirection = default);

        Task<ModelSetView> CreateModelSetView(Guid containerId, Guid modelSetId, NewModelSetView view);

        Task<ModelSetView> UpdateModelSetView(Guid containerId, Guid modelSetId, Guid viewId, UpdateModelSetView updatedView);

        Task<ModelSetViewVersion> GetModelSetViewVersion(Guid containerId, Guid modelSetId, uint version, Guid viewId);
    }
}
