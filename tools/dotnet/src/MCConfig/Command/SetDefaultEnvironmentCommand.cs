using MCCommon;
using System;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;

namespace MCConfig.Command
{
    [Export(typeof(IConsoleCommand))]
    internal sealed class SetDefaultEnvironmentCommand : CommandBase
    {
        private readonly IForgeAppConfigurationManager _configManager;

        [ImportingConstructor]
        public SetDefaultEnvironmentCommand(IForgeAppConfigurationManager configManager) => _configManager = configManager ?? throw new ArgumentNullException(nameof(configManager));

        public override string Display => "Set default Forge App configuration";

        public override uint Group => 1;

        public override uint Order => 4;

        public override async Task DoInput()
        {
            var configurations = await _configManager.GetCurrentConfiguraitons();

            var currentDef = configurations.SingleOrDefault(c => c.IsDefault);

            if (currentDef != null)
            {
                string env = null;

                switch (currentDef.Environment)
                {
                    case ForgeEnvironment.Development:
                        env = "dev";
                        break;

                    case ForgeEnvironment.Staging:
                        env = "stg";
                        break;

                    case ForgeEnvironment.Production:
                        env = "prod";
                        break;
                }

                Console.Write($"Default Environment (prod|stg|dev), current ({env}) : ");
            }
            else
            {
                Console.Write("Default Environment (prod|stg|dev) : ");
            }

            Me.Environment = Console.ReadLine();
        }

        public override async Task RunCommand()
        {
            if (Me.Environment != null)
            {
                if (Me.Environment.Equals("dev", StringComparison.OrdinalIgnoreCase))
                {
                    await _configManager.SetDefaultEnvironment(ForgeEnvironment.Development);
                }
                else if (Me.Environment.Equals("stg", StringComparison.OrdinalIgnoreCase))
                {
                    await _configManager.SetDefaultEnvironment(ForgeEnvironment.Staging);
                }
                else if (Me.Environment.Equals("prod", StringComparison.OrdinalIgnoreCase))
                {
                    await _configManager.SetDefaultEnvironment(ForgeEnvironment.Production);
                }
                else
                {
                    throw new InvalidOperationException($"Environment {Me.Environment} not supported");
                }
            }
        }
    }
}
