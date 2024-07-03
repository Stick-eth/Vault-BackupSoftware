using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaultGUI_Client.Model
{
    public class BackupJob
    {
        public string GridName { get; set; }
        public string Name { get; set; }
        public string FileSource { get; set; }
        public string FileTarget { get; set; }
        public string SaveType { get; set; }

        public bool IsPaused { get; set; }
        public int Progress { get; set; }
    }
}
