using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vault.Model.Config;

namespace Vault.Model.Uilities
{
    public class BusinessAppMonitoring
    {
        private static BusinessAppMonitoring instance;
        private Thread monitoringThread;
        public volatile bool isForbiddenProcessRunning = false;


        public bool GetIsForbiddenProcessRunning()
        {
            return isForbiddenProcessRunning;
        }

        private BusinessAppMonitoring()
        {
            StartMonitoring();
        }
        public static BusinessAppMonitoring GetInstance()
        {
            if (instance == null)
            {
                instance = new BusinessAppMonitoring();
            }
            return instance;
        }

        public void StartMonitoring()
        {
            monitoringThread = new Thread(new ThreadStart(MonitorProcess));
            monitoringThread.Name = "MonitoringBusinessApp";
            monitoringThread.Start();
        }

        public void StopMonitoring()
        {
            monitoringThread.Abort();
        }

        private void MonitorProcess()
        {
            while (true)
            {
                if (AppConfig.monitoredProcesses.Count > 0)
                {
                    foreach (string process in AppConfig.monitoredProcesses)
                    {
                        if (Process.GetProcessesByName(process).Length > 0)
                        {
                            isForbiddenProcessRunning = true;
                        }
                        else
                        {
                            isForbiddenProcessRunning = false;
                        }
                    }
                }
                else
                {
                    isForbiddenProcessRunning = false;
                }
                Thread.Sleep(1000);
            }
        }
    }
}
