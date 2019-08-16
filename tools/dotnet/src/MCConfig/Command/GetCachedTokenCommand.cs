using MCCommon;
using System;
using System.Composition;
using System.Threading.Tasks;

namespace MCConfig.Command
{
    [Export(typeof(IConsoleCommand))]
    internal sealed class GetCachedTokenCommand : CommandBase
    {
        private readonly IForgeAppConfigurationManager _configManager;

        [ImportingConstructor]
        public GetCachedTokenCommand(IForgeAppConfigurationManager configManager) => _configManager = configManager ?? throw new ArgumentNullException(nameof(configManager));

        public override string Display => "Get cached Forge App auth token";

        public override uint Group => 2;

        public override uint Order => 2;

        public override async Task RunCommand()
        {
            var token = await _configManager.GetCachedToken();

            if (!string.IsNullOrWhiteSpace(token))
            {
                Console.WriteLine(token);
            }
        }
    }
}
