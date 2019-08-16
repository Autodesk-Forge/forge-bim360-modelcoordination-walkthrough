using MCCommon;
using System;
using System.Composition;
using System.Threading.Tasks;

namespace MCConfig.Command
{
    [Export(typeof(IConsoleCommand))]
    internal sealed class DeleteCachedTokenCommand : CommandBase
    {
        private readonly IForgeAppConfigurationManager _configManager;

        [ImportingConstructor]
        public DeleteCachedTokenCommand(IForgeAppConfigurationManager configManager) => _configManager = configManager ?? throw new ArgumentNullException(nameof(configManager));

        public override string Display => "Delete cached Forge App auth token";

        public override uint Group => 2;

        public override uint Order => 3;

        public override async Task RunCommand()
        {
            await _configManager.DeleteCachedToken();
        }
    }
}
