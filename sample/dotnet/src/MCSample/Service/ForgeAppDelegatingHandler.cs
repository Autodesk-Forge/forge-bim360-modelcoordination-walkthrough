using MCCommon;
using System;
using System.Composition;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace MCSample.Service
{
    [Export]
    public sealed class ForgeAppDelegatingHandler : DelegatingHandler
    {
        private readonly IForgeAppConfigurationManager _configManager;

        [ImportingConstructor]
        public ForgeAppDelegatingHandler(IForgeAppConfigurationManager tokenManager) => _configManager = tokenManager ?? throw new ArgumentNullException(nameof(tokenManager));

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await _configManager.GetCachedToken();

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
