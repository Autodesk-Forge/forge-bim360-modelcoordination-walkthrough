using MCCommon;
using System;
using System.Composition;
using System.Threading.Tasks;

namespace MCConfig.Command
{
    [Export(typeof(IConsoleCommand))]
    internal sealed class ConfigureCosmoDbCommand : CommandBase
    {
        private readonly ICosmoDbConfigurationManager _configManager;

        [ImportingConstructor]
        public ConfigureCosmoDbCommand(ICosmoDbConfigurationManager configManager) => _configManager = configManager ?? throw new ArgumentNullException(nameof(configManager));

        public override string Display => "Configure Azure Cosmo DB Account";

        public override uint Group => 3;

        public override uint Order => 1;

        public override async Task DoInput()
        {
            var current = await _configManager.Get();

            GetSetExistingValue(
                 () => current != null && current.AccountEndpoint != null,
                 () => current.AccountEndpoint.OriginalString,
                 "Endpoint",
                 value => Me.AccountEndpoint = new Uri((string)value),
                 value => Me.AccountEndpoint = new Uri(value));

            GetSetExistingValue(
                 () => current != null && !string.IsNullOrWhiteSpace(current.AccountPrimaryKey),
                 () => current.AccountPrimaryKey,
                 "PrimaryKey",
                 value => Me.AccountPrimaryKey = value,
                 value => Me.AccountPrimaryKey = value ?? throw new InvalidOperationException("Primary key cannot be null!"));
        }

        public override async Task RunCommand()
        {
            await _configManager.Set(
                new CosmoDbConfiguration
                {
                    AccountEndpoint = Me.AccountEndpoint,
                    AccountPrimaryKey = Me.AccountPrimaryKey
                });
        }
    }
}
