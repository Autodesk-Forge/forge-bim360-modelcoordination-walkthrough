using MCCommon;
using System;
using System.Collections.ObjectModel;
using System.Composition;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using UXCommon;

namespace MCAuth
{
    [Export]
    internal sealed class AppSelectorViewModel : ChangeNotifier
    {
        private readonly IForgeAppConfigurationManager _forgeConfigManager;

        private ForgeAppConfiguration _selected;

        private NavigationService _navigationService;

        [ImportingConstructor]
        public AppSelectorViewModel(IForgeAppConfigurationManager forgeConfigManager)
        {
            _forgeConfigManager = forgeConfigManager ?? throw new ArgumentNullException(nameof(forgeConfigManager));
        }

        public ObservableCollection<ForgeAppConfiguration> AppConfigurations { get; } = new ObservableCollection<ForgeAppConfiguration>();

        public ForgeAppConfiguration Selected
        {
            get
            {
                return _selected;
            }

            set
            {
                _selected = value;

                if (_selected != null)
                {
                    ForgeAppConfiguration.Current = _selected;
                }

                RaisePropertyChanged();
            }
        }

        public IAsyncCommand SignInCommand => AsyncCommand.Create(async () =>
        {
            await Application.Current.Dispatcher.InvokeAsync(() => _navigationService.Navigate(new SignInView()));
        });

        public IAsyncCommand RefreshCommand => AsyncCommand.CreateWithParameter(async (state) =>
        {
            AppConfigurations.Clear();

            _navigationService = ((Page)state).NavigationService;

            var current = await _forgeConfigManager.GetCurrentConfiguraitons();

            foreach (var config in current)
            {
                if (!string.IsNullOrWhiteSpace(config.ClientId) &&
                    !string.IsNullOrWhiteSpace(config.Secret) &&
                    !string.IsNullOrWhiteSpace(config.CallbackUrl))
                {
                    AppConfigurations.Add(config);
                }
            }
        });
    }
}
