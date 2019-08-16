using Microsoft.Extensions.DependencyInjection;

namespace MCSample.Service
{
    internal static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddForgeAppDelegatingHandler(this IServiceCollection services)
        {
            return services.AddTransient<ForgeAppDelegatingHandler>();
        }
    }
}
