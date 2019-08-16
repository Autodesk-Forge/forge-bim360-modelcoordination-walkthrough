using MCCommon;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace MCAuth
{
    /// <summary>
    /// Interaction logic for SignInView.xaml
    /// </summary>
    public partial class SignInView : Page
    {
        public SignInView()
        {
            InitializeComponent();
        }

        private async void OnWebBrowserNavigating(object sender, NavigatingCancelEventArgs e)
        {
            if (e.Uri.AbsoluteUri.StartsWith(ForgeAppConfiguration.Current.CallbackUrl))
            {
                this.Visibility = Visibility.Collapsed;

                e.Cancel = true;

                var code = e.Uri.Query.Split('=')[1];

                await ((SignInViewModel)DataContext).SignInSuccess(code);
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            ((SignInViewModel)DataContext).NavigationService = NavigationService;

            this.Browser.Navigate(ForgeAppConfiguration.Current.AuthorizeUrl);
        }
    }
}
