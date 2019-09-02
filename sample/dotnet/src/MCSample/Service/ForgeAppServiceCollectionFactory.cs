using Autodesk.Forge.Bim360.ModelCoordination.Clash;
using Autodesk.Forge.Bim360.ModelCoordination.Index;
using Autodesk.Forge.Bim360.ModelCoordination.ModelSet;
using MCCommon;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Composition;
using System.Threading.Tasks;

namespace MCSample.Service
{
    [Shared]
    [Export(typeof(IForgeAppServiceCollectionFactory))]
    public class ForgeAppServiceCollectionFactory : IForgeAppServiceCollectionFactory
    {
        private readonly IForgeAppConfigurationManager _configManager;

        private readonly Lazy<Task<ForgeAppConfiguration>> _lazyDefaultConfiguraitonTask;

        [ImportingConstructor]
        public ForgeAppServiceCollectionFactory(IForgeAppConfigurationManager configManager)
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

            sc.AddHttpClient<IIndexClient, IndexClient>(options => options.BaseAddress = config.NucleusLegacyBasePath)
              .AddTokenManagerDelegatingHandler();

            sc.AddHttpClient<IModelSetClient, ModelSetClient>(options => options.BaseAddress = config.NucleusModelSetBasePath)
              .AddTokenManagerDelegatingHandler();

            sc.AddHttpClient<IClashClient, ClashClient>(options => options.BaseAddress = config.NucleusClashBasePath)
              .AddTokenManagerDelegatingHandler();

            return sc.BuildServiceProvider();
        }
    }
}
