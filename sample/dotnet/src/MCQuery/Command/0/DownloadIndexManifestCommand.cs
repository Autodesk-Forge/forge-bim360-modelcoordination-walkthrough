using MCCommon;
using MCSample;
using MCSample.Model;
using Newtonsoft.Json;
using System;
using System.Composition;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MCQuery.Command
{
    [Export(typeof(IConsoleCommand))]
    internal sealed class DownloadIndexManifestCommand : CommandBase
    {
        private readonly IForgeIndexClient _indexClient;

        private LastQueryState _lastQueryState;

        [ImportingConstructor]
        public DownloadIndexManifestCommand(IForgeIndexClient indexClient) => _indexClient = indexClient ?? throw new ArgumentNullException(nameof(indexClient));

        public override string Display => "Save index manifest for query to file";

        public override uint Order => 6;

        public override async Task DoInput()
        {
            _lastQueryState = (await SampleFileManager.LoadSavedState<LastQueryState>()) ??
                throw new InvalidOperationException("No cached query not found!");

            Me.OutputPath = SampleFileManager.NewStatePath("manifest.json");

            Console.Write($"Output path ({Me.OutputPath.FullName}) : ");
            var path = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(path))
            {
                Me.OutputPath = new FileInfo(path);
            }
        }

        public override async Task RunCommand()
        {
            var resFile = new FileInfo(_lastQueryState.ResultPath);

            if (!resFile.Exists)
            {
                throw new InvalidOperationException("Cached query result file not found!");
            }

            var manifest = await _indexClient.GetIndexManifest(_lastQueryState.Container, _lastQueryState.ModelSet, _lastQueryState.Verison);

            using (var fout = Me.OutputPath.Open(FileMode.Create))
            using (var sw = new StreamWriter(fout, Encoding.UTF8))
            {
                await sw.WriteAsync(JsonConvert.SerializeObject(manifest, Formatting.Indented));
            }
        }
    }
}
