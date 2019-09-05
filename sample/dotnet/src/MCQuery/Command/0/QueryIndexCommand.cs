using MCCommon;
using MCSample;
using MCSample.Model;
using System;
using System.Composition;
using System.Text;
using System.Threading.Tasks;

namespace MCQuery.Command
{
    [Export(typeof(IConsoleCommand))]
    internal sealed class QueryIndexCommand : CurrentStateCommand
    {
        private readonly IForgeIndexClient _indexClient;

        [ImportingConstructor]
        public QueryIndexCommand(IForgeIndexClient indexClient) => _indexClient = indexClient ?? throw new ArgumentNullException(nameof(indexClient));

        public override string Display => "Run index query";

        public override uint Order => 2;

        public override async Task DoInput()
        {
            await DoCurrentContainerModelSetInput();

            Console.Write("Query : ");

            int bufferSize = 1024;

            using (var cin = Console.OpenStandardInput(bufferSize))
            {
                byte[] bytes = new byte[bufferSize];

                int read = cin.Read(bytes, 0, bufferSize);

                if (read > 0)
                {
                    Me.Query = Encoding.ASCII.GetString(bytes, 0, read);
                }
            }
        }

        public override async Task RunCommand()
        {
            var state = new LastQueryState
            {
                Container = Me.Container,
                ModelSet = Me.ModelSetId,
                Verison = (uint)Me.ModelSetVersion,
                Query = Me.Query
            };

            ResetStopwatch();
            StartStopwatch();
            var file = await _indexClient.QueryIndex(state.Container, state.ModelSet, state.Verison, state.Query);
            StopStopwatch();
            Console.WriteLine($"Success, gzip result set {file.Length} bytes in {ElapsedMilliseconds} ms");

            state.ResultPath = file.FullName;
            state.Success = true;
            state.Size = file.Length;
            state.ElapsedMilliseconds = ElapsedMilliseconds;

            await SampleFileManager.SaveState(state);
        }
    }
}
