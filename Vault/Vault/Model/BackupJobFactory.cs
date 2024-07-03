﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Vault.Model
{
    /**
     *\brief This class represent the BackupJobFactory. The BackupJobFactory is a class that will be used to instanciate the BackupJob
     * It will be used to instanciate the BackupJob when the application is restarted
     */
    abstract class BackupJobFactory
    {
        abstract public BackupJob InstanciateBackupJob(string name, string sourceDirectory, string targetDirectory);

        /**
         * Create a BackupJob
         *
         * @param name : the name of the BackupJob
         * @param sourceDirectory : the source directory of the BackupJob
         * @param targetDirectory : the target directory of the BackupJob
         */
        public BackupJob CreateBackupJob(string name, string sourceDirectory, string targetDirectory)
        {
            return InstanciateBackupJob(name, sourceDirectory, targetDirectory);
        }
    }
}
