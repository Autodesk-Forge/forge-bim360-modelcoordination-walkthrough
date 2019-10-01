using MCCommon;
using MCSample.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Composition;
using System.Threading.Tasks;

namespace MCQuery.Command
{
    [Export(typeof(IConsoleCommand))]
    internal sealed class GetIndexFieldsCommand : CurrentStateCommand
    {
        private readonly IForgeIndexClient _indexClient;

        public override string Display => "Search index fields";

        public override uint Order => 1;

        [ImportingConstructor]
        public GetIndexFieldsCommand(IForgeIndexClient indexClient) => _indexClient = indexClient ?? throw new ArgumentNullException(nameof(indexClient));

        public override async Task DoInput()
        {
            await DoCurrentContainerModelSetInput();

            Console.Write("Search : ");
            Me.SearchText = Console.ReadLine();
        }

        public override async Task RunCommand()
        {
            var results = await _indexClient.SearchFields(Me.Container, Me.ModelSetId, (uint)Me.ModelSetVersion, Me.SearchText);

            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new StringEnumConverter());

            foreach (var res in results)
            {
                Console.WriteLine(JsonConvert.SerializeObject(res, settings));
            }
        }
    }
}
