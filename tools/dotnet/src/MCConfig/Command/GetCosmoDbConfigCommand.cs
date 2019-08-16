using MCCommon;
using System;
using System.Composition;
using System.Threading.Tasks;

namespace MCConfig.Command
{
    [Export(typeof(IConsoleCommand))]
    internal sealed class GetCosmoDbConfigCommand : CommandBase
    {
        private readonly ICosmoDbConfigurationManager _configManager;

        [ImportingConstructor]
        public GetCosmoDbConfigCommand(ICosmoDbConfigurationManager configManager) => _configManager = configManager ?? throw new ArgumentNullException(nameof(configManager));

        public override string Display => "Get Azure Cosmo DB Account";

        public override uint Group => 3;

        public override uint Order => 2;

        public override async Task RunCommand()
        {
            var config = await _configManager.Get();

            if (config == null)
            {
                Console.WriteLine("Cosmo not configured!");
            }
            else
            {
                Console.WriteLine($"Endpoint   : {config.AccountEndpoint.OriginalString}");
                Console.WriteLine($"PrimaryKey : {config.AccountPrimaryKey}");
            }
        }
    }
}
