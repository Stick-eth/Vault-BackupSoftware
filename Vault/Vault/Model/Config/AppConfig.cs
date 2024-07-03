using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Vault.Model.Logger;
using Vault.Model.Uilities;
using static System.Runtime.InteropServices.JavaScript.JSType;


/**
 *\brief This namespace contain all the class that will be used to store the configuration of the application
 * 
 */
namespace Vault.Model.Config
{

    /**
     *\brief This class represent the AppConfig. The AppConfig is a class that will be used to store all the configuration of the application
     *It will be used to store the language, the extension of the file, the list of the monitored processes and the list of the extension
     *
     */
    public class AppConfig
    {
        private string FilePath { get; set; } = $"C:/Vault/Config.json";
        public static string language { get; set; } = "fr-FR"; // 1 = French, 2 = English
        public static string extension { get; set; } = ".json"; // 1 = .json, 2 = .xml
        public static string LogFilePath { get; set; } = $"C:/Vault/Log";
        public static long bandWidth { get; set; }
        public static List<string> monitoredProcesses { get; set; } = new List<string>();

        /**
         *Constructor of the AppConfig
         *
         *param path : the path of the file
         */
        public void CreateFile()
        {
            try
            {
                if (!System.IO.File.Exists(FilePath))
                {
                    System.IO.File.Create(FilePath).Close();
                    System.IO.File.WriteAllText(FilePath, $"{{\"language\":\"{language}\",\"extension\":\"{extension}\"}}");
                }
            }
            catch (Exception)
            {
                throw new System.IO.FileLoadException($"ErrorCreateAppConfigFile|{FilePath}");
            }
        }

        /**
         *Set the language of the application
         *
         *@param e : the language
         */
        public void SetLanguage(string e)
        {
            language = e;
            WriteToFile();
        }

        /**
         * Set the extension of the file
         *
         *@param e : the extension
         */
        public void SetExtension(string e)
        {
            extension = e;
            WriteToFile();

        }

        /**
         *Set the path of the file
         *
         *@param path : the path
         */
        public void SetFilePath(string path)
        {
            //Define the path of the file
            FilePath = path;
        }

        /**
         *Get the language of the application
         *
         *return : int
         */
        public string GetLanguage()
        {
            return language;
        }

        /**
         *Get the extension of the file
         *
         *return : int
         */
        public string GetExtension() 
        { 
            return extension; 
        }

        public void SetProcess(List<string> processName)
        {
            monitoredProcesses.Clear();
            foreach (string process in processName)
            {                
                    monitoredProcesses.Add(process);
            }
            WriteToFile();
        }

        /**
         *Get the path of the file
         *
         *return : string
         */
        public void AddProcess(string processName)
        {
            if (!monitoredProcesses.Contains(processName))
            {
                monitoredProcesses.Add(processName);
                WriteToFile();
            }
        }

        /**
         *Get the path of the file
         *
         *return : string
         */
        public void RemoveProcess(string processName)
        {
            if (monitoredProcesses.Contains(processName))
            {
                monitoredProcesses.Remove(processName);
                WriteToFile();
            }
        }

        /**
         *Get the path of the file
         *
         *return : string
         */
        public List<string> GetMonitoredProcesses()
        {
            return monitoredProcesses;
        }

        /**
         *Get the path of the file
         *
         *return : string
         */
        public List<string> GetListExtension()
        {
            return Extension.GetInstance().GetExtensionList();
        }

        public long GetBandWidthLimit()
        {
            return bandWidth;
        }

        public void SetBandWidthLimit(long bandWidthLimit)
        {
            bandWidth = bandWidthLimit;
            WriteToFile();
            BandWidthManager.GetInstance().SetLimit(bandWidthLimit);
        }
        /**
         *Get the list of the priority extension
         *
         * return : List<string>
         */
        public List<string> GetPriorityExtension()
        {
            return PriorityExtensions.GetInstance().GetExtensionList();
        }

        
        /**
         *Set the list of the extension
         *
         *@param list : the list of the extension
         */
        public void SetListExtension(List<string> list)
        {
            Extension.GetInstance().loadListExtension(list);
            WriteToFile();
        }

        /**
         *Set the list of the priority extension
         *
         *@param list : the list of the priority extension
         */
        public void SetPriorityExtension(List<string> list)
        {
            PriorityExtensions.GetInstance().loadListExtension(list);
            WriteToFile();
        }

        public void SetLogFilePath(string path)
        {
            LogFilePath = path;
            WriteToFile();
        }

        public string GetLogFilePath()
        {
            return LogFilePath;
        }

        /**
         *Write the configuration in the file
         *
         */
        public void WriteToFile()
        {
            CreateFile();
            System.IO.File.WriteAllText(FilePath, $"{{\"language\":\"{language}\",\"extension\":\"{extension}\", \"logpath\":\"{LogFilePath}\", \"monitoredProcesses\":{JsonSerializer.Serialize(monitoredProcesses)}, \"listExtension\":{JsonSerializer.Serialize(GetListExtension())}, \"priorityExtension\":{JsonSerializer.Serialize(GetPriorityExtension())}, \"bandwidthLimit\":{JsonSerializer.Serialize(bandWidth)}}}");
               
        }

        /**
         *Load the configuration from the file
         *
         */
        public void LoadFile()
        {
            if (!File.Exists(FilePath))
            {
                CreateFile();
            }
            else
            {
                string jsonContent = File.ReadAllText(FilePath);
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var data = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(jsonContent, options);
                if (data != null)
                {
                    // Directement affecter la valeur string sans conversion en int
                    if (data.ContainsKey("language") && data["language"].ValueKind == JsonValueKind.String)
                    {
                        language = data["language"].GetString();
                    }
                    if (data.ContainsKey("extension") && data["extension"].ValueKind == JsonValueKind.String)
                    {
                        extension = data["extension"].GetString();
                    }
                    if (data.ContainsKey("logpath") && data["logpath"].ValueKind == JsonValueKind.String)
                    {
                        LogFilePath = data["logpath"].GetString();
                    }
                    if (data.ContainsKey("bandwidthLimit") && data["bandwidthLimit"].ValueKind == JsonValueKind.Number)
                    {
                        bandWidth = data["bandwidthLimit"].GetInt64();
                    }
                    if (data.ContainsKey("monitoredProcesses") && data["monitoredProcesses"].ValueKind == JsonValueKind.Array)
                    {
                        monitoredProcesses = data["monitoredProcesses"].EnumerateArray().Select(item => item.GetString()).Where(s => s != null).ToList();
                    }
                    if (data.ContainsKey("listExtension") && data["listExtension"].ValueKind == JsonValueKind.Array)
                    {
                        SetListExtension(data["listExtension"].EnumerateArray().Select(item => item.GetString()).Where(s => s != null).ToList());
                    }
                    if (data.ContainsKey("priorityExtension") && data["priorityExtension"].ValueKind == JsonValueKind.Array)
                    {
                        SetPriorityExtension(data["priorityExtension"].EnumerateArray().Select(item => item.GetString()).Where(s => s != null).ToList());
                    }


                }
            }
        }

    }
}
