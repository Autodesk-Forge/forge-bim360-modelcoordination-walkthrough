using MCCommon;
using System;
using System.Composition;
using System.Text;
using System.Threading.Tasks;

namespace MCConfig.Command
{
    [Export(typeof(IConsoleCommand))]
    internal sealed class SetCachedTokenCommand : CommandBase
    {
        private readonly IForgeAppConfigurationManager _configManager;

        [ImportingConstructor]
        public SetCachedTokenCommand(IForgeAppConfigurationManager configManager) => _configManager = configManager ?? throw new ArgumentNullException(nameof(configManager));

        public override string Display => "Set cached Forge App auth token";

        public override uint Group => 2;

        public override uint Order => 1;

        public override Task DoInput()
        {
            Console.Write("Token : ");

            int bufferSize = 512;

            using (var cin = Console.OpenStandardInput(bufferSize))
            {
                byte[] bytes = new byte[bufferSize];

                int read = cin.Read(bytes, 0, bufferSize);

                if (read > 0)
                {
                    Me.Token = Encoding.ASCII.GetString(bytes, 0, read);
                }
            }

            return base.DoInput();
        }

        public override async Task RunCommand()
        {
            if (!string.IsNullOrWhiteSpace(Me.Token))
            {
                await _configManager.CacheToken(Me.Token.Trim());
            }
        }
    }
}
