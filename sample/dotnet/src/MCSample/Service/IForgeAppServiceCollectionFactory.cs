using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace MCSample.Service
{
    public interface IForgeAppServiceCollectionFactory
    {
        Task<ServiceProvider> CreateServiceProvider();
    }
}
