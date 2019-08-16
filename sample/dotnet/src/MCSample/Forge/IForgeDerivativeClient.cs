using System;
using System.IO;
using System.Threading.Tasks;

namespace MCSample.Forge
{
    public interface IForgeDerivativeClient : IForgeClient
    {
        Task<dynamic> GetDerivativeManifest(string urn);

        Task GetDerivative(string urn, string derivativeUrn, Func<Stream, Task> streamProcessor);
    }
}
