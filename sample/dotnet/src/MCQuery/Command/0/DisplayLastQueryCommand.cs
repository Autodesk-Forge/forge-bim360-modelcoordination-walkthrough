using MCCommon;
using MCSample;
using System;
using System.Composition;
using System.IO;
using System.Threading.Tasks;

namespace MCQuery.Command
{
    [Export(typeof(IConsoleCommand))]
    internal sealed class DisplayLastQueryCommand : CommandBase
    {
        public override string Display => "Display last query details";

        public override uint Order => 7;

        public override async Task DoInput()
        {
            Me.State = await SampleFileManager.LoadSavedState<LastQueryState>();
        }

        public override Task RunCommand()
        {
            LastQueryState state = Me.State;

            if (state == null)
            {
                throw new InvalidOperationException("No cached query not found!");
            }

            var resFile = new FileInfo(state.ResultPath);

            if (!resFile.Exists)
            {
                throw new InvalidOperationException("Cached query result file not found!");
            }

            Console.WriteLine($"Container               : {state.Container}");
            Console.WriteLine($"Model Set               : {state.ModelSet}");
            Console.WriteLine($"Version                 : {state.Verison}");
            Console.WriteLine($"Query                   : {state.Query}");
            Console.WriteLine($"Success                 : {state.Success}");
            Console.WriteLine($"Compressed size (bytes) : {resFile.Length}");

            return Task.FromResult(true);
        }
    }
}
