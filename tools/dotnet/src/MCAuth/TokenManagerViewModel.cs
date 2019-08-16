using MCAuth.Auth;
using MCCommon;
using System;
using System.Composition;
using System.Windows;
using UXCommon;

namespace MCAuth
{
    [Export]
    internal class TokenManagerViewModel : ChangeNotifier
    {
        private readonly ITokenManager _tokenManager;
        private readonly IForgeAppConfigurationManager _configurationManager;

        private bool _commandsEnabled;
        private string _type;
        private int _expiresIn;
        private DateTime _expiresOn;
        private string _refresh;
        private string _accessToken;
        private bool _withAuthorizePrefix;

        [ImportingConstructor]
        public TokenManagerViewModel(ITokenManager tokenManager, IForgeAppConfigurationManager configurationManager)
        {
            _tokenManager = tokenManager ?? throw new ArgumentNullException(nameof(tokenManager));

            _configurationManager = configurationManager ?? throw new ArgumentNullException(nameof(configurationManager));
        }

        public bool CommandsEnabled
        {
            get
            {
                return _commandsEnabled;
            }

            set
            {
                _commandsEnabled = value;

                RaisePropertyChanged();
            }
        }

        public string Type
        {
            get
            {
                return _type;
            }

            set
            {
                _type = value;

                RaisePropertyChanged();
            }
        }

        public int ExpiresIn
        {
            get
            {
                return _expiresIn;
            }

            set
            {
                _expiresIn = value;

                RaisePropertyChanged();
            }
        }

        public DateTime ExpiresOn
        {
            get
            {
                return _expiresOn;
            }

            set
            {
                _expiresOn = value;

                RaisePropertyChanged();
            }
        }

        public string Refresh
        {
            get
            {
                return _refresh;
            }

            set
            {
                _refresh = value;

                RaisePropertyChanged();
            }
        }

        public string AccessToken
        {
            get
            {
                return _accessToken;
            }

            set
            {
                _accessToken = value;

                RaisePropertyChanged();
            }
        }

        public bool WithAuthorizePrefix
        {
            get
            {
                return _withAuthorizePrefix;
            }

            set
            {
                _withAuthorizePrefix = value;

                RaisePropertyChanged();
            }
        }

        public IAsyncCommand CopyCommand => AsyncCommand.Create(async () =>
        {
            try
            {
                CommandsEnabled = false;

                if (WithAuthorizePrefix)
                {
                    await App.Current.Dispatcher.InvokeAsync(() => Clipboard.SetText($"{Type} {AccessToken}"));
                }
                else
                {
                    await App.Current.Dispatcher.InvokeAsync(() => Clipboard.SetText(AccessToken));
                }
            }
            finally
            {
                CommandsEnabled = true;
            }
        });

        public IAsyncCommand SaveCommand => AsyncCommand.Create(async () =>
        {
            try
            {
                CommandsEnabled = false;

                await _configurationManager.CacheToken(AccessToken);
            }
            finally
            {
                CommandsEnabled = true;
            }
        });

        public IAsyncCommand RefreshCommand => AsyncCommand.CreateWithParameter(async (state) =>
        {
            try
            {
                CommandsEnabled = false;

                bool refresh = (bool)state;

                var token = await _tokenManager.GetAccessToken(forceRefresh: refresh);

                Type = token.Type;
                ExpiresIn = token.ExpiresIn;
                ExpiresOn = token.ExpiresOn;
                AccessToken = token.AccessToken;
                Refresh = token.Refresh;
            }
            finally
            {
                CommandsEnabled = true;
            }
        });
    }
}
