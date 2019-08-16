using MCCommon;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Threading.Tasks;

namespace MCSample.Model.Cosmo
{

    [Export(typeof(ICosmoRevitClashClient))]
    internal sealed class CosmoRevitClashClient : ICosmoRevitClashClient
    {
        private const string DatabseId = "Clash";

        private const string ContainerId = "Results";

        private readonly Lazy<Task<CosmoDbConfiguration>> _lazyGetConfigTask;

        [ImportingConstructor]
        public CosmoRevitClashClient(ICosmoDbConfigurationManager configManager) => _lazyGetConfigTask
            = new Lazy<Task<CosmoDbConfiguration>>(async () => await configManager.Get());

        public async Task CreateContainerIfNotExists()
        {
            using (var client = await GetClient())
            {
                var dbResp = await client.CreateDatabaseIfNotExistsAsync(DatabseId);

                if (dbResp.StatusCode != System.Net.HttpStatusCode.Created &&
                    dbResp.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new CosmoResponseException<DatabaseResponse>(dbResp);
                }

                var db = dbResp.Database;

                var conResp = await db.CreateContainerIfNotExistsAsync(ContainerId, "/test");

                if (conResp.StatusCode != System.Net.HttpStatusCode.Created &&
                    conResp.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new CosmoResponseException<ContainerResponse>(conResp);
                }
            }
        }

        public async Task PublishReports(IReadOnlyCollection<RevitClashReport> reports)
        {
            await CreateContainerIfNotExists();

            using (var client = await GetClient())
            {
                var container = client.GetContainer(DatabseId, ContainerId);

                foreach (var report in reports)
                {
                    await container.CreateItemAsync<RevitClashReport>(report, new PartitionKey(report.Test.ToString().ToLowerInvariant()));
                }
            }
        }

        private async Task<CosmosClient> GetClient()
        {
            var config = await _lazyGetConfigTask.Value;

            return new CosmosClient(config.AccountEndpoint.OriginalString, config.AccountPrimaryKey);
        }
    }
}
