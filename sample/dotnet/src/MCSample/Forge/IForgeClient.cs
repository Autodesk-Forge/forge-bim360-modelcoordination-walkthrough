using MCCommon;
using System.Threading.Tasks;

namespace MCSample.Forge
{
    public interface IForgeClient
    {
        ForgeAppConfiguration Configuration { get; }

        Task<string> GetToken();
    }
}
