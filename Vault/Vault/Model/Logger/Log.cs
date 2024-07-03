using System;
using System.IO;
using Vault.Model.Config;

/**
 * \brief This class represent the Log. The Log is a class that will be used to log the backup
 */
namespace Vault.Model.Logger
{
    /**
    *\brief This class represent the Log. The Log is a class that will be used to log the backup
    * It will be used to log the backup
    *
    */
    public sealed class Log
    {
        // Attribute fileName with default value based on current date
        public string FileName { get; set; }
        // Attribute path with default value
        public string PathName { get; set; }

        private int log_type {  get; set; }
        // 1 = Json ; 2 XML

        private readonly object loglock = new object();

        private static Log _instance;

        /**
         * Constructor of the Log
         */
        private Log()
        {
            FileName = DateTime.Now.ToString("yyyy-MM-dd") + ".json";
            PathName = AppConfig.LogFilePath;
            log_type = 1;
        } 

        /**
         * Get the instance of the Log
         *
        * @return the instance of the Log
         */
        public int LogType
        {
            get
            {
                return log_type;
            }
        }

        /**
         * Get the instance of the Log
         *
         * @return the instance of the Log
         */
        public static Log GetInstance()
        {
            if (_instance == null)
            {
                _instance = new Log();
            }
            return _instance;
        }

        /**
         * Set the path of the log file
         *
         * @param path : the path of the log file
         */
        public void SettingJson()
        {
            log_type = 1;
            // Chang the path of the log file
            FileName = FileName.Split('.')[0]+".json";
        }

        /**
        *Change the log format to XML
        */
        public void SettingXml()
        {
            log_type = 2;
            // Chang the path of the log file
            FileName = FileName.Split('.')[0] + ".xml";
        }

        public void SetPath(string newPath)
        {
            if (!PathName.Equals(newPath, StringComparison.OrdinalIgnoreCase))
            {
                Directory.CreateDirectory(PathName);
                DirectoryInfo sourceFolder = new DirectoryInfo(PathName);
                DirectoryInfo destinationFolder = new DirectoryInfo(newPath);

                CopyAll(sourceFolder, destinationFolder);

                Directory.Delete(PathName, true);
            }

            PathName = newPath;
        }

        private void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            Directory.CreateDirectory(target.FullName);
            foreach (FileInfo fileInfo in source.GetFiles())
            {
                fileInfo.CopyTo(Path.Combine(target.FullName, fileInfo.Name), true);
            }
            foreach (DirectoryInfo subDirectory in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir = target.CreateSubdirectory(subDirectory.Name);
                CopyAll(subDirectory, nextTargetSubDir);
            }
        }

        // Method to create a file
        /**
        * Method to create a file
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
                    FileStream fs = File.Create(fullPath);
                    fs.Close();

                }
            }
            catch (Exception)
            {
                // Log errors
                throw new FileNotFoundException($"ErrorCreateLogFile|{Path.Combine(PathName, FileName)}");
            }
        }

        /**
        *Method to check if a file exists
        */
        public bool FileExists()
        {
            string fullPath = Path.Combine(PathName, FileName);
            return File.Exists(fullPath);
        }

        /**
        *Write the content in the file log
        *
        *@param name : the name of the backup
        *@param fsource : the source file
        *@param ftarget : the target file
        *@param fsize : the size of the file
        *@param fttime : the time of the transfer
        *@param cryptTime : the time of the crypt
        */
        public void WriteContent(string name, string fsource, string ftarget, int fsize, double fttime, int cryptTime)
        {
            try
            {
                // Verify if the file exist
                if (!FileExists())
                {
                    // If not, create it
                    CreateFile();
                }

                // Combine the full path of the directory with the file name
                string fullPath = Path.Combine(PathName, FileName);

                // Format the current date and time
                string currentTime = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");

                string Content;
                // Format the content to append to the file depending of the extension used
                if (log_type ==1) 
                {
                    Content =
                    $"{{\n\"Name\": \"{name}\",\n\"FileSource\": \"{fsource}\",\n\"FileTarget\": \"{ftarget}\",\n\"FileSize\": \"{fsize}\",\n\"FileTransferTime\": \"{fttime}\",\n\"time\": \"{currentTime}\",\n\"EncryptTime\": \"{cryptTime}\"\n}}\n";

                }
                else if (log_type ==2)
                {
                    Content = $"<log>\n" +
                    $"    <Name>{name}</Name>\n" +
                    $"    <FileSource>{fsource}</FileSource>\n" +
                    $"    <FileTarget>{ftarget}</FileTarget>\n" +
                    $"    <FileSize>{fsize}</FileSize>\n" +
                    $"    <FileTransferTime>{fttime}</FileTransferTime>\n" +
                    $"    <time>{currentTime}</time>\n" +
                    $"</log>\n";
                }
                else
                {
                    Content = "Error";
                }

                lock (loglock)
                {
                // Append the content to the file
                File.AppendAllText(fullPath, Content);
                }
                

            }
            catch (Exception)
            {
                // Log errors
                throw new FileLoadException($"ErrorWriteLogFile|{Path.Combine(PathName, FileName)}");
            }
        }
    }
}
