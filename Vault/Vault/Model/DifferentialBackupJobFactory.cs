using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vault.Model
{
    /**
     *\brief This class represent the BackupJobFactory. The BackupJobFactory is a class that will be used to instanciate the BackupJob
     * It will be used to instanciate the BackupJob when the application is restarted
     */
    internal class DifferentialBackupJobFactory : BackupJobFactory
    {
            /**
            * Instanciate a BackupJob
            *
            * @param name : the name of the BackupJob
            * @param sourceDirectory : the source directory of the BackupJob
            * @param targetDirectory : the target directory of the BackupJob
            */
        public override BackupJob InstanciateBackupJob(string name, string sourceDirectory, string targetDirectory)
        {
            if (!Directory.Exists(sourceDirectory))
            {
                throw new ArgumentException($"InexistantSourceDirectoryError|{sourceDirectory}");
            }
            if (!Directory.Exists(targetDirectory))
            {
                throw new ArgumentException($"InexistantTargetDirectoryError|{targetDirectory}");
            }
            if (targetDirectory.Contains(sourceDirectory))
            {
                throw new ArgumentException("DirectoryTargetInsideSourceError");
            }
            return new BackupJob(name, sourceDirectory, targetDirectory, new DifferentialBackupStrategie());
        }
    }
}
