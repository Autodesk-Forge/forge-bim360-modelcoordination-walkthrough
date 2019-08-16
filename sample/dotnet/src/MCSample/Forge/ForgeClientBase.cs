using Autodesk.Forge.Model;
using MCCommon;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MCSample.Forge
{
    internal abstract class ForgeClientBase : IForgeClient
    {
        private readonly IForgeAppConfigurationManager _configurationManager = null;

        private readonly Lazy<Task<ForgeAppConfiguration>> _lazyForgeAppConfiguration = null;

        public ForgeClientBase(IForgeAppConfigurationManager configurationManager)
        {
            _configurationManager = configurationManager ?? throw new ArgumentNullException(nameof(configurationManager));

            _lazyForgeAppConfiguration = new Lazy<Task<ForgeAppConfiguration>>(() => _configurationManager.GetDefaultConfiguration());
        }

        public ForgeAppConfiguration Configuration => _lazyForgeAppConfiguration.Value.Result;

        public async Task<string> GetToken() => await _configurationManager.GetCachedToken() ?? throw new InvalidOperationException("No token set!");

        public async Task<T> NewForgeApi<T>()
        {
            var apiType = typeof(T);

            var instance = apiType.GetConstructor(new[] { typeof(string) }).Invoke(new object[] { Configuration.BasePath.AbsoluteUri });

            var configProp = apiType.GetProperty("Configuration");

            var config = configProp.GetValue(instance);

            var atProp = config.GetType().GetProperty("AccessToken");

            atProp.SetValue(config, await GetToken());

            return (T)instance;
        }

        protected async Task<T> CallService<T>(Func<Task<DynamicJsonResponse>> serviceCall)
        {
            DynamicJsonResponse res = await serviceCall();

            return res.ToObject<T>();
        }

        protected async Task<IDictionary<string, string>> CreateDefaultHttpRequestHeaders(bool contentTypeJsonApi = false)
        {
            var token = await GetToken();

            var headers = new Dictionary<string, string>
            {
                { "Authorization", $"Bearer {token}" }
            };

            if (contentTypeJsonApi)
            {
                headers.Add("Content-Type", "application/vnd.api+json");
            }

            return headers;
        }
    }
}
