using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace VaultGUI.Utilities
{
    public class SingleInstance
    {
        private static Mutex mutex = null;
        private const string uniqueIdentifier = "Global\\VaultGUI";

        public static bool CheckAlreadyRunning()
        {
            bool createdNew;
            mutex = new Mutex(true, uniqueIdentifier, out createdNew);
            if (!createdNew)
            {
                return false;
            }
            return true;
        }

        public static void ReleaseMutex()
        {
            if (mutex != null)
            {
                mutex.ReleaseMutex();
                mutex.Dispose();
            }
        }
    }
}
