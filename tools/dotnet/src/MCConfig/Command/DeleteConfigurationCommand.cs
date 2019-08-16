using MCCommon;
using System;
using System.Composition;
using System.Threading.Tasks;

namespace MCConfig.Command
{
    [Export(typeof(IConsoleCommand))]
    internal sealed class DeleteConfigurationCommand : CommandBase
    {
        private readonly IForgeAppConfigurationManager _configManager;

        [ImportingConstructor]
        public DeleteConfigurationCommand(IForgeAppConfigurationManager configManager) => _configManager = configManager ?? throw new ArgumentNullException(nameof(configManager));

        public override string Display => "Delete all local developer configuration";

        public override uint Group => 1;

        public override uint Order => 3;

        public override async Task RunCommand()
        {
            await Task.Run(() => _configManager.Reset());
        }
    }
}
