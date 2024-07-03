using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vault.Model.Config;
using Vault.Model.Logger;
using Vault.Model.Uilities;
using Vault.ViewModel;

namespace Vault.Model
{
    /**
     *\brief This class represent the DifferentialBackupStrategie. The DifferentialBackupStrategie is a class that will be used to execute the differential backup
     * It will be used to execute the differential backup
     *
     */
    public class DifferentialBackupStrategie : iBackupStrategie
    {
        private Log log = Log.GetInstance();
        private RTLog rtlog = RTLog.GetInstance();
        public Mutex mutex = new Mutex();
        public PriorityExtensions priority = PriorityExtensions.GetInstance();
        public PriorityJobs priorityThread = PriorityJobs.GetInstance();

        /**
         * PreExecute the backupJob
         *
         * @param backupJob : the backupJob
         */
        private void PreExecute(BackupJob backupJob)
        {

            backupJob.realtimeStats.State = "running";

            backupJob.realtimeStats.NbFiles = backupJob.TotalFiles;
            backupJob.realtimeStats.NbFilesLeft = backupJob.TotalFiles;

            backupJob.realtimeStats.SizeFiles = backupJob.JobSize;
            backupJob.realtimeStats.SizeFilesLeft = backupJob.JobSize;

            backupJob.realtimeStats.Progress = 0;
            rtlog.WriteLiveState();
            BackupManager.GetInstance().AddActiveJob(backupJob);
            BackupManager.GetInstance().OnUpdateBackupJob();
        }

        /**
            * PostExecute the backupJob
            *
            * @param backupJob : the backupJob
            */
        private void PostExecute(BackupJob backupJob)
        {
            backupJob.realtimeStats.State = "idle";

            backupJob.realtimeStats.NbFiles = 0;
            backupJob.realtimeStats.NbFilesLeft = 0;

            backupJob.realtimeStats.SizeFiles = 0;
            backupJob.realtimeStats.SizeFilesLeft = 0;

            backupJob.realtimeStats.CurrentFileSize = 0;

            backupJob.realtimeStats.SourceFilePath = "";
            backupJob.realtimeStats.TargetFilePath = "";
            backupJob.realtimeStats.Progress = 0;
            backupJob.realtimeStats.IsPaused = false;
            rtlog.WriteLiveState();
            BackupManager.GetInstance().RemoveActiveJob(backupJob);
            BackupManager.GetInstance().OnUpdateBackupJob();
        }

        /**
         * Execute the backupJob
         *
         * @param backupJob : the backupJob
         */
        public void Excute(BackupJob backupJob)
        {
            PreExecute(backupJob);
            LauchFindPriorityFiles(backupJob, backupJob.SourceDirectory);
            RecursiveCopy(backupJob, backupJob.SourceDirectory);
            PostExecute(backupJob);
        }

        private void IfStoped(bool IsStoped, BackupJob backupJob)
        {
            if (IsStoped)
            {
                PostExecute(backupJob);
                backupJob.realtimeStats.IsStoped = false;
                Thread.CurrentThread.Join();
            }
        }

        /**
         * Recursive copy of the source directory to the target directory
         *
         * @param backupJob : the backupJob
         * @param sourceDirectory : the source directory
         */
        public void RecursiveCopy(BackupJob backupJob, String sourceDirectory)
        {
            // Process the list of files found in the directory.
            string[] fileEntries = Directory.GetFiles(sourceDirectory);
            foreach (string fileName in fileEntries)
            {
                while (backupJob.realtimeStats.IsPaused || BusinessAppMonitoring.GetInstance().GetIsForbiddenProcessRunning())
                {
                    Thread.Sleep(1000);
                    IfStoped(backupJob.realtimeStats.IsStoped, backupJob);
                }
                IfStoped(backupJob.realtimeStats.IsStoped, backupJob);


                // Remove path from the file name.
                string fName = fileName.Substring(backupJob.SourceDirectory.Length + 1);
                String fullSourcePath = Path.Combine(backupJob.SourceDirectory, fName);
                String fullDestPath = Path.Combine(backupJob.TargetDirectory, fName);

                try
                {
                    // Updating information in RealtimeStats
                    backupJob.realtimeStats.SourceFilePath = fullSourcePath;
                    backupJob.realtimeStats.TargetFilePath = fullDestPath;
                    FileInfo file = new System.IO.FileInfo(fullSourcePath);
                    backupJob.realtimeStats.CurrentFileSize = file.Length;
                    string extensionFile = fullSourcePath.Split('.')[1];

                    //Get the file size for BandWidthManaging
                    if (file.Length > BandWidthManager.GetInstance().GetLimit())
                    {
                        BandWidthManager.GetInstance().GetMutex();
                        Console.WriteLine("Took the bandwidth Mutex of " + fullSourcePath);
                    }

                    //Verify if the file exist in the target directory
                    if (!PriorityExtensions.GetInstance().IsInList(extensionFile))
                    {
                        // Start timmer for log
                        int cryptime = 0;
                        DateTime start = DateTime.Now;

                        // Test if the file need to be encrypted
                        if (Extension.GetInstance().ExtensionInList(fullSourcePath))
                        {
                            fullDestPath += ".crypt";
                            cryptime = CryptedCopy(fullSourcePath, fullDestPath);
                        }
                        else
                        {
                            if (!(File.Exists(fullSourcePath)) || File.GetLastWriteTimeUtc(fullSourcePath) != File.GetLastWriteTimeUtc(fullDestPath))
                            {
                                File.Copy(fullSourcePath, fullDestPath, true);
                            }
                        }
                        TimeSpan timeDiff = DateTime.Now - start;
                        // Write RealtimeStats and Log in file
                        log.WriteContent(backupJob.Name, fullSourcePath, fullDestPath, (int)file.Length, timeDiff.TotalMilliseconds, cryptime);
                        backupJob.realtimeStats.SizeFilesLeft -= file.Length;
                        backupJob.realtimeStats.NbFilesLeft--;

                        // Update progress
                        backupJob.realtimeStats.Progress = (int)((backupJob.JobSize - backupJob.realtimeStats.SizeFilesLeft) * 100 / backupJob.JobSize);

                        rtlog.WriteLiveState();
                        BackupManager.GetInstance().OnUpdateBackupJob("blocked");
                    }

                }
                catch (Exception e)
                {
                    throw new ArgumentException("ErrorFileCopy|", fullSourcePath);
                }
                finally
                {
                    try
                    {
                        BandWidthManager.GetInstance().ReleaseMutex();
                        Console.WriteLine("Released the bandwidth Mutex");
                    }
                    catch (Exception e)
                    {

                    }

                }
            }

            // Recurse into subdirectories of this directory.
            string[] subdirectoryEntries = Directory.GetDirectories(sourceDirectory);
            foreach (string subdirectory in subdirectoryEntries)
            {
                string fDir = subdirectory.Substring(backupJob.SourceDirectory.Length + 1);
                Directory.CreateDirectory(Path.Combine(backupJob.TargetDirectory, fDir));
                RecursiveCopy(backupJob, subdirectory);
            }
        }

        /**
        *Start process Cryptosoft.exe that cpoy fullSourcePath to fullDestPath
        *with a XOR BIT to BIT encryption stock written in key.txt
        *Cryptosoft.exe and key.txt should be in te same folder as Vault.exe
        *Return the time (in ms) to execute encrypted copy
        *
        *@param fullSourcePath : the source path
        */
        private int CryptedCopy(string fullSourcePath, string fullDestPath)
        {
            try
            {
                using (Process myProcess = new Process())
                {
                    // You can start any process, HelloWorld is a do-nothing example.
                    myProcess.StartInfo.FileName = Path.Combine(Environment.CurrentDirectory,"Cryptosoft.exe");
                    myProcess.StartInfo.Arguments = $"\"{fullSourcePath}\" \"{fullDestPath}\"";
                    myProcess.StartInfo.CreateNoWindow = true;
                    myProcess.Start();
                    myProcess.WaitForExit();
                    int time = myProcess.ExitCode;
                    return time;
                }
            }
            catch (Exception e)
            {
                return -1;
            }
        }
        public void LauchFindPriorityFiles(BackupJob backupJob, String sourceDirectory)
        {
            PriorityJobs.GetInstance()._mutex.WaitOne();
            BackupManager.GetInstance().PauseAllActiveJobs();
            backupJob.realtimeStats.IsPaused = false;
            BackupManager.GetInstance().OnUpdateBackupJob();
            CopyPriorityFiles(backupJob, sourceDirectory);
            BackupManager.GetInstance().ResumeAllActiveJobs();
            PriorityJobs.GetInstance()._mutex.ReleaseMutex();
            BackupManager.GetInstance().OnUpdateBackupJob();
        }


        /**
         *Find the priority files
         *
         *Scan all the folder and subfolder to find the files including the extension in the priority list and add them to a list
         *
         */
        public void CopyPriorityFiles(BackupJob backupJob, String sourceDirectory)
        {

            // Process the list of files found in the directory.
            string[] fileEntries = Directory.GetFiles(sourceDirectory);
            foreach (string fileName in fileEntries)
            {
                while (backupJob.realtimeStats.IsPaused || BusinessAppMonitoring.GetInstance().GetIsForbiddenProcessRunning())
                {
                    Thread.Sleep(1000);
                    if(backupJob.realtimeStats.IsStoped)
                    {
                        BackupManager.GetInstance().ResumeAllActiveJobs();
                        PriorityJobs.GetInstance()._mutex.ReleaseMutex();
                    }
                    IfStoped(backupJob.realtimeStats.IsStoped, backupJob);
                }
                if (backupJob.realtimeStats.IsStoped)
                {
                    BackupManager.GetInstance().ResumeAllActiveJobs();
                    PriorityJobs.GetInstance()._mutex.ReleaseMutex();
                }
                IfStoped(backupJob.realtimeStats.IsStoped, backupJob);


                // Remove path from the file name.
                string fName = fileName.Substring(backupJob.SourceDirectory.Length + 1);
                String fullSourcePath = Path.Combine(backupJob.SourceDirectory, fName);
                String fullDestPath = Path.Combine(backupJob.TargetDirectory, fName);

                try
                {

                    // Updating information in RealtimeStats
                    backupJob.realtimeStats.SourceFilePath = fullSourcePath;
                    backupJob.realtimeStats.TargetFilePath = fullDestPath;
                    FileInfo file = new System.IO.FileInfo(fullSourcePath);
                    backupJob.realtimeStats.CurrentFileSize = file.Length;
                    string extensionFile = fullSourcePath.Split('.')[1];

                    //Verify if the file exist in the target directory
                    if (PriorityExtensions.GetInstance().IsInList(extensionFile))
                    {
                        // Start timmer for log
                        int cryptime = 0;
                        DateTime start = DateTime.Now;

                        // Test if the file need to be encrypted
                        if (Extension.GetInstance().ExtensionInList(fullSourcePath))
                        {
                            fullDestPath += ".crypt";
                            cryptime = CryptedCopy(fullSourcePath, fullDestPath);
                        }
                        else
                        {
                            if (!(File.Exists(fullSourcePath)) || File.GetLastWriteTimeUtc(fullSourcePath) != File.GetLastWriteTimeUtc(fullDestPath))
                            {
                                File.Copy(fullSourcePath, fullDestPath, true);
                            }
                        }
                        TimeSpan timeDiff = DateTime.Now - start;
                        // Write RealtimeStats and Log in file
                        log.WriteContent(backupJob.Name, fullSourcePath, fullDestPath, (int)file.Length, timeDiff.TotalMilliseconds, cryptime);
                        backupJob.realtimeStats.SizeFilesLeft -= file.Length;
                        backupJob.realtimeStats.NbFilesLeft--;

                        // Update progress
                        backupJob.realtimeStats.Progress = (int)((backupJob.JobSize - backupJob.realtimeStats.SizeFilesLeft) * 100 / backupJob.JobSize);

                        rtlog.WriteLiveState();
                        BackupManager.GetInstance().OnUpdateBackupJob("blocked");
                    }

                }
                catch (Exception e)
                {
                    throw new ArgumentException("ErrorFileCopy|", fullSourcePath);
                }
            }

            // Recurse into subdirectories of this directory.
            string[] subdirectoryEntries = Directory.GetDirectories(sourceDirectory);
            foreach (string subdirectory in subdirectoryEntries)
            {
                string fDir = subdirectory.Substring(backupJob.SourceDirectory.Length + 1);
                Directory.CreateDirectory(Path.Combine(backupJob.TargetDirectory, fDir));
                CopyPriorityFiles(backupJob, subdirectory);
            }
        }

        /**
         * Return the type of the backupStrategie
         *
         * @return the type of the backupStrategie
         */
        public string Type()
        {
            return "differential";
        }
    }
}
