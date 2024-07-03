using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Logique d'interaction pour Home.xaml
    /// </summary>
    public partial class Home : UserControl
    {
        private ViewModelCore ViewModelCore;
        public static ResourceManager rm = new ResourceManager("VaultGUI.Ressources.Texts", typeof(MainView).Assembly);
        private string lang;
        public Home()
        {
            InitializeComponent();
            ViewModelCore = ViewModelCore.GetInstance();
            refreshlang();
            FillText();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        public void FillText()
        {
            refreshlang();
            HomeWelcome.Content = rm.GetString("HomeWelcome");
            HomeTips.Content = rm.GetString("HomeTips");
            DocButton.Content = rm.GetString("DocButton");
            MainName.Text = rm.GetString("MainName");

        }

        private void refreshlang()
        {
            lang = ViewModelCore.GetLanguage();
            if (lang == "fr-FR")
            {
                System.Globalization.CultureInfo.CurrentUICulture = new System.Globalization.CultureInfo("fr-FR");
            }
            else
            {
                System.Globalization.CultureInfo.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
            }
        }
    }
}
