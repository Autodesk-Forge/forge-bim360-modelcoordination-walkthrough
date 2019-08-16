using MCCommon;
using System;
using System.Composition;
using System.Threading.Tasks;

namespace MCConfig.Command
{
    [Export(typeof(IConsoleCommand))]
    internal sealed class SetForgeAppCommand : CommandBase
    {
        private readonly IForgeAppConfigurationManager _configManager;

        [ImportingConstructor]
        public SetForgeAppCommand(IForgeAppConfigurationManager configManager) => _configManager = configManager ?? throw new ArgumentNullException(nameof(configManager));

        public override string Display => "Set Forge App Configuration";

        public override uint Group => 1;

        public override uint Order => 1;

        public override async Task DoInput()
        {
            ForgeAppConfiguration env = await LoadCurrentSettings();

            GetClient(env);

            GetSecret(env);

            GetCallback(env);

            GetAccount(env);

            GetProject(env);

            env.ClientId = Me.Client;
            env.CallbackUrl = Me.Callback;
            env.Secret = Me.Secret;
            env.Account = Me.Account;
            env.Project = Me.Project;

            Me.AppConfig = env;
        }

        public override async Task RunCommand()
        {
            await _configManager.SaveEnvironmentConfiguration(Me.AppConfig);
        }

        private void GetAccount(ForgeAppConfiguration currentEnvironment)
        {
            if (currentEnvironment.Account != Guid.Empty)
            {
                Console.Write($"Optional, Selected Account ({currentEnvironment.Account}) : ");
            }
            else
            {
                Console.Write("Optional, Selected Account : ");
            }

            var tmp = Console.ReadLine();

            if (Guid.TryParse(tmp, out Guid output))
            {
                Me.Account = output;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(tmp) && currentEnvironment.Account != Guid.Empty)
                {
                    Me.Account = currentEnvironment.Account;
                }
                else
                {
                    Me.Account = Guid.Empty;
                }
            }
        }

        private void GetProject(ForgeAppConfiguration currentEnvironment)
        {
            if (currentEnvironment.Project != Guid.Empty)
            {
                Console.Write($"Optional, Selected Project ({currentEnvironment.Project}) : ");
            }
            else
            {
                Console.Write("Optional, Selected Project : ");
            }

            var tmp = Console.ReadLine();

            if (Guid.TryParse(tmp, out Guid output))
            {
                Me.Project = output;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(tmp) && currentEnvironment.Account != Guid.Empty)
                {
                    Me.Project = currentEnvironment.Project;
                }
                else
                {
                    Me.Project = Guid.Empty;
                }
            }
        }

        private void GetCallback(ForgeAppConfiguration currentEnvironment)
        {
            if (!string.IsNullOrWhiteSpace(currentEnvironment.CallbackUrl))
            {
                Console.Write($"Callback URL ({currentEnvironment.CallbackUrl}) : ");
            }
            else
            {
                Console.Write("Callback URL : ");
            }

            Me.Callback = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(Me.Callback) && !string.IsNullOrWhiteSpace(currentEnvironment.CallbackUrl))
            {
                Me.Callback = currentEnvironment.CallbackUrl;
            }
        }

        private void GetSecret(ForgeAppConfiguration currentEnvironment)
        {
            if (!string.IsNullOrWhiteSpace(currentEnvironment.Secret))
            {
                Console.Write($"Secret ({currentEnvironment.Secret}) : ");
            }
            else
            {
                Console.Write("Secret : ");
            }

            Me.Secret = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(Me.Secret) && !string.IsNullOrWhiteSpace(currentEnvironment.Secret))
            {
                Me.Secret = currentEnvironment.Secret;
            }
        }

        private void GetClient(ForgeAppConfiguration currentEnvironment)
        {
            if (!string.IsNullOrWhiteSpace(currentEnvironment.ClientId))
            {
                Console.Write($"Client ID ({currentEnvironment.ClientId}) : ");
            }
            else
            {
                Console.Write("Client ID : ");
            }

            Me.Client = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(Me.Client) && !string.IsNullOrWhiteSpace(currentEnvironment.ClientId))
            {
                Me.Client = currentEnvironment.ClientId;
            }
        }

        private async Task<ForgeAppConfiguration> LoadCurrentSettings()
        {
            Console.Write("Environment (prod|stg|dev) : ");
            Me.Environment = Console.ReadLine();

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

            return env;
        }
    }
}
