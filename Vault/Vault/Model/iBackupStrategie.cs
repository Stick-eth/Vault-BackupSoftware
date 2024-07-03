using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Vault.Model
{
    /**
     *\brief This class represent the BackupJobFactory. The BackupJobFactory is a class that will be used to instanciate the BackupJob
     * It will be used to instanciate the BackupJob when the application is restarted
     */
    public interface iBackupStrategie
    {
        /**
         * Execute the BackupJob
         *
         * @param backupJob : the BackupJob to execute
         */
        public void Excute(BackupJob backupJob);

        /**
         * PreExecute the BackupJob
         *
         * @param backupJob : the BackupJob
         */
        public String Type();

        /**
         * RecursiveCopy the BackupJob
         *
         * @param backupJob : the BackupJob
         * @param sourceDirectory : the source directory
         */
        public void RecursiveCopy(BackupJob backupJob, String sourceDirectory);
        
    }
}
