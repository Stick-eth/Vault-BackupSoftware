using System;
using System.IO;
using Vault.ViewModel;
using Vault.Model;
using System.Text.Json;
using System.Xml;
using System.Text.Json.Serialization;
using System.Net.Http.Json;


namespace Vault.Model.Logger
{
    /**
     *\brief This class represent the RTLog. The RTLog is a class that will be used to log the backup in real time
     * It will be used to log the backup in real time
     *
     */
    internal sealed class RTLog
    {
        public string FileName { get; set; }
        public string PathName { get; set; }
        public ViewModel.ViewModelCore ViewModel {  get; set; }
        private readonly object rtlock = new object();

        private static RTLog _instance;

        /**
         * Constructor of the RTLog
         */
        private RTLog()
        {
            FileName = "currentState.json";
            PathName = @"C:/Vault";
            ViewModel = Vault.ViewModel.ViewModelCore.GetInstance();
        }

        /**
         * Get the instance of the RTLog
         *
         * @return the instance of the RTLog
         */
        public static RTLog GetInstance()
        {
            if (_instance == null)
            {
                _instance = new RTLog();
                _instance.CreateFile();
            }
            return _instance;
        }

        /**
         * Create the currentState.json file
         */
        public void CreateFile()
        {
            try
            {
                // Combine the full path of the directory with the file name
                string fullPath = Path.Combine(PathName, FileName);

                // Check if the file already exists
                if (!File.Exists(fullPath))
                {
                    // Create the directory if it doesn't exist already
                    Directory.CreateDirectory(PathName);

                    // Create the file
                    FileStream rfs = File.Create(fullPath);
                    rfs.Close();
                }
            }
            catch (Exception)
            {
                // Log errors
                throw new FileNotFoundException($"ErrorCreateRTLogFile|{Path.Combine(PathName, FileName)}");
            }
        }

        /**
         * Check if the currentState.json file exists
         *
         * @return true if the file exists, false otherwise
         */
        public bool FileExists()
        {
            string fullPath = Path.Combine(PathName, FileName);
            return File.Exists(fullPath);
        }

        /**
         * Delete the currentState.json file
         */
        public void WriteLiveState()
        {
            if (!FileExists())
            {
                CreateFile();
            }
            String LiveState = "[";
            var options = new JsonSerializerOptions { WriteIndented = true };
            // Retrieve all RealtimeStats of each BackupJob add it in the string and write the sting in the currentState.json file
            foreach (BackupJob backupJob in ViewModel.backupList.ToList())
            {
                LiveState += System.Environment.NewLine;
                LiveState += (JsonSerializer.Serialize(backupJob.realtimeStats, options));
                LiveState += ",";
            }
            LiveState = LiveState.Remove(LiveState.Length-1) + System.Environment.NewLine + "]";
            lock (rtlock)
            {
                File.WriteAllText(Path.Combine(PathName, FileName), LiveState);
            }
        }
    }
}
