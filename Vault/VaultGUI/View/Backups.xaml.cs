using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Vault.Model.Uilities;
using Vault.ViewModel;


namespace VaultGUI.View
{
    /// <summary>
    /// Logique d'interaction pour Backups.xaml
    /// </summary>
    public partial class Backups : UserControl
    {
        private ViewModelCore ViewModelCore; 
        private List<int> selectedJobs;

        public Backups()
        {
            InitializeComponent();
            ViewModelCore = ViewModelCore.GetInstance();
            datagridBackupJob.ItemsSource = ViewModelCore.backupList;


            BackupManager.GetInstance().UpdateBackupJob += RefreshActiveGrid;
            FillText();
        }

        public void RefreshDataGrid()
        {
            datagridBackupJob.ItemsSource = null;
            datagridBackupJob.ItemsSource = ViewModelCore.backupList;
        }


        public void RefreshActiveGrid(object? sender, EventArgs e)
        {
            List<Vault.Model.BackupJob> jobs = null;
            lock (BackupManager._activeJobs)
            {
                jobs = BackupManager.GetInstance().GetJobs().ToList(); 
            }

            Application.Current.Dispatcher.Invoke(() =>
            {
                try
                {
                    datagridActiveJob.ItemsSource = jobs;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error updating data grid: {ex.Message}");
                }
            });
        }




        private void RunSelectedJob_Click(object sender, RoutedEventArgs e)
        {
            // Get the selected jobs
            selectedJobs = GetSelectedJobs();
            //Create a list of all the selected jobs
            
            try {
                foreach (int job in selectedJobs)
                {
                    ViewModelCore.RunJobs(job.ToString());
                }
                RefreshDataGrid();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private void CreateBackupJob_Click_1(object sender, RoutedEventArgs e)
        {
            BackupsCreate backupsCreate = new BackupsCreate();
            backupsCreate.ShowDialog();
            RefreshDataGrid();
        }

        private void datagridBackupJob_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           
        }

        private List<int> GetSelectedJobs()
        {
            var indexes = new List<int>();
            for (int i = 0; i < datagridBackupJob.Items.Count; i++)
            {
                var item = datagridBackupJob.Items[i];
                var cell = datagridBackupJob.Columns[0].GetCellContent(item) as CheckBox;

                if (cell != null && (bool)cell.IsChecked)
                {
                    indexes.Add(i+1);
                }
               
            }
            return indexes;

        }

        private void Edit_Button(object sender, RoutedEventArgs e)
        {
            for (var vis = sender as Visual; vis != null; vis = VisualTreeHelper.GetParent(vis) as Visual)
            {
                if (vis is DataGridRow)
                {
                    var row = (DataGridRow)vis;
                    int index = row.GetIndex();
                    index++;
                    BackupsEdit backupsEdit = new BackupsEdit(index);
                    backupsEdit.ShowDialog();
                    RefreshDataGrid();
                    break;
                }
            }
        }

        private void Delete_Button(object sender, RoutedEventArgs e)
        {
            for (var vis = sender as Visual; vis != null; vis = VisualTreeHelper.GetParent(vis) as Visual)
            {
                if (vis is DataGridRow)
                {
                    var row = (DataGridRow)vis;
                    int index = row.GetIndex();
                    index++;
                    ViewModelCore.DeleteJob(index);
                    RefreshDataGrid();
                    break;
                }
            }
        }

        private void PauseResumeButton(object sender, RoutedEventArgs e)
        {
            for (var vis = sender as Visual; vis != null; vis = VisualTreeHelper.GetParent(vis) as Visual)
            {
                if (vis is DataGridRow)
                {
                    var row = (DataGridRow)vis;
                    int index = row.GetIndex();
                    BackupManager.GetInstance().PauseResume(index);
                    
                    break;
                }
            }   
        }

        private void Stop_Button(object sender, RoutedEventArgs e)
        {
            for (var vis = sender as Visual; vis != null; vis = VisualTreeHelper.GetParent(vis) as Visual)
            {
                if (vis is DataGridRow)
                {
                    var row = (DataGridRow)vis;
                    int index = row.GetIndex();
                    BackupManager.GetInstance().Stop(index);
                    break;
                }
            }
        }

        private void FillText()
        {
            BackupListTitle.Text = MainView.rm.GetString("BackupListTitle");
            CreateBackupJob.Content = MainView.rm.GetString("CreateBackupJob");
            HeaderName.Header = MainView.rm.GetString("HeaderName");
            HeaderSourceDirectory.Header = MainView.rm.GetString("HeaderSourceDirectory");
            HeaderTargetDirectory.Header = MainView.rm.GetString("HeaderTargetDirectory");
            HeaderType.Header = MainView.rm.GetString("HeaderType");
            RunSelectedJob.Content = MainView.rm.GetString("RunSelectedJob");
        }
    }
}
