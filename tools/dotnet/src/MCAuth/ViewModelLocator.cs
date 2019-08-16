using MCCommon;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Composition.Hosting;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace MCAuth
{
    internal sealed class ViewModelLocator : DynamicObject
    {
        private readonly Lazy<Dictionary<string, Type>> LazyExportableTypes = new Lazy<Dictionary<string, Type>>(() =>
            Assembly.GetExecutingAssembly()
                    .GetTypes()
                    .Where(t => t.GetCustomAttributes(typeof(ExportAttribute), false).SingleOrDefault() != null)
                    .ToDictionary(t => t.Name, StringComparer.OrdinalIgnoreCase));

        private readonly Lazy<CompositionHost> LazyContainer = new Lazy<CompositionHost>(() =>
            new ContainerConfiguration()
                .WithAssembly(Assembly.GetExecutingAssembly())
                .AddModelCoordinationCommon()
                .CreateContainer());

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = null;

            try
            {
                result = LazyExportableTypes.Value.ContainsKey(binder.Name) ? LazyContainer.Value.GetExport(LazyExportableTypes.Value[binder.Name]) : null;
            }
            catch (CompositionFailedException)
            {
                try
                {
                    result = LazyExportableTypes.Value.ContainsKey(binder.Name) ? LazyContainer.Value.GetExport(LazyExportableTypes.Value[binder.Name], binder.Name) : null;
                }
                catch (CompositionFailedException)
                {
                }
            }

            return result != null;
        }
    }
}
