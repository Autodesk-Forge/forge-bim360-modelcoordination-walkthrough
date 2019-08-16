using MCCommon;
using System;
using System.Composition;
using System.Composition.Hosting;
using System.Reflection;

namespace MCSample.Forge
{
    public class ForgeAppContext : IDisposable
    {
        private bool disposed = false;

        private CompositionHost _compositionHost;

        private ForgeAppContext(CompositionHost compositionHost)
        {
            _compositionHost = compositionHost ?? throw new ArgumentNullException(nameof(compositionHost));
        }

        public CompositionContext CompositionContext => _compositionHost;

        public T ExportService<T>() => CompositionContext.GetExport<T>();

        public static ForgeAppContext Create() => Create(new ContainerConfiguration());

        public static ForgeAppContext Create(ContainerConfiguration containerConfiguration, bool addCommon = true, bool addSelf = true)
        {
            if (addCommon)
            {
                containerConfiguration.AddModelCoordinationCommon();
            }

            if (addSelf)
            {
                containerConfiguration.WithAssembly(Assembly.GetExecutingAssembly());
            }

            return new ForgeAppContext(containerConfiguration.CreateContainer());
        }

        #region IDisposable Support

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                try
                {

                    if (disposing)
                    {
                        if (_compositionHost != null)
                        {
                            _compositionHost.Dispose();

                            _compositionHost = null;
                        }
                    }
                }
                finally
                {
                    disposed = true;
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}
