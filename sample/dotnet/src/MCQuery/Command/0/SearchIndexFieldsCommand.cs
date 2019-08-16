using MCCommon;
using MCSample.Model;
using Newtonsoft.Json;
using System;
using System.Composition;
using System.Threading.Tasks;

namespace MCQuery.Command
{
    [Export(typeof(IConsoleCommand))]
    internal sealed class GetIndexFieldsCommand : CurrentStateCommand
    {
        private readonly IIndexClient _indexClient;

        public override string Display => "Search index fields";

        public override uint Order => 1;

        [ImportingConstructor]
        public GetIndexFieldsCommand(IIndexClient indexClient) => _indexClient = indexClient ?? throw new ArgumentNullException(nameof(indexClient));

        public override async Task DoInput()
        {
            await DoCurrentContainerModelSetInput();

            Console.Write("Search : ");
            Me.SearchText = Console.ReadLine();
        }

        public override async Task RunCommand()
        {
            var results = await _indexClient.SearchFields(Me.Container, Me.ModelSetId, (uint)Me.ModelSetVersion, Me.SearchText);

            foreach (var res in results)
            {
                Console.WriteLine(JsonConvert.SerializeObject(res));
            }
        }
    }
}
