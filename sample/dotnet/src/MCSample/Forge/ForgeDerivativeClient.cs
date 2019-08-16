using Autodesk.Forge;
using MCCommon;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Composition;
using System.IO;
using System.Threading.Tasks;

namespace MCSample.Forge
{
    [Export(typeof(IForgeDerivativeClient))]
    internal sealed class ForgeDerivativeClient : ForgeClientBase, IForgeDerivativeClient
    {
        [ImportingConstructor]
        public ForgeDerivativeClient(IForgeAppConfigurationManager configurationManager)
            : base(configurationManager)
        {
        }

        public async Task<dynamic> GetDerivativeManifest(string urn)
        {
            var api = await NewForgeApi<DerivativesApi>();

            var manifestResponse = await api.GetManifestAsyncWithHttpInfo(urn);

            dynamic manifest = null;

            if (manifestResponse.StatusCode == 200)
            {
                manifest = JObject.Parse(JsonConvert.SerializeObject(manifestResponse.Data));
            }

            return manifest;
        }

        public async Task GetDerivative(string urn, string derivativeUrn, Func<Stream, Task> streamProcessor)
        {
            var uri = Configuration.ModelDerivativePath($"{urn}/manifest/{derivativeUrn}");

            using (var httpStream = await uri.OpenHttpStream(await CreateDefaultHttpRequestHeaders()))
            {
                await streamProcessor(httpStream);
            }
        }
    }
}
