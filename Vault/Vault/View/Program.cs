using Vault.ViewModel;
using System.Resources;
using Vault.Model.Config;
using Vault.Model.Logger;

/** \mainpage Vault Documentation
* Application made by students of CESI, for the Software Engineering course.
* Credits to the students: Kadijé PAYE, Axel EUDIER, Luc MAERTEN & Aniss SEJEAN
*
* \section intro_sec Introduction
* The application is a backup software that will be used to backup files and directories. In order to do so, the application will use a configuration file to know what to backup and where to backup it.
* Please take a look at the diagram following this link:
*
* https://aniss-sej.notion.site/Vault-Diagrams-58edfec9ffdc4f0fb97d54bdd1abc247?pvs=4 
* 
* \section install_sec Installation
* To install the application, you will need to download the .exe file. A .json file will be created to store the configuration of the application.
* 
* \section core_sec Vault.Core
* The Core namespace contains all the classes that will be used to manage the backup of the files and directories.
*It is composed of the following classes:
* - BackupJob
* - BackupJobFactory
* - iBackupStrategie
* - RecursiveCopy
* - SaveConfig
* - AppConfig
*
* \section gui_sec Vault.GUI
* The GUI namespace contains all the classes that will be used to display the application.
*It is composed of the following classes:
* - MenuView
* - Program
* - ViewModelCore
*
*/
namespace Vault.View
{
    /**
     *\brief This class represent the Program. The Program is a class that will be used to start the application
     */
    class Program
    {
        /**
         * The main method of the application
         *
         * @param args : the arguments of the application
         */
        static void Main(string[] args)
        {
            if (System.Diagnostics.Process.GetProcessesByName(System.Diagnostics.Process.GetCurrentProcess().ProcessName).Length > 1)
            {
                return;
            }
            Console.ResetColor();
            ViewModel.ViewModelCore viewModel = ViewModel.ViewModelCore.GetInstance();
           // viewModel.AddProcess("notepad");
            viewModel.LoadSaveConfig();
            viewModel.LoadConfig();
            Log.GetInstance();


            //viewModel.SetLogPath("D:/Vault/Log");
            MenuView view = new MenuView(viewModel);
            while (true)
            {
                view.DisplayMenu();
            }
        }
    }

}
