using Autodesk.Nucleus.Clash.Client.V3;
using Autodesk.Nucleus.Index.Client.V1;
using Autodesk.Nucleus.Scopes.Client.V3;
using MCCommon;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Composition;
using System.Threading.Tasks;

namespace MCSample.Service
{
    [Shared]
    [Export(typeof(IModelCoordinationServiceCollectionFactory))]
    public class ModelCoordinationServiceCollectionFactory : IModelCoordinationServiceCollectionFactory
    {
        private readonly IForgeAppConfigurationManager _configManager;

        private readonly Lazy<Task<ForgeAppConfiguration>> _lazyDefaultConfiguraitonTask;

        [ImportingConstructor]
        public ModelCoordinationServiceCollectionFactory(IForgeAppConfigurationManager configManager)
        {
            _configManager = configManager ?? throw new ArgumentNullException(nameof(configManager));

            _lazyDefaultConfiguraitonTask = new Lazy<Task<ForgeAppConfiguration>>(() => _configManager.GetDefaultConfiguration());
        }

        public Task<ForgeAppConfiguration> GetConfiguration() => _lazyDefaultConfiguraitonTask.Value;

        public async Task<ServiceProvider> CreateServiceProvider()
        {
            var sc = new ServiceCollection();

            sc.AddSingleton(_configManager);

            sc.AddForgeAppDelegatingHandler();

            var config = await GetConfiguration();

            sc.AddHttpClient<IIndexClientV1, IndexClientV1>(options => options.BaseAddress = config.NucleusLegacyBasePath)
              .AddTokenManagerDelegatingHandler();

            sc.AddHttpClient<IScopesClientV3, ScopesClientV3>(options => options.BaseAddress = config.NucleusModelSetBasePath)
              .AddTokenManagerDelegatingHandler();

            sc.AddHttpClient<IClashClientV3, ClashClientV3>(options => options.BaseAddress = config.NucleusClashBasePath)
              .AddTokenManagerDelegatingHandler();

            return sc.BuildServiceProvider();
        }
    }
}
