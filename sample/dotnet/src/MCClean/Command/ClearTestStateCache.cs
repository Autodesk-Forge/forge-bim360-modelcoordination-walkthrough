using MCCommon;
using MCSample;
using System.Composition;
using System.Threading.Tasks;

namespace MCClean.Command
{
    [Export(typeof(IConsoleCommand))]
    internal sealed class ClearTestStateCache : CommandBase
    {
        public override string Display => "Clear cached test state";

        public override async Task RunCommand()
        {
            await Task.Run(() => SampleFileManager.ResetStateCache());
        }
    }
}
