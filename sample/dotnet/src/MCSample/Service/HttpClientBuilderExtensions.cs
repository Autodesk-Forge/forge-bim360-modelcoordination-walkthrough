using Microsoft.Extensions.DependencyInjection;

namespace MCSample.Service
{
    internal static class HttpClientBuilderExtensions
    {
        public static IHttpClientBuilder AddTokenManagerDelegatingHandler(this IHttpClientBuilder builder) =>
            builder.AddHttpMessageHandler<ForgeAppDelegatingHandler>();
    }
}
