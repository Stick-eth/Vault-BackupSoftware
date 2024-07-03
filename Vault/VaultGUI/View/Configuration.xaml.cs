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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Vault.ViewModel;

namespace VaultGUI.View
{
    /// <summary>
    /// Logique d'interaction pour Configuration.xaml
    /// </summary>
    public partial class Configuration : UserControl
    {

        private ViewModelCore ViewModelCore;
        private string logExtension;

        public Configuration()
        {
            InitializeComponent();
            ViewModelCore = ViewModelCore.GetInstance();
            if(ViewModelCore.GetLogExtension() == ".json")
            {
                LogJson.IsChecked = true;
                logExtension = ".json";
            }
            else
            {
                LogXml.IsChecked = true;
                logExtension = ".xml";
            }
            foreach (var item in ViewModelCore.GetExtension())
            {
                CryptedExtension.Items.Add(item);
            }
            foreach (var item in ViewModelCore.GetPriority())
            {
                PrioExtension.Items.Add(item);
            }
            FillText();
        }

        private void removeList_Click(object sender, RoutedEventArgs e)
        {
            try { 
            CryptedExtension.Items.RemoveAt(CryptedExtension.SelectedIndex);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void removePrioList_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PrioExtension.Items.RemoveAt(PrioExtension.SelectedIndex);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void addList_Click(object sender, RoutedEventArgs e)
        {
            if (NewCryptedExtention.Text == "") return;
            if (CryptedExtension.Items.Contains(NewCryptedExtention.Text)) return;

            CryptedExtension.Items.Add(NewCryptedExtention.Text);
            NewCryptedExtention.Text = "";
        }

        private void addPrioList_Click(object sender, RoutedEventArgs e)
        {
            if (NewPrioExtension.Text == "") return;
            if (PrioExtension.Items.Contains(NewPrioExtension.Text)) return;

            PrioExtension.Items.Add(NewPrioExtension.Text);
            NewPrioExtension.Text = "";
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SetLog();
            SetExtension();
            SetPrio();
        }

        private void NewCryptedExtention_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void LogXml_Checked(object sender, RoutedEventArgs e)
        {
            logExtension = ".xml";
        }

        private void LogJson_Checked(object sender, RoutedEventArgs e)
        {
            logExtension = ".json";
        }
        private void SetLog()
        {
            if (logExtension == ".json")
            {
                ViewModelCore.SetLogJson();
            }
            else
            {
                ViewModelCore.SetLogXml();
            }
        }

        private void SetExtension()
        {
            List<string> list = new List<string>();
            foreach (var item in CryptedExtension.Items)
            {
                list.Add(item.ToString());
            }
            ViewModelCore.SetExtension(list);
        }

        private void SetPrio()
        {
            List<string> list = new List<string>();
            foreach (var item in PrioExtension.Items)
            {
                list.Add(item.ToString());
            }
            ViewModelCore.SetPriority(list);
        }

        private void FillText()
        {
            JobConfTitle.Text = MainView.rm.GetString("JobConfTitle");
            LogFormatTitle.Text = MainView.rm.GetString("LogFormatTitle");
            ExtToEncryptTitle.Text = MainView.rm.GetString("ExtToEncryptTitle");
            SelectedExt.Text = MainView.rm.GetString("SelectedExt");
            AddExt.Text = MainView.rm.GetString("AddExt");
            RemoveFromList.Content = MainView.rm.GetString("RemoveFromList");
            AddToList.Content = MainView.rm.GetString("AddToList");
            SaveButton.Content = MainView.rm.GetString("SaveButton");
            ExtToPrioritizeTitle.Text = MainView.rm.GetString("ExtToPrioritizeTitle");


        }

    }
}
