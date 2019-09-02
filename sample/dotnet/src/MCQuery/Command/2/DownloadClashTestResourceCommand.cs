using Autodesk.Forge.Bim360.ModelCoordination.Clash;
using MCCommon;
using MCSample;
using MCSample.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Composition;
using System.IO;
using System.Threading.Tasks;

namespace MCQuery.Command
{
    [Export(typeof(IConsoleCommand))]
    internal sealed class DownloadClashTestResourceCommand : CurrentStateCommand
    {
        private readonly IForgeClashClient _clashClient;

        [ImportingConstructor]
        public DownloadClashTestResourceCommand(IForgeClashClient clashClient) => _clashClient = clashClient ?? throw new ArgumentNullException(nameof(clashClient));

        public override string Display => "Download clash test resources";

        public override uint Group => 2U;

        public override uint Order => 2;

        public override async Task DoInput()
        {
            await DoCurrentContainerClashTestInput();

            var resources = await _clashClient.GetModelSetClashTestResources(Me.Container, Me.ClashTestId);

            Console.WriteLine();

            for (int i = 0; i < resources.Resources.Count; i++)
            {
                Console.WriteLine($"  {i + 1} {resources.Resources[i].Type}");
            }

            Console.WriteLine();
            Console.Write("Select : ");

            var choice = int.Parse(Console.ReadLine()) - 1;

            if (choice < 0 || choice >= resources.Resources.Count)
            {
                throw new IndexOutOfRangeException();
            }

            Me.Resource = resources.Resources[choice];

            Console.Write($"Convert to CSV (y/n) : ");
            Me.ToCsv = Yes.Equals(Console.ReadLine(), StringComparison.OrdinalIgnoreCase);

            if (Me.ToCsv)
            {
                Me.OutputPath = SampleFileManager.NewStatePath($"{resources.Resources[choice].Type}.csv");
            }
            else
            {
                Me.OutputPath = SampleFileManager.NewStatePath($"{resources.Resources[choice].Type}.json.gz");
            }

            Console.Write($"Output path ({Me.OutputPath.FullName}) : ");
            var path = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(path))
            {
                Me.OutputPath = new FileInfo(path);
            }
        }

        public override async Task RunCommand()
        {
            ClashTestResource resource = Me.Resource;

            if (!Me.ToCsv)
            {
                await _clashClient.DownloadClashTestResource(resource, Me.OutputPath);
            }
            else
            {
                var tmp = SampleFileManager.NewStatePath(Guid.NewGuid().ToString());

                try
                {
                    await _clashClient.DownloadClashTestResource(resource, tmp);

                    var reader = new ClashResultReader<JObject>(tmp, true);

                    var exporter = new ClashNdJsonCsvExporter(Me.OutputPath);

                    await exporter.Export(reader);
                }
                finally
                {
                    tmp.Refresh();

                    if (tmp.Exists)
                    {
                        tmp.Delete();
                    }
                }
            }
        }
    }
}
