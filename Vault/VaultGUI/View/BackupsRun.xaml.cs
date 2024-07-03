using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Threading;
using System.Diagnostics;
using Vault.ViewModel;


namespace VaultGUI.View
{
    /// <summary>
    /// Logique d'interaction pour BackupsRun.xaml
    /// </summary>
    public partial class BackupsRun : Window
    {
        private ViewModelCore ViewModelCore;
        private List<int> selectedJobs;
        private bool isPaused = false;
        private Mutex mutex = new Mutex();

        public BackupsRun(List<int> e)
        {
            InitializeComponent();
            ViewModelCore = ViewModelCore.GetInstance();

        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            BackgroundWorker runner = new BackgroundWorker();
            runner.WorkerReportsProgress = true;
            runner.DoWork += worker_DoWork;
            runner.ProgressChanged += worker_ProgressChanged;
            runner.RunWorkerAsync();
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            //Work to be done
            for (int i = 0; i <= 100; i++)
            {
                    mutex.WaitOne();
                       //Thread.Sleep(1000);
                    mutex.ReleaseMutex();
                (sender as BackgroundWorker).ReportProgress(i);
              //  Thread.Sleep(500);
            }
        }

        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
            update("Backup in progress: " + e.ProgressPercentage + "%");
            if (progressBar.Value == 100)
            {
                Debug.WriteLine("Backup finished");
                Close();
                
            }
        }

        private void update(string message)
        {
            Text.Content = message;
        }


        private void PauseButton(object sender, RoutedEventArgs e)
        {
            if (isPaused)
            {
                isPaused = false;
                PauseBtn.Content = "Pause";
                update("Backup in progress: " + progressBar.Value + "%");
                mutex.ReleaseMutex();
            }
            else
            {
                isPaused = true;
                PauseBtn.Content = "Resume";
                update("Backup paused: " + progressBar.Value + "%");
                mutex.WaitOne();
            }

        }

        private void StopButton(object sender, RoutedEventArgs e)
        {
            isPaused = true;
            Close();

        }
    }
}
