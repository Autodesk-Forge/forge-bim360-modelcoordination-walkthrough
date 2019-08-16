using System.Windows;
using System.Windows.Controls;

namespace MCAuth
{
    /// <summary>
    /// Interaction logic for TokenManagerView.xaml
    /// </summary>
    public partial class TokenManagerView : Page
    {
        public TokenManagerView()
        {
            InitializeComponent();
        }

        private void OnCloseClick(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }
    }
}
