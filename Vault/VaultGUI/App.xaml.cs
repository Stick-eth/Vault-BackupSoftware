using System.Configuration;
using System.Data;
using System.Windows;

namespace VaultGUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            if (!Utilities.SingleInstance.CheckAlreadyRunning())
            {
                MessageBox.Show("Another instance of VaultGUI is already running.", "VaultGUI", MessageBoxButton.OK, MessageBoxImage.Information);
                Shutdown();
                return;
            }
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Utilities.SingleInstance.ReleaseMutex();
            base.OnExit(e);
        }
    }

}
