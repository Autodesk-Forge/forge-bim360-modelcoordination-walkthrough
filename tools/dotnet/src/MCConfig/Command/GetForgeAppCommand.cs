using MCCommon;
using System;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;

namespace MCConfig.Command
{
    [Export(typeof(IConsoleCommand))]
    internal sealed class GetForgeAppCommand : CommandBase
    {
        private readonly IForgeAppConfigurationManager _configManager;

        [ImportingConstructor]
        public GetForgeAppCommand(IForgeAppConfigurationManager configManager) => _configManager = configManager ?? throw new ArgumentNullException(nameof(configManager));

        public override string Display => "Get Forge App Configuration";

        public override uint Group => 1;

        public override uint Order => 2;

        public override Task DoInput()
        {
            Console.Write("Environment (prod|stg|dev) : ");
            Me.Environment = Console.ReadLine();

            return base.DoInput();
        }
        public override async Task RunCommand()
        {
            ForgeAppConfiguration env = null;

            if (Me.Environment != null)
            {
                if (Me.Environment.Equals("dev", StringComparison.OrdinalIgnoreCase))
                {
                    env = await _configManager.GetEnvironmentConfiguration(ForgeEnvironment.Development);
                }
                else if (Me.Environment.Equals("stg", StringComparison.OrdinalIgnoreCase))
                {
                    env = await _configManager.GetEnvironmentConfiguration(ForgeEnvironment.Staging);
                }
                else if (Me.Environment.Equals("prod", StringComparison.OrdinalIgnoreCase))
                {
                    env = await _configManager.GetEnvironmentConfiguration(ForgeEnvironment.Production);
                }
                else
                {
                    throw new InvalidOperationException($"Environment {Me.Environment} not supported");
                }
            }

            if (env != null && !string.IsNullOrWhiteSpace(env.ClientId) && !string.IsNullOrWhiteSpace(env.Secret) && !string.IsNullOrWhiteSpace(env.CallbackUrl))
            {
                foreach (var prop in typeof(ForgeAppConfiguration).GetProperties().OrderBy(p => p.Name))
                {
                    var val = prop.GetValue(env);

                    if (val != null)
                    {
                        Console.WriteLine($"{prop.Name} : {val}");
                    }
                    else
                    {
                        Console.WriteLine($"{prop.Name} : null");
                    }
                }

                Console.WriteLine();
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine($"No App configured for environment {env.Environment}");

            }
        }
    }
}
