using MCCommon;
using System;
using System.Windows;
using System.Windows.Controls;

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

        private void On_Loaded(object sender, RoutedEventArgs e)
        {
            ((SignInViewModel)DataContext).NavigationService = NavigationService;

            NavigationService.RemoveBackEntry();
        }

        private async void Browser_FrameLoadEnd(object sender, CefSharp.FrameLoadEndEventArgs e)
        {
            if (e.Url.StartsWith(ForgeAppConfiguration.Current.CallbackUrl))
            {
                await Dispatcher.InvokeAsync(async () =>
                {
                    this.Visibility = Visibility.Collapsed;

                    var code = new Uri(e.Url, UriKind.Absolute).Query.Split('=')[1];

                    await ((SignInViewModel)DataContext).SignInSuccess(code);
                });
            }
        }
    }
}
