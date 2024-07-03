using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using VaultGUI.View;
using Vault.ViewModel;
using Vault.Model;


namespace VaultGUI.View
{
    /// <summary>
    /// Logique d'interaction pour BackupsCreate.xaml

    public partial class BackupsCreate : Window
    {
        private ViewModelCore ViewModelCore;
        private int saveType; // 1 = full, 2 = differential

        OpenFolderDialog dialog = new OpenFolderDialog()
        {
            Title = "Select Folder",
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal),
            Multiselect = false
        };
        public BackupsCreate()
        {
            InitializeComponent();
            ViewModelCore = ViewModelCore.GetInstance();
            saveType = 0;
            FillText();
        }

        private void SelectSourceFolder_Click(object sender, RoutedEventArgs e)
        {
            if (dialog.ShowDialog() == true)
            {
                SourceFolder.Text = dialog.FolderName;
            }
        }

        private void SelectTargetFolder_Click(object sender, RoutedEventArgs e)
        {
            if (dialog.ShowDialog() == true)
            {
                TargetFolder.Text = dialog.FolderName;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ViewModelCore = ViewModelCore.GetInstance();
            try {
                ViewModelCore.CreateJob(BakckupJobName.Text, SourceFolder.Text, TargetFolder.Text, GetSaveType());
                Close();
                Backups backups = new Backups();
                backups.RefreshDataGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

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

        private string GetSaveType()
        {
            if (saveType == 1)
            {
                return "full";
            }
            else if (saveType == 2)
            {
                return "differential";
            }
            else
            {
                throw new ArgumentException("InvalidSaveType");
            }
        }

        private void FillText()
        {
            BackupNameTitle.Text = MainView.rm.GetString("BackupNameTitle");
            SourceFolderTitle.Text = MainView.rm.GetString("SourceFolderTitle");
            DestFolderTitle.Text = MainView.rm.GetString("DestFolderTitle");
            SaveTypeTitle.Text = MainView.rm.GetString("SaveTypeTitle");
            DifferentialSave.Content = MainView.rm.GetString("DifferentialSave");
            FullSave.Content = MainView.rm.GetString("FullSave");
            CreateSave.Content = MainView.rm.GetString("SaveButton");
        }
    }
}
