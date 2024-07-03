using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

/**
 *\brief This namespace contain all the class that will be used to store the configuration of the application
 *  
 */
namespace Vault.Model
{
    /**
     *\brief This class represent a BackupJob. A BackupJob is a task that will be executed by the BackupManager
     * It can be a simple backup or a backup with a specific comportement (incremental, differential, full)
     * at every execution of the BackupJob, the RealtimeStats will be updated
     * 
     */
    public class BackupJob
    {
        // Attributes
        private String name { get; }
        public String sourceDirectory { get; set; }
        private String targetDirectory { get; set; }
        private int totalFiles { get; set; }
        private long jobSize { get; set; }
        public iBackupStrategie saveType;   // Each BackupJob have an iBackupStrategie instance that define the comportement during executing the BackupJob
        public RealtimeStats realtimeStats; // Each BackupJob have an RealtimeStats instance that take car of saving all the stats


        /*
         * Accessor & Mutator for the attributes
         * 
         * @param name : the name of the BackupJob
         * @param sourceDirectory : the source directory of the BackupJob
         * @param targetDirectory : the target directory of the BackupJob
         * @param saveType : the comportement of the BackupJob
         * 
         * return : void
         */
        public BackupJob(string name, string sourceDirectory, string targetDirectory, iBackupStrategie saveType)
        {
            this.name = name;
            this.sourceDirectory = sourceDirectory;
            this.targetDirectory = targetDirectory;
            this.saveType = saveType;
            this.realtimeStats = new RealtimeStats(this.name);
        }
        public String Name { get { return name; } } 
        public String SourceDirectory { get { return sourceDirectory; } }
        public String TargetDirectory { get { return targetDirectory; } }
        public int TotalFiles { get { return totalFiles; } }
        public long JobSize { get { return jobSize; } }
        public int Progress{ get { return realtimeStats.Progress; }  }
        public string IsPosed { get { return realtimeStats.IsPaused.ToString(); } }

        public String Type { get { return saveType.Type();} }

        // Methods
        /**
         * Get information about the BackupJob
         *
         * return : String
         */
        public String Info()
        {
            return $"{name}, SRC : {sourceDirectory}, DST : {targetDirectory}, type : {saveType.Type()}";
        }

        /**
         * Update the folder of the BackupJob
         * 
         * return : void
         */
        public void Update(string sourceDirectory, string targetDirectory)
        {
            this.sourceDirectory = sourceDirectory;
            this.targetDirectory = targetDirectory;
        }

        /**
         * Run the BackupJob
         */
        public void Run()
        {
            ScanTargetDirectory();
            // Use the saveType comportement to execute the BackupJob
            saveType.Excute(this);
        }

        /*
         * Retrieve File number & Total size byte about the BackupJob
         */
        public void ScanTargetDirectory()
        {
            totalFiles = 0;
            jobSize = 0;
            // Recursive search in the folder to get the information about each file
            ProcessDirectory(sourceDirectory);
        }

        /** 
         *Recursive search in the folder to get the information about each file
         * 
         *@param targetDirectory : the directory to scan
         *
         *return : void
         */
        private void ProcessDirectory(string targetDirectory)
        {
            // Process the list of files found in the directory.
            string[] fileEntries = Directory.GetFiles(targetDirectory);
            totalFiles += fileEntries.Length;
            foreach (string fileName in fileEntries)
            {
                FileInfo file = new System.IO.FileInfo(fileName);
                jobSize += file.Length;
            }

            // Recurse into subdirectories of this directory.
            string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
            foreach (string subdirectory in subdirectoryEntries)
                ProcessDirectory(subdirectory);
        }


        public static string SerializeJobs( List<BackupJob> jobs)
        {
            String AllJobs = "[";
            // Retrieve all RealtimeStats of each BackupJob add it in the string and write the sting in the currentState.json file
            foreach (BackupJob backupJob in jobs)
            {
                var saveData = new
                {
                    backupJob.Name,
                    FileSource = backupJob.SourceDirectory,
                    FileTarget = backupJob.TargetDirectory,
                    SaveType = backupJob.saveType.Type()
                };

                AllJobs += System.Environment.NewLine;
                AllJobs += (JsonSerializer.Serialize(saveData));
                AllJobs += ",";
            }
            AllJobs = AllJobs.Remove(AllJobs.Length - 1) + System.Environment.NewLine + "]";
            return AllJobs;

        }
        public static string SerializeActiveJobs(List<BackupJob> jobs)
        {
            String AllJobs = "[ ";
            // Retrieve all RealtimeStats of each BackupJob add it in the string and write the sting in the currentState.json file
            foreach (BackupJob backupJob in jobs)
            {
                var saveData = new
                {
                    Name = backupJob.Name,
                    IsPaused = backupJob.realtimeStats.IsPaused,
                    Progress = backupJob.realtimeStats.Progress,
                };

                AllJobs += System.Environment.NewLine;
                AllJobs += (JsonSerializer.Serialize(saveData));
                AllJobs += ",";
            }
            AllJobs = AllJobs.Remove(AllJobs.Length - 1)+ "]";
            return AllJobs;

        }


    }
}
