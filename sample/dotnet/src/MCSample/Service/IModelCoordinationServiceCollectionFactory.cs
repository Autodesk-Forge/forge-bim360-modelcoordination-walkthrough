using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace MCSample.Service
{
    public interface IModelCoordinationServiceCollectionFactory
    {
        Task<ServiceProvider> CreateServiceProvider();
    }
}
