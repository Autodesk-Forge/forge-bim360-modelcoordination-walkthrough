using System.Threading.Tasks;

namespace MCCommon
{
    public interface ICosmoDbConfigurationManager
    {
        Task<CosmoDbConfiguration> Get();

        Task Set(CosmoDbConfiguration configuration);
    }
}
