using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Vault.Model;
using Vault.Model.Config;
using Vault.Model.Logger;
using Vault.Model.Uilities;
using System.ComponentModel;
using System.Text.Json;

/**
 * \brief This namespace contain all the class that will be used to store the configuration of the application
 *  
 */
namespace Vault.ViewModel
{
    /*!
    \class ViewModelCore
    \brief This class is the core of the ViewModel. It is the only class that can be instanciate and it is a singleton.
    It contains all the necessary model to run the application and the methods to interact with them.
     */
    public sealed class ViewModelCore
    {
        public List<BackupJob> backupList { get; set; }
        public List<BackupJob> runningJobs { get; set; }
        private FullBackupJobFactory fullBackupJobFactory { get; set; }
        private DifferentialBackupJobFactory differentialBackupJobFactory { get; set; }
        private SaveConfig saveConfig { get; set; }

        private static ViewModelCore _instance;
        public AppConfig appConfig { get; set; }
        public CurrentProcess currentProcess { get; set; }
        public PriorityExtensions priority { get; set; }
        public PriorityJobs priorityJob { get; set; }

        private Extension extension;

        public BusinessAppMonitoring businessAppMonitoring { get; set; }

        /**
         * Constructor: 
         * It is private because it is a singleton
         * 
         * Instanciate all the model necessary for running the application 
         */
        private ViewModelCore()
        {
            backupList = new List<BackupJob>();

            // Instanciate all the model necessary for running the application
            fullBackupJobFactory = new FullBackupJobFactory();
            differentialBackupJobFactory = new DifferentialBackupJobFactory();
            saveConfig = new SaveConfig();
            appConfig = new AppConfig();
            extension = Extension.GetInstance();
            priority = PriorityExtensions.GetInstance();
            priorityJob = PriorityJobs.GetInstance();

           // extension.AddToList("txt");
            currentProcess = new CurrentProcess();
            businessAppMonitoring = BusinessAppMonitoring.GetInstance();
        }

        /**
         *GetInstance: 
         *
         *Return the instance of the ViewModelCore
         *
         *Example :
         *ViewModelCore.GetInstance();
         * > Return the instance of the ViewModelCore
         */
        public static ViewModelCore GetInstance()
        {
            if (_instance == null)
            {
                _instance = new ViewModelCore();
            }
            return _instance;
        }

        /**
         * Select the lang by changing the Ressources filed use
         * @param language : string : the language to use
         * 
         * Example :
         * SetLanguage("Français");
         * > Change the language of the application to French
         */
        public void SetLanguage(string language)
        {
            if (language == "Français")
            {
                System.Globalization.CultureInfo.CurrentUICulture = new System.Globalization.CultureInfo("fr-FR");
                //Write it in the config.json file
                appConfig.SetLanguage("fr-FR");
            }
            else
            {
                System.Globalization.CultureInfo.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
                //Write it in the config.json file
                appConfig.SetLanguage("en-US");
            }
        }
        
        public string GetLanguage()
        {
            return appConfig.GetLanguage();
        }

        public void SetBandWidthLimit(long bandWidthLimit)
        {
            //Write it in the config.json file
            appConfig.SetBandWidthLimit(bandWidthLimit);
        }

        public long GetBandWidthLimit()
        {
            return appConfig.GetBandWidthLimit();
        }
        /**
         * Load all information written in the Config.json file
         */
        public void LoadConfig()
        {
            //Create config.json file if it does not exist
            appConfig.CreateFile();
            //Use the information read form the Config.json instead of the default values
            appConfig.LoadFile();
            //Apply change with this new values
            if (appConfig.GetLanguage() == "fr-FR")
            {
                SetLanguage("Français");
            }
            else
            {
                SetLanguage("English");
            }
            if (appConfig.GetExtension() == ".json")
            {
                SetLogJson();
            }
            else
            {
                SetLogXml();
            }
            SetBandWidthLimit(appConfig.GetBandWidthLimit());
            SetLogPath(appConfig.GetLogFilePath());
        }

        /**
         * Write all information in the Config.json file
        */
        public void WriteConfig()
        {
            appConfig.WriteToFile();
        }

        /**
         * Load all information written in the SaveConfig.json file
         */
        public void LoadSaveConfig()
        {
            if (!saveConfig.FileExists())
            {
                saveConfig.CreateFile();
            }
            else
            {
                // LoadSaveConfig is a class with the same attribute used so save BackupJob
                List<LoadSaveConfig> LoadSaveConfigs;
                try
                {
                    // Read SaveConfig.json and return a list of LoadSaveConfig 
                    LoadSaveConfigs = saveConfig.GetSave();
                }
                catch
                {
                    // Delete the SaveConfig.json file content if there is an error during the data retrieving
                    saveConfig.DeleteLines();
                    return;
                }
                saveConfig.DeleteLines();
                foreach (LoadSaveConfig save in LoadSaveConfigs)
                {
                    try
                    {
                        // Create the job with the attribute of the LoadSaveConfig object
                        CreateJob(save.Name, save.FileSource, save.FileTarget, save.SaveType);
                    }
                    catch (Exception)
                    {
                        // Pass to the next LoadSaveConfig if there is an error during the creation of the BackupJob
                        // The curent BackupJob in creation will not be instanciate
                        continue;
                    }
                }
            }
        }
        
        /**
         * Write all information in the SaveConfig.json file
         */
        public List<String> AllInfo()
        {
            List<String> info = new List<String>();
            foreach (BackupJob backup in backupList)
            {
                info.Add(backup.Info());
            }
            return info;
        }

        public string GetJobName(int index)
        {
            if (index > backupList.Count || index <= 0)
            {
                throw new ArgumentException("InvalidArgumentSelectionRangeError|" + backupList.Count);
            }
            return backupList[index - 1].Name;
        }   


        /**
         * Write all information in the SaveConfig.json file
         */
        public void AllScanTargetDirectory()
        {
            foreach (BackupJob backup in backupList)
            {
                backup.ScanTargetDirectory();
            }
        }

        /**
         *Write all information in the SaveConfig.json file
         *
         *@param name : string : the name of the BackupJob
         *@param sourceDirectory : string : the source directory of the BackupJob
         *@param targetDirectory : string : the target directory of the BackupJob
         *@param type : string : the type of the BackupJob
         *
         *Example :
         *CreateJob("BackupJob1", "C:/source", "D:/target", "full");
         *> Create a BackupJob with the name "BackupJob1", the source directory "C:/source", the target directory "D:/target" and the type "full"
         */
        public String CreateJob(String name, String sourceDirectory, String targetDirectory, String type)
        {
            //Verify if the name isn't already use
            foreach (BackupJob backup in backupList)
            {
                if (backup.Name == name)
                {
                    throw new ArgumentException("NameAlreadyUsedError");
                }
            }
            // Choose the right type of BackupJob to create depending of the type given
            switch (type.ToLower())
            {
                case "full":
                    //Call the fullBackupJobFactory to have a BackupJob with the rigth strategie
                    backupList.Add(fullBackupJobFactory.CreateBackupJob(name, sourceDirectory, targetDirectory));
                    //Write it in the SaveConfig.json
                    saveConfig.CreateSave(backupList.Last());
                    return $"{backupList.Last().Info()}";
                case "differential":
                    //Call the differentialBackupJobFactory to have a BackupJob with the rigth strategie
                    backupList.Add(differentialBackupJobFactory.CreateBackupJob(name, sourceDirectory, targetDirectory));
                    //Write it in the SaveConfig.json
                    saveConfig.CreateSave(backupList.Last());
                    return $"{backupList.Last().Info()}";
                default:
                    throw new ArgumentException("InvalidSaveTypeError|" + type);
            }
        }
        /**
         * Update the BackupJob with the index given
         * 
         *@param index : int : the index of the BackupJob to update
         *@param sourceDirectory : string : the new source directory of the BackupJob
         *@param targetDirectory : string : the new target directory of the BackupJob
         *
         *Example :
         *UpdateJob(1, "C:/source", "D:/target");
         * > Update the BackupJob with the index 1 with the new source directory "C:/source" and the new target directory "D:/target"
         * */
        public String UpdateJob(int index, string sourceDirectory, string targetDirectory, int Savetype)
        {
            if (index > backupList.Count || index <= 0)
            {
                throw new ArgumentException("InvalidArgumentSelectionRangeError|" + backupList.Count);
            }
            if (!Directory.Exists(sourceDirectory))
            {
                throw new ArgumentException($"InexistantSourceDirectoryError|{sourceDirectory}");
            }
            if (!Directory.Exists(targetDirectory))
            {
                throw new ArgumentException($"InexistantTargetDirectoryError|{targetDirectory}");
            }
            //Verify if the target directory is inside the sourceDirectory to avoid ForkBomb
            if (targetDirectory.Contains(sourceDirectory))
            {
                throw new ArgumentException("DirectoryTargetInsideSourceError");
            }
            //Update the BackupJob on the index-1 because BackupJobs number are displayed from 1 to ...
            backupList[index - 1].Update(sourceDirectory, targetDirectory);

            if (Savetype == 1)
            {
                // Full
                backupList[index - 1].saveType = new FullBackupStrategie();
            }
            else if (Savetype == 2)
            {
                //Differential
                backupList[index - 1].saveType = new DifferentialBackupStrategie();
            }
            else
            {
                throw new ArgumentException("InvalideSaveType");
            }
            //Overwrite it in the SaveConfig.json
            saveConfig.CreateAllSave(backupList);
            return backupList[index - 1].Info();
        }
        

        /**
         *Delete the BackupJob with the index given
         *
         *@param index : int : the index of the BackupJob to delete
         *
         *Example :
         *DeleteJob(1);
         *> Delete the BackupJob with the index 1
         */
        public void DeleteJob(int index)
        {
            if (index > backupList.Count || index <= 0) //
            {
                throw new ArgumentException("InvalidArgumentSelectionRangeError|" + backupList.Count);
            }
            //Delete the BackupJob on the index-1 because BackupJobs number are displayed from 1 to ...
            backupList.RemoveAt(index - 1);
            //Delete it in the SaveConfig.json
            saveConfig.CreateAllSave(backupList);
        }

        /**
         *Run the BackupJob with the index given
         *
         *@param index : int : the index of the BackupJob to run
         *
         *Example :
         *RunJob(1);
         *> Run the BackupJob with the index 1
         */
        public void RunJobs(String selection)
        {
            List<int> indexes = new List<int>();
            // Parse the string given => Selection
            if (selection.Contains(";"))
            {
                foreach (String indexe in selection.Split(";"))
                {
                    int number;
                    if (!int.TryParse(indexe, out number))
                    {
                        throw new ArgumentException("InvalidArgumentIntegersOnlyError");
                    }
                    else
                    {
                        if (number <= backupList.Count & number > 0)
                        {
                            indexes.Add(number - 1);
                        }
                        else
                        {
                            throw new ArgumentException("InvalidArgumentSelectionRangeError|" + backupList.Count);

                        }
                    }
                }
                LaunchOperations(indexes);

            }
            // Parse the string given => Range
            else if (selection.Contains("-"))
            {
                if (selection.Split("-").Count() < 3)
                {
                    for (int i = selection.Split("-").Select(int.Parse).ToList().Min() - 1; i < selection.Split("-").Select(int.Parse).ToList().Max(); i++)
                    {
                        if (i >= backupList.Count)
                        {
                            continue;
                        }
                        indexes.Add(i);
                    }
                    LaunchOperations(indexes);
                }
                else
                {
                    throw new ArgumentException("InvalidArgumentOneRangeError|" + backupList.Count);
                }
            }
            // Parse the string given => One BackupJobs
            else if (!selection.Contains(" "))
            {
                int number;
                if (int.TryParse(selection, out number) & number > 0 & number <= backupList.Count())
                {
                    indexes.Add(number-1);
                    LaunchOperations(indexes);
                }
                else
                {
                    throw new ArgumentException("InvalidArgumentSelectionsRangeError|" + backupList.Count);
                }
            }
            else
            {
                throw new ArgumentException("InvalidArgumentSelectionsDetailedError|" + backupList.Count);
            }
        }

        /***
         *
         *Launch the BackupJob with the index given
         *
         *@param indexes : List<int> : the indexes of the BackupJob to run
         */
        private void LaunchOperations(List<int> indexes)
        {

            foreach (int index in indexes)
            {
                //Verify if there is process that prevent BackupJob to execute
                //currentProcess.CheckRunningProcess();

                //Run backupList[index].Run(); in a new thread
                /*Task.Run(() => 
                    backupList[index].Run());   */
                if (backupList[index].realtimeStats.State == "running")
                {
                    continue;
                }
                Thread thread = new Thread(() =>
                {
                    backupList[index].Run();
                });
                thread.Start();
                Thread.Sleep(150);

            }
        }


        public void SetMonitoredProcess(List<string> processName)
        {
            try
            {
                appConfig.SetProcess(processName);
            }
            catch (Exception)
            {
                throw new ArgumentException("ErrorAddProcess|" + processName);
            }

        }

        public List<string> GetMonitoredProcess()
        {
            return appConfig.GetMonitoredProcesses();
        }
        
        /**
         * Define the log file extension to JSON
         */
        public void SetLogJson()
        {
            Log.GetInstance().SettingJson();
            //Save it in the Config.json file
            appConfig.SetExtension(".json");
        }


        /**
         * Set the log file extension to XML
         */
        public void SetLogXml()
        {
            Log.GetInstance().SettingXml();
            //Save it in the Config.json file
            appConfig.SetExtension(".xml");
        }

        public void SetLogPath(string path)
        {
            Log.GetInstance().SetPath(path);
            appConfig.SetLogFilePath(path);
        }

        public string GetLogPath()
        {
            return appConfig.GetLogFilePath();
        }

        /**
         * Return the extension log format used
         */
        public string GetLogExtension()
        {
            if (appConfig.GetExtension() == ".json")
            {
                return ".json";
            }
            else
            {
                return ".xml";
            }
        }

        public void SetExtension(List<string> list)
        {
            appConfig.SetListExtension(list);
        }

        public List<string> GetExtension()
        {
            return appConfig.GetListExtension();
        }

        public void SetPriority(List<string> list)
        {
            appConfig.SetPriorityExtension(list);
        }

        public List<string> GetPriority()
        {
            return appConfig.GetPriorityExtension();
        }

        public void AddRunningJob(BackupJob job)
        {
            runningJobs.Add(job);
        }

        public void RemoveRunningJob(BackupJob job)
        {
            runningJobs.Remove(job);
        }



        /* ------------------------ Server ------------------------ */



        private BackgroundWorker serverWorker = new BackgroundWorker();
        private Socket serverSocket;
        private bool isRunning = false;
        private Socket clientSockets;

        public bool IsRunning()
        {
            return isRunning;
        }
        public void StartServer()
        {
            BackupManager.GetInstance().UpdateBackupJob += SendRunningJobList;
            serverWorker.DoWork += new DoWorkEventHandler(serverWorker_DoWork);
            serverWorker.RunWorkerAsync();
        }

        private void serverWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(new IPEndPoint(IPAddress.Any, 12324)); 
            serverSocket.Listen(10);
            isRunning = true;

            while (isRunning)
            {
                try
                {
                    Socket clientSocket = serverSocket.Accept(); 
                    clientSockets = clientSocket;
                    SendUpdate(clientSocket);
                    BeginReceive(clientSocket);
                }
                catch (SocketException ex)
                {
                    Console.WriteLine($"SocketException in Accept: {ex.Message}");
                }
                catch (ObjectDisposedException)
                {
                    break;
                }
            }
        }
        class StateObject
        {
            // Size of receive buffer.
            public const int BufferSize = 1024;
            // Receive buffer.
            public byte[] buffer = new byte[BufferSize];
            // Client socket.
            public Socket workSocket = null;
        }

        private void BeginReceive(Socket socket)
        {
            StateObject state = new StateObject();
            state.workSocket = socket; 
            socket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
        }


        public void SendUpdate(Socket socket)
        {
            SendJobList(socket);
            //SendRunningJobList(socket);
        }

        private void ReceiveCallback(IAsyncResult AR)
        {
            StateObject state = (StateObject)AR.AsyncState;
            Socket socket = state.workSocket;
            try
            {
                int received = socket.EndReceive(AR);
                if (received > 0)
                {
                    byte[] data = new byte[received];
                    Array.Copy(state.buffer, data, received); 
                    string text = Encoding.ASCII.GetString(data);

                    if (text.Contains("RunJobs"))
                    {
                        RunJobs(text.Split("RunJobs")[1]);
                    }
                    else if (text.Contains("PauseResume"))
                    {
                        PauseResume(text.Split("PauseResume")[1]);
                    }
                    else if (text.Contains("Stop"))
                    {
                        StopJobs(text.Split("Stop")[1]);
                        
                    }
                    BeginReceive(socket); 

                }
                else
                {
                    socket.Close();
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"SocketException in ReceiveCallback: {ex.Message}");
                socket.Close();
            }
            catch (ObjectDisposedException)
            {
                // This can happen normally when the socket is closed
            }
        }

        public void SendJobList(Socket socket)
        {
            byte[] buffer = Encoding.ASCII.GetBytes("AllJobs"+BackupJob.SerializeJobs(backupList));
            socket.Send(buffer);
        }

        private void PauseResume(string index)
        {
            BackupManager backupManager = BackupManager.GetInstance();
            backupManager.PauseResume(int.Parse(index));
        }

        public void StopJobs(string index)
        {
            BackupManager backupManager = BackupManager.GetInstance();
            backupManager.Stop(int.Parse(index));
        }


        public void SendRunningJobList(object? sender, EventArgs e)
        {

            BackupManager backupManager = BackupManager.GetInstance();
            Encoding.ASCII.GetBytes("RunningJobs" + BackupJob.SerializeActiveJobs(backupManager.GetJobs()));
            string sendMsg = BackupJob.SerializeActiveJobs(backupManager.GetJobs());
            if (sendMsg == (System.Environment.NewLine + "]"))
            {
                return;
            }
            Trace.WriteLine("Send: " + sendMsg);
            byte[] buffer = Encoding.ASCII.GetBytes("RunningJobs" + sendMsg);
            clientSockets.Send(buffer);

        }

        public void StopServer()
        {
            if (isRunning)
            {
                serverSocket.Close();
                isRunning = false;
            }
        }

    }
}
