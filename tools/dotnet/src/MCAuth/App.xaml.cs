using System;
using System.Reflection;
using System.Windows;
using System.Windows.Navigation;

namespace MCAuth
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            this.StartupUri = new Uri("AppSelectorView.xaml", UriKind.Relative);
        }

        protected override void OnLoadCompleted(NavigationEventArgs e)
        {
            base.OnLoadCompleted(e);

            this.MainWindow.Height = 550;
            this.MainWindow.Width = 500;
            this.MainWindow.Title = $"MCAuth {Assembly.GetExecutingAssembly().GetName().Version} (c) Autodesk 2019";
        }

        private void OnExit(object sender, ExitEventArgs e)
        {
        }
    }
}
