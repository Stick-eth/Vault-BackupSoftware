using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Vault.ViewModel;

namespace VaultGUI.View
{
    /// <summary>
    /// Logique d'interaction pour Settings.xaml
    /// </summary>
    public partial class Settings : UserControl
    {

        OpenFolderDialog dialog = new OpenFolderDialog()
        {
            Title = "Select Folder",
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal),
            Multiselect = false
        };

        private int lang;
        private string Logs;
        private List<string> monitoredApplications;
        //private MainView MainView;


        private ViewModelCore ViewModelCore;

        public Settings()
        {

            InitializeComponent();
            ViewModelCore = ViewModelCore.GetInstance();
            if (ViewModelCore.GetLanguage() == "fr-FR")
            {
                LangFrench.IsChecked = true;
            }
            else
            {
                LangEnglish.IsChecked = true;
            }

            //recuperer le chemin des logs en attribut
            if (ViewModelCore.GetLogPath() != "")
            {
                FolderLog.Text = ViewModelCore.GetLogPath();
            }
            
            //recuperer les applications monitorées
            if (ViewModelCore.GetMonitoredProcess() != null)
            {
                foreach (var item in ViewModelCore.GetMonitoredProcess())
                {
                    MonitoredApplications.Items.Add(item);
                }
            }
            if (ViewModelCore.IsRunning())
            {
                LaunchServerButton.IsEnabled = false;
                LaunchServerButton.Content = "Server Launched";
            }
            FillText();
            ServerMode.IsChecked = true;
            LaunchServerButton.Visibility = Visibility.Visible;
            ClientDetailsPanel.Visibility = Visibility.Collapsed;
            ServerIP.Text = "127.0.0.1";
            ServerPort.Text = "12324";



        }

        private void TargetFolderLog_Click(object sender, RoutedEventArgs e)
        {
            if (dialog.ShowDialog() == true)
            {
                FolderLog.Text = dialog.FolderName;
            }
        }


        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModelCore = ViewModelCore.GetInstance();
            SetLang();
            SetLog();
            SetMonitoredApplications();
            FillText();
            MainView mainView = MainView.GetInstance();
            mainView.FillText();
            


        }

        private void LangEnglish_Checked(object sender, RoutedEventArgs e)
        {
            lang = 2;

        }

        private void LangFrench_Checked(object sender, RoutedEventArgs e)
        {
            lang = 1;
        }

        private void SetLang()
        {
            try
            {
                if (lang != 0)
                {
                    if (lang == 1)
                    {
                        ViewModelCore.SetLanguage("Français");
                    }
                    else
                    {
                        ViewModelCore.SetLanguage("English");
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }


        private void SetLog()
        {
            if (FolderLog.Text != "")
            {
                Logs = FolderLog.Text.Replace("\\", "/");
                ViewModelCore.SetLogPath(Logs);
            }
        }

        private void SetMonitoredApplications()
        {
            List<string> list = new List<string>();
            foreach (var item in MonitoredApplications.Items)
            {
                list.Add(item.ToString());
            }
            ViewModelCore.SetMonitoredProcess(list);

        }

        private void TargetFolderApp_Click(object sender, RoutedEventArgs e)
        {
            if (dialog.ShowDialog() == true)
            {
 
            }

        }

        private void RemoveFromList_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MonitoredApplications.Items.RemoveAt(MonitoredApplications.SelectedIndex);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void AddToList_Click(object sender, RoutedEventArgs e)
        {
            if(NewMonitoredApp.Text == "")
            {
                return;
            }
            if (MonitoredApplications.Items.Contains(NewMonitoredApp.Text))
            {
                return;
            }
            MonitoredApplications.Items.Add(NewMonitoredApp.Text);
        }

        private void ServerMode_Checked(object sender, RoutedEventArgs e)
        {
            LaunchServerButton.Visibility = Visibility.Visible;
            ClientDetailsPanel.Visibility = Visibility.Collapsed;
        }

        private void ClientMode_Checked(object sender, RoutedEventArgs e)
        {
            LaunchServerButton.Visibility = Visibility.Collapsed;
            ClientDetailsPanel.Visibility = Visibility.Visible;
        }



        private void LaunchServerButton_Click(object sender, RoutedEventArgs e)
        {

            ViewModelCore = ViewModelCore.GetInstance();
            ViewModelCore.StartServer();
            LaunchServerButton.IsEnabled = false;
            LaunchServerButton.Content = "Server Launched";
        }


        private void FillText()
        {
            SettingsTitle.Text = MainView.rm.GetString("SettingsTitle");
            LangTitle.Text = MainView.rm.GetString("SettingsLangTitle");
            LogPathTitle.Text = MainView.rm.GetString("SettingsLogPathTitle");
            BusinessAppTitle.Text = MainView.rm.GetString("SettingsBuisnessAppTitle");
            SelectedApps.Text = MainView.rm.GetString("SettingsSelectedApps");
            AddApp.Text = MainView.rm.GetString("SettingsAddapp");

            TargetFolderLog.Content = MainView.rm.GetString("SelectFolder");
            RemoveFromList.Content = MainView.rm.GetString("RemoveFromList");
            AddToList.Content = MainView.rm.GetString("AddToList");
            SaveButton.Content = MainView.rm.GetString("SaveButton");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ConnectToServerButton_Click(object sender, RoutedEventArgs e)
        {            
            
            string serverIp = ServerIP.Text;
            string serverPort = ServerPort.Text;
            VaultGUI_Client.MainWindow mainWindow = new VaultGUI_Client.MainWindow(serverIp,serverPort);
            mainWindow.Show();
            Window.GetWindow(this).Close();
            /*            RemoteViewModel remoteViewModel = RemoteViewModel.Instance;
                        RemoteViewModel.Ip = "127.0.0.1";
                        RemoteViewModel.Port = 12324;
                        Socket socket = RemoteViewModel.ConnectToServer();
                        if (socket != null)
                        {
                            MessageBox.Show("Connected to server");
                            // Open the client view
                            ClientWindow clientWindow = new ClientWindow();
                            clientWindow.Show();
                            // Close the settings view
                            Window.GetWindow(this).Close();

                        }
                        else
                        {
                            MessageBox.Show("Failed to connect to server");
                        }*/


        }


    }
}
