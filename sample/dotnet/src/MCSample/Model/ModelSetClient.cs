using Autodesk.Nucleus.Scopes.Client.V3;
using Autodesk.Nucleus.Scopes.Entities.V3;
using MCSample.Service;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;

namespace MCSample.Model
{
    [Export(typeof(IModelSetClient))]
    internal sealed class ModelSetClient : ClientBase, IModelSetClient
    {
        [ImportingConstructor]
        public ModelSetClient(IModelCoordinationServiceCollectionFactory serviceCollecitonFactory)
            : base(serviceCollecitonFactory)
        {
        }

        public async Task<Container> GetContainer(Guid containerId)
        {
            using (var sc = await CreateServiceProvider())
            {
                var client = sc.GetRequiredService<IScopesClientV3>();

                return await client.GetContainerAsync(containerId);
            }
        }

        public async Task<ModelSet> CreateModelSet(Guid containerId, string name, ModelSetFolder[] folders)
        {
            using (var sc = await CreateServiceProvider())
            {
                var client = sc.GetRequiredService<IScopesClientV3>();

                var status = await RunJob(
                    () => client.CreateModelSetAsync(
                        containerId,
                        new NewModelSet
                        {
                            ModelSetId = Guid.NewGuid(),
                            Name = name,
                            Description = $"This model set has been created to demonstrate the Model Coordination API and encapsulates {name}",
                            Folders = folders.ToList()
                        }),
                    job => client.GetModelSetJobAsync(containerId, job.ModelSetId, job.JobId),
                    job => job.Status == ModelSetJobStatus.Running);

                if (status.Status != ModelSetJobStatus.Succeeded)
                {
                    throw new InvalidOperationException(JsonConvert.SerializeObject(status));
                }

                return await client.GetModelSetAsync(containerId, status.ModelSetId);
            }
        }

        public async Task DeleteModelSet(Guid containerId, Guid modelSetId)
        {
            using (var sc = await CreateServiceProvider())
            {
                var client = sc.GetRequiredService<IScopesClientV3>();

                throw new NotImplementedException();
            }
        }

        public async Task<ModelSet> GetModelSet(Guid containerId, Guid modelSetId)
        {
            using (var sc = await CreateServiceProvider())
            {
                var client = sc.GetRequiredService<IScopesClientV3>();

                return await client.GetModelSetAsync(containerId, modelSetId);
            }
        }

        public async Task<ModelSetVersionSummaryCollection> GetModelSetVersions(Guid containerId, Guid modelSetId, int? pageLimit = default, string continuationToken = default)
        {
            using (var sc = await CreateServiceProvider())
            {
                var client = sc.GetRequiredService<IScopesClientV3>();

                return await client.GetModelSetVersionsAsync(containerId, modelSetId, pageLimit, continuationToken);
            }
        }

        public async Task<ModelSetVersion> GetModelSetVersion(Guid containerId, Guid modelSetId, uint version)
        {
            using (var sc = await CreateServiceProvider())
            {
                var client = sc.GetRequiredService<IScopesClientV3>();

                return await client.GetModelSetVersionAsync(containerId, modelSetId, (int)version);
            }
        }

        public async Task<ModelSetVersion> GetLatestModelSetVersion(Guid containerId, Guid modelSetId)
        {
            using (var sc = await CreateServiceProvider())
            {
                var client = sc.GetRequiredService<IScopesClientV3>();

                return await client.GetModelSetVersionLatestAsync(containerId, modelSetId);
            }
        }

        public async Task<ModelSetViewCollection> GetModelSetViews(
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
            SortDirection? sortDirection = default)
        {
            using (var sc = await CreateServiceProvider())
            {
                var client = sc.GetRequiredService<IScopesClientV3>();

                return await client.GetModelSetViewsAsync(containerId, modelSetId, pageLimit, continuationToken, createdBy, modifiedBy, after, before, isPrivate, sortBy, sortDirection);
            }
        }

        public async Task<ModelSetView> CreateModelSetView(Guid containerId, Guid modelSetId, NewModelSetView view)
        {
            using (var sc = await CreateServiceProvider())
            {
                var client = sc.GetRequiredService<IScopesClientV3>();

                var status = await RunJob(
                    () => client.CreateModelSetViewAsync(containerId, modelSetId, view),
                    job => client.GetModelSetViewJobAsync(containerId, modelSetId, job.ViewId, job.JobId),
                    job => job.Status == ModelSetViewJobStatus.Running);

                if (status.Status != ModelSetViewJobStatus.Succeeded)
                {
                    throw new InvalidOperationException(JsonConvert.SerializeObject(status));
                }

                return await client.GetModelSetViewAsync(containerId, modelSetId, status.ViewId);
            }
        }

        public async Task<ModelSetView> UpdateModelSetView(Guid containerId, Guid modelSetId, Guid viewId, UpdateModelSetView updatedView)
        {
            using (var sc = await CreateServiceProvider())
            {
                var client = sc.GetRequiredService<IScopesClientV3>();

                var status = await RunJob(
                    () => client.UpdateModelSetViewAsync(containerId, modelSetId, viewId, updatedView),
                    job => client.GetModelSetViewJobAsync(containerId, modelSetId, viewId, job.JobId),
                    job => job.Status == ModelSetViewJobStatus.Running);

                if (status.Status != ModelSetViewJobStatus.Succeeded)
                {
                    throw new InvalidOperationException(JsonConvert.SerializeObject(status));
                }

                return await client.GetModelSetViewAsync(containerId, modelSetId, viewId);
            }
        }

        public async Task<ModelSetViewVersion> GetModelSetViewVersion(Guid containerId, Guid modelSetId, uint version, Guid viewId)
        {
            using (var sc = await CreateServiceProvider())
            {
                var client = sc.GetRequiredService<IScopesClientV3>();

                return await client.GetModelSetViewVersionAsync(containerId, modelSetId, (int)version, viewId);
            }
        }
    }
}
