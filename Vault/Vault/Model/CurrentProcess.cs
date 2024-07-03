using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vault.Model.Config;

namespace Vault.Model
{
    /**
     *\brief This class represent the CurrentProcess. The CurrentProcess is a class that will be used to store all the process that are currently running
     * It will be used to store the list of the monitored processes
     *
     */
    public class CurrentProcess
    {
        /**
         * Check if the process is running
         */
        public void CheckRunningProcess()
        {
            foreach (var processName in AppConfig.monitoredProcesses)
            {
                if (Process.GetProcessesByName(processName).Length > 0)
                {
                    Console.WriteLine($"Process {processName} is running");
                    throw new ArgumentException($"AppRunning_Error");
                }
            }
        }

        /**
         * Add a process to the monitored process list
         */
        public void AddProcess(string processName)
        {
            AppConfig.monitoredProcesses.Add(processName);
        }

        /**
         * Remove a process from the monitored process list
         */
        public void RemoveProcess(string processName)
        {
            AppConfig.monitoredProcesses.Remove(processName);
        }
    }
}
