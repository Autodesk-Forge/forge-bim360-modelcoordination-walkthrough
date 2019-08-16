using System.IO;
using System.Threading.Tasks;

namespace MCCommon
{
    public interface IForgeAppConfigurationManager
    {
        DirectoryInfo ConfigDirectory { get; }

        Task<ForgeAppConfiguration> GetEnvironmentConfiguration(ForgeEnvironment environment);

        Task SaveEnvironmentConfiguration(ForgeApp configuration);

        Task<ForgeAppConfiguration[]> GetCurrentConfiguraitons();

        Task<string> GetCachedToken();

        Task DeleteCachedToken();

        Task CacheToken(string token);

        Task SetDefaultEnvironment(ForgeEnvironment environment);

        Task<ForgeAppConfiguration> GetDefaultConfiguration();

        void Reset();
    }
}
