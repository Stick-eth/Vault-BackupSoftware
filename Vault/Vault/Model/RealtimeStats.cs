using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vault.Model
{
    /**
     *\brief This class represent the RealtimeStats. The RealtimeStats is a class that will be used to display the stats of the backup in real time
     */
    public class RealtimeStats
    {
        public String Name { get; set; }
        public String State { get; set; }
        public String SourceFilePath { get; set; }
        public String TargetFilePath { get; set; }
        public int NbFiles { get; set; }
        public long SizeFiles { get; set; }
        public int NbFilesLeft { get; set; }
        public long SizeFilesLeft { get; set; }
        public long CurrentFileSize { get; set; }
        public int Progress { get; set; }
        public bool IsPaused { get; set; }
        public bool IsStoped {  get; set; }

        /**
         * Constructor of the RealtimeStats
         *
         * @param name : the name of the backup
         */
        public RealtimeStats(String name)
        {
            this.Name = name;
            this.State = "idle";
            this.SourceFilePath = "";
            this.TargetFilePath = "";
            this.IsPaused = false;
            this.IsStoped = false;
        }
    }
}
