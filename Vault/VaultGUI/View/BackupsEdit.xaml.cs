using Microsoft.Win32;
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
using System.Security.AccessControl;

namespace VaultGUI.View
{
    /// <summary>
    /// Logique d'interaction pour BackupsEdit.xaml
    /// </summary>
    public partial class BackupsEdit : Window
    {
        private ViewModelCore ViewModelCore;
        private int index;
        private int saveType = 0;

        OpenFolderDialog dialog = new OpenFolderDialog()
        
        {
            Title = "Select Folder",
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal),
            Multiselect = false
        };
        public BackupsEdit(int ind)
        {
            InitializeComponent();
            ViewModelCore = ViewModelCore.GetInstance();
            SaveName.Text = ViewModelCore.GetJobName(ind);
            index = ind;
            SourceFolder.Text = ViewModelCore.backupList[index - 1].SourceDirectory;
            TargetFolder.Text = ViewModelCore.backupList[index - 1].TargetDirectory;
            FillText();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (dialog.ShowDialog() == true)
            {
                SourceFolder.Text = dialog.FolderName;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (dialog.ShowDialog() == true)
            {
                TargetFolder.Text = dialog.FolderName;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            //Verifiy if the fields are not empty, then ViewModelCore.UpdateJob(index, Source,Target);
            if (SourceFolder.Text != "" && TargetFolder.Text != "" && saveType != 0)
            {
                ViewModelCore.UpdateJob(index, SourceFolder.Text, TargetFolder.Text, saveType);
                this.Close();
                Backups backups = new Backups();
                backups.RefreshDataGrid();
            }
            else
            {
                MessageBox.Show("Veuillez remplir tous les champs");
            }
        }

        private void DifferentialSave_Checked(object sender, RoutedEventArgs e)
        {
            saveType = 2;
        }

        private void FullSave_Checked(object sender, RoutedEventArgs e)
        {
            saveType = 1;
        }

        private void FillText()
        {
            Savename.Text = MainView.rm.GetString("SaveName");
            SourceFolderTitle.Text = MainView.rm.GetString("SourceFolderTitle");
            DestFolderTitle.Text = MainView.rm.GetString("DestFolderTitle");
            SaveButton.Content = MainView.rm.GetString("SaveButton");
            SelectFolder.Content = MainView.rm.GetString("SelectFolder");
            SelectFolder1.Content = MainView.rm.GetString("SelectFolder");
        }
    }
}
