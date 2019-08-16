using MCCommon;
using MCSample;
using MCSample.Forge;
using Newtonsoft.Json;
using System;
using System.Composition;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MCQuery.Command
{
    [Export(typeof(IConsoleCommand))]
    internal sealed class DownloadModelDerivativeManifestCommand : CommandBase
    {
        private readonly static Regex IsBase64Encoded = new Regex(@"^[a-zA-Z0-9\+/]+={0,3}$", RegexOptions.Compiled);

        private readonly IForgeDerivativeClient _forgeClient;

        [ImportingConstructor]
        public DownloadModelDerivativeManifestCommand(IForgeDerivativeClient forgeClient) => _forgeClient = forgeClient ?? throw new ArgumentNullException(nameof(forgeClient));

        public override string Display => "Download model derivative manifest";

        public override uint Group => 1U;

        public override uint Order => 1;

        public override Task DoInput()
        {
            Console.Write("Manifest urn : ");
            string urn = Console.ReadLine().Trim();

            if (IsBase64Encoded.IsMatch(urn))
            {
                Me.Urn = urn;
            }
            else
            {
                Me.Urn = Convert.ToBase64String(Encoding.UTF8.GetBytes(urn));
            }

            Me.OutputPath = SampleFileManager.NewStatePath("derivativeManifest.json");

            Console.Write($"Output path ({Me.OutputPath.FullName}) : ");
            var path = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(path))
            {
                Me.OutputPath = new FileInfo(path);
            }

            return base.DoInput();
        }

        public override async Task RunCommand()
        {
            var manifest = await _forgeClient.GetDerivativeManifest(Me.Urn);

            using (var fout = Me.OutputPath.Open(FileMode.Create))
            using (var sw = new StreamWriter(fout, Encoding.UTF8))
            {
                await sw.WriteAsync(JsonConvert.SerializeObject(manifest, Formatting.Indented));
            }
        }
    }
}
