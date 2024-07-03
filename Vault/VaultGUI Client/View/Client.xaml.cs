using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
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
using VaultGUI_Client.Model;
using VaultGUI_Client.ViewModel.Remote;

namespace VaultGUI_Client.View
{
    /// <summary>
    /// Logique d'interaction pour Client.xaml
    /// </summary>
    public partial class Client : UserControl
    {
        private string selectedJobs;
        private List<BackupJob> jobs;

        public Client()
        {
            InitializeComponent();
            RemoteViewModel.UpdateBackupJob += RefreshBackupJobs;
            RemoteViewModel.UpdateActiveBackupJob += RefreshActiveBackupJobs;
        }

        public void RefreshBackupJobs(object? sender,List<BackupJob> backuplist)
        {
            Dispatcher.Invoke(() =>
            {
                datagridBackupJob.ItemsSource = backuplist;
                datagridBackupJob.Items.Refresh();
            });
        }

        public void RefreshActiveBackupJobs(object? sender, List<BackupJob> backuplist)
        {
            Dispatcher.Invoke(() =>
            {
                datagridActiveJob.ItemsSource = backuplist;
                datagridActiveJob.Items.Refresh();
            });
        }


        private void RunSelectedJob_Click(object sender, RoutedEventArgs e)
        {
            // Get the selected jobs
            selectedJobs = GetSelectedJobs();
            //Create a list of all the selected jobs
                RemoteViewModel.RunJobs(RemoteViewModel._socket, selectedJobs);

        }

        private string GetSelectedJobs()
        {
            var indexes = new List<int>();
            string job = "";
            for (int i = 0; i < datagridBackupJob.Items.Count; i++)
            {
                var item = datagridBackupJob.Items[i];
                var cell = datagridBackupJob.Columns[0].GetCellContent(item) as CheckBox;

                if (cell != null && (bool)cell.IsChecked)
                {
                    indexes.Add(i + 1);
                    job += (i + 1).ToString() + ";";
                }

            }
            return job.Remove(job.Length - 1);

        }


        private  void datagridBackupJob_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private  void datagridActiveJob_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }


        private void PauseResumeButton(object sender, RoutedEventArgs e)
        {
            for (var vis = sender as Visual; vis != null; vis = VisualTreeHelper.GetParent(vis) as Visual)
            {
                if (vis is DataGridRow)
                {
                    var row = (DataGridRow)vis;
                    int index = row.GetIndex();
                    RemoteViewModel.PauseResume(RemoteViewModel._socket, index);
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
                    RemoteViewModel.Stop(RemoteViewModel._socket, index);
                }
            }
        }


        private void DisconnectButton_Click(object sender, RoutedEventArgs e)
        {
            // Disconnect from the server
            RemoteViewModel.Shutdown(RemoteViewModel._socket);
            // Close the window
            Window.GetWindow(this).Close();
        }

    }
}
