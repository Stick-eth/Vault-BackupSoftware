using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Vault.ViewModel;
using Vault.Model;
using System.Resources;

namespace VaultGUI
{
    /// <summary>
    /// Logique d'interaction pour MainView.xaml
    /// </summary>
    public partial class MainView : Window
    {

        private ViewModelCore ViewModelCore;
        public static ResourceManager rm = new ResourceManager("VaultGUI.Ressources.Texts", typeof(MainView).Assembly);
        private string lang;
        private static MainView _instance;
        public static MainView GetInstance()
        {
            if (_instance == null)
            {
                _instance = new MainView();
            }
            return _instance;
        }
        private MainView()
        {
            InitializeComponent();
            ViewModelCore = ViewModelCore.GetInstance();
            ViewModelCore.LoadSaveConfig();
            ViewModelCore.LoadConfig();
            refreshlang();  
            FillText();
        }

        private void CloseApp_Click_1(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Btn_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void Btn_Checked_1(object sender, RoutedEventArgs e)
        {

        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();

        }

        public void FillText()
        {
            refreshlang();
            homebtn.Text = rm.GetString("homebtn");
            backupbtn.Text = rm.GetString("backupbtn");
            configbtn.Text = rm.GetString("configbtn");
            settingsbtn.Text = rm.GetString("settingsbtn");
            Subscribebtn.Content = rm.GetString("Subscribebtn");
        }

        private void refreshlang()
        {
            lang = ViewModelCore.GetLanguage();
            if(lang == "fr-FR")
            {
                System.Globalization.CultureInfo.CurrentUICulture = new System.Globalization.CultureInfo("fr-FR");
            }
            else
            {
                System.Globalization.CultureInfo.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
            }
        }

        private void Subscribebtn_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
