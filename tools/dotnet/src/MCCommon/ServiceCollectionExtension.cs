using MCCommon.Auth;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Composition.Hosting;
using System.Reflection;
using System.Text;

namespace MCCommon
{
    public static class ServiceCollectionExtension
    {
        private static readonly Lazy<CompositionContext> LazyContext = new Lazy<CompositionContext>(
            () => new ContainerConfiguration().AddModelCoordinationCommon().CreateContainer());

        public static IServiceCollection AddModelCoordinationCommon(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton(p => LazyContext.Value.GetExport<ITokenManager>());
            serviceCollection.AddTransient(p => LazyContext.Value.GetExport<IForgeAppConfigurationManager>());

            return serviceCollection;
        }
    }
}
