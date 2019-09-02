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
    internal sealed class DownloadModelSetVersionCommand : CurrentStateCommand
    {
        private readonly IForgeModelSetClient _modelSetClient;

        [ImportingConstructor]
        public DownloadModelSetVersionCommand(IForgeModelSetClient modelSetClient) => _modelSetClient = modelSetClient ?? throw new ArgumentNullException(nameof(modelSetClient));

        public override string Display => "Download model set version";

        public override uint Group => 2U;

        public override uint Order => 1;

        public override async Task DoInput()
        {
            await DoCurrentContainerModelSetInput();

            Me.OutputPath = SampleFileManager.NewStatePath("modelset.json");

            Console.Write($"Output path ({Me.OutputPath.FullName}) : ");
            var path = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(path))
            {
                Me.OutputPath = new FileInfo(path);
            }
        }

        public override async Task RunCommand()
        {
            Guid container = Me.Container;
            Guid modelSet = Me.ModelSetId;
            uint version = Me.ModelSetVersion;

            var modelSetVersion = await _modelSetClient.GetModelSetVersion(container, modelSet, version);

            using (var fout = Me.OutputPath.Open(FileMode.Create))
            using (var sw = new StreamWriter(fout, Encoding.UTF8))
            {
                await sw.WriteAsync(JsonConvert.SerializeObject(modelSetVersion, Formatting.Indented));
            }
        }
    }
}
