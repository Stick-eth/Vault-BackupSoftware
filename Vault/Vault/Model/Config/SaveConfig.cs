using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using Vault.Model;
using System;
using System.IO;
using Vault.ViewModel;
using System.Threading.Tasks;

namespace Vault.Model.Config
{
    /**
     *\brief This class represent the SaveConfig. The SaveConfig is a class that will be used to store all the configuration of the application
     * It will be used to store the list of the BackupJob
     *
     * Information about the BackupJob will be stored in the SaveConfig.json file
     * The SaveConfig.json file will be used to reinstanciate the BackupJob when the application is restarted
     */
    class LoadSaveConfig
    {
        public string Name { get; set; }
        public string FileSource { get; set; }
        public string FileTarget { get; set; }
        public string SaveType { get; set; }

    }

    /**
     * This class represent the SaveConfig. The SaveConfig is a class that will be used to store all the configuration of the application
     * It will be used to store the list of the BackupJob
     *
     * Information about the BackupJob will be stored in the SaveConfig.json file
     * The SaveConfig.json file will be used to reinstanciate the BackupJob when the application is restarted
     */
    internal class SaveConfig
    {
        public string FileName { get; set; } = "SaveConfig.json";
        public string PathName { get; set; } = @"C:/Vault";

        /**
        * Create the SaveConfig.json file
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
                throw new FileLoadException($"ErrorCreateSaveFile|{Path.Combine(PathName, FileName)}");
            }
        }

        /**
         * Check if the SaveConfig.json file exists
         *
         * @return true if the file exists, false otherwise
         */
        public bool FileExists()
        {
            string fullPath = Path.Combine(PathName, FileName);
            return File.Exists(fullPath);
        }

        /**
         * Delete the SaveConfig.json file
         */
        public void DeleteLines()
        {
            if (!FileExists())
            {
                CreateFile();
            }
            File.WriteAllText(Path.Combine(PathName, FileName), string.Empty);
        }

        /**
         * Create the SaveConfig.json file
         *
         *@param backupJobs : the list of the BackupJob
         */
        public void CreateAllSave(List<BackupJob> backupJobs)
        {
            DeleteLines();
            foreach (BackupJob backupJob in backupJobs)
            {
                CreateSave(backupJob);
            }
        }

        /**
         * Create a SaveConfig.json file
         *
         *@param job : the BackupJob
         */
        public void CreateSave(BackupJob job)
        {
            try
            {
                if (!Directory.Exists(PathName))
                {
                    Directory.CreateDirectory(PathName);
                }

                string fullPath = Path.Combine(PathName, FileName);

                // Create anonymous object containing necassary atribute about the BackupJob
                var saveData = new
                {
                    job.Name,
                    FileSource = job.SourceDirectory,
                    FileTarget = job.TargetDirectory,
                    SaveType = job.saveType.Type()
                };

                // Read content already in the SaveConfig.json
                string jsonContent = File.Exists(fullPath) ? File.ReadAllText(fullPath) : "";

                List<object> savesList;

                // Deserialize content to create object and add them in a list
                if (!string.IsNullOrEmpty(jsonContent))
                {
                    savesList = JsonSerializer.Deserialize<List<object>>(jsonContent);
                }
                else
                {
                    savesList = new List<object>();
                }
                // Add the BackupJob to the list as well
                savesList.Add(saveData);

                // Export the list in string
                string newJsonContent = JsonSerializer.Serialize(savesList, new JsonSerializerOptions
                {
                    WriteIndented = true 
                });

                // Write it to the SaveConfig.json
                File.WriteAllText(fullPath, newJsonContent);
   
            }
            catch (Exception)
            {
                throw new FileLoadException($"ErrorWriteSaveFile|{Path.Combine(PathName, FileName)}");
            }
        }

        /**
        *Read the SaveConfig.json file and return list of LoadSaveConfig (containing information to reinstanciate the BackupJob)
        *
        *@return : List<LoadSaveConfig>
        */
        public List<LoadSaveConfig> GetSave()
        {
            List<LoadSaveConfig> savesList = new List<LoadSaveConfig>();
            if (!FileExists())
            {
                CreateFile();
            }
            // Read the SaveConfig.json file
            string fullPath = Path.Combine(PathName, FileName);
            string jsonContent = File.ReadAllText(fullPath);
            // Save the stext in a list of oadSaveConfig
            savesList = JsonSerializer.Deserialize<List<LoadSaveConfig>>(jsonContent);

            return savesList;
            
        }
    }
}
