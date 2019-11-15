using Microsoft.Extensions.DependencyInjection;
using Sample.Forge;
using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Text;

namespace Forge.Automation
{
    public abstract class ForgeCmdlet : PSCmdlet
    {
        private static readonly Lazy<IServiceProvider> _serviceProvider = new Lazy<IServiceProvider>(() =>
        {
            var provider = new ServiceCollection()
                .AddSampleForgeServices()
                .BuildServiceProvider();

            var fileManager = provider.GetRequiredService<ILocalFileManager>();

            if(fileManager.JsonPathExists<SampleConfiguration>())
            {
                var configFile = fileManager.ReadJson<SampleConfiguration>();

                var current = provider.GetRequiredService<SampleConfiguration>();

                current.AccountId = configFile.AccountId;
                current.ProjectId = configFile.ProjectId;
                current.ClientId = configFile.ClientId;
                current.Secret = configFile.Secret;
                current.CallbackUrl = configFile.CallbackUrl;
                current.AuthToken = configFile.AuthToken;
            }

            return provider;
        });

        protected IServiceProvider ServiceProvider => _serviceProvider.Value;

        protected SampleConfiguration Configuration => ServiceProvider.GetRequiredService<SampleConfiguration>();
    }
}
