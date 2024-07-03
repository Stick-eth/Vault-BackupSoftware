using System;
using System.Resources;
using System.Text;
using Vault.ViewModel;

/**
 * \brief This namespace contain all the class that will be used to display the menu of the application
 */

namespace Vault.View
{
    /**
    *\brief This class represent the MenuView. The MenuView is a class that will be used to display the menu of the application
    */
    internal class MenuView
    {
        private readonly ViewModel.ViewModelCore _viewModel;

        private readonly ResourceManager rm = new ResourceManager("Vault.Ressources.Texts", typeof(MenuView).Assembly);

        /**
        * Constructor of the MenuView
        *
        * @param viewModel : the viewModel of the application
        */
        public enum MenuState
        {
            Main,
            Save,
            Settings,
            Languages,
            Log
        }

        private MenuState _currentState = MenuState.Main;

        /**
        * Constructor of the MenuView
        *
        * @param viewModel : the viewModel of the application
        */
        public MenuView(ViewModel.ViewModelCore viewModel)
        {
            _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel)); // throw exception if viewModel is null
            OnStateChanged += DisplayMenu;
        }

        /**
        * Get the current state of the menu
        *
        * @return the current state of the menu
        */
        public MenuState CurrentState
        {
            get => _currentState;
            private set
            {
                _currentState = value;
                OnStateChanged?.Invoke();
                DisplayMenu(); 
            }
        }


        public Action OnStateChanged { get; set; }

        /**
        * Select the option of the menu
        * 
        * @param option : the option of the menu
        */
        public void SelectOption(int option)
        {
            switch (CurrentState)
            {
                case MenuState.Main:
                    HandleMainStateOption(option);
                    break;
                case MenuState.Save:
                    HandleSaveOption(option);
                    break;
                case MenuState.Languages:
                    HandleLanguageOption(option);
                    break;
                case MenuState.Settings:
                    HandleGeneralSettingsOption(option);
                    break;
                case MenuState.Log:
                    HandleLogOption(option);
                    break;
                default:
                    ShowInvalidOptionMessage();
                    break;
            }
        }

        /**
        * Handle the main state option
        *
        * @param option : the option of the main state
        */
        private void HandleMainStateOption(int option)
        {
            switch (option)
            {
                case 1:
                    CurrentState = MenuState.Save;
                    break;
                case 2:
                    CurrentState = MenuState.Settings;
                    break;
                case 3:
                    ExitApplication();
                    break;
                default:
                    ShowInvalidOptionMessage();
                    break;
            }
        }

        /**
        * Handle the save state option
        *
        * @param option : the option of the save state
        */
        private void HandleSaveOption(int option)
        {
            switch (option)
            {
                case 1:
                    ExecuteJob();
                    WaitKey();
                    break;
                case 2:
                    ShowAllJobs();
                    WaitKey();
                    break;
                case 3:
                    CreateJob();
                    WaitKey();
                    break;
                case 4:
                    EditJob();
                    WaitKey();
                    break;
                case 5:
                    DeleteJob();
                    WaitKey();
                    break;
                case 0:
                    CurrentState = MenuState.Main;
                    break;
                default:
                    ShowInvalidOptionMessage();
                    break;
            }
        }

        private void HandleLanguageOption(int option)
        {
            switch (option)
            {
                case 1:
                    SetLanguage("Français");
                    break;
                case 2:
                    SetLanguage("English");
                    break;
                case 0:
                    CurrentState = MenuState.Settings;
                    break;
                default:
                    ShowInvalidOptionMessage();
                    break;
            }
        }

        private void HandleGeneralSettingsOption(int option)
        {
            switch (option)
            {
                case 1:
                    CurrentState = MenuState.Languages;
                    break;
                case 2:
                    CurrentState = MenuState.Log;
                    break;
                case 0:
                    CurrentState = MenuState.Main;
                    break;
                default:
                    ShowInvalidOptionMessage();
                    break;
            }
        }

        private void HandleLogOption(int option)
        {
            switch (option)
            {
                case 1:
                    _viewModel.SetLogJson();
                    break;
                case 2:
                    _viewModel.SetLogXml();
                    break;
                case 0:
                    CurrentState = MenuState.Settings;
                    break;
                default:
                    ShowInvalidOptionMessage();
                    break;
            }
        }

        private void WaitKey()
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine(rm.GetString("PressAnyKey"));
            Console.ResetColor();
            Console.ReadKey();
        }

        public void DisplayMenu()
        {
            switch (CurrentState)
            {
                case MenuState.Main:
                    DisplayMainMenu();
                    break;
                case MenuState.Save:
                    DisplaySaveMenu();
                    break;
                case MenuState.Settings:
                    DisplayGeneralSettingsMenu();
                    break;
                case MenuState.Languages:
                    DisplayChangementDeLangueMenu();
                    break;
                case MenuState.Log:
                    DisplayLogMenu();
                    break;
            }
        }

        private void ShowInvalidOptionMessage()
        {
            Console.WriteLine(rm.GetString("InvalidOption"));
        }

        private void ExitApplication()
        {
            Environment.Exit(0);
        }

        private void DisplayMenuWithFrame(string[] menuItems)
        {
            Console.Clear();
            int windowWidth = Console.WindowWidth;
            string artASCII = ">>=======================================================<<\r\n||                                                       ||\r\n||   __   __   ______     __  __     __         ______   ||\r\n||  /\\ \\ / /  /\\  __ \\   /\\ \\/\\ \\   /\\ \\       /\\__  _\\  ||\r\n||  \\ \\ \\'/   \\ \\  __ \\  \\ \\ \\_\\ \\  \\ \\ \\____  \\/_/\\ \\/  ||\r\n||   \\ \\__|    \\ \\_\\ \\_\\  \\ \\_____\\  \\ \\_____\\    \\ \\_\\  ||\r\n||    \\/_/      \\/_/\\/_/   \\/_____/   \\/_____/     \\/_/  ||\r\n||                                                  v1.1 ||\r\n>>=======================================================<<";
            string[] artLines = artASCII.Split("\r\n");

            int artWidth = artLines.Max(line => line.Length);
            int paddingArt = (windowWidth - artWidth) / 2;
            if (paddingArt < 0) paddingArt = 0;

            foreach (string line in artLines)
            {
                Console.WriteLine(line.PadLeft(line.Length + paddingArt));
            }

            string topBottomBorder = $"┌{new string('─', windowWidth - 2)}┐";
            Console.WriteLine(topBottomBorder); 

            if (menuItems.Length > 0)
            {
                string title = menuItems[0];
                int padding = (windowWidth - title.Length) / 2 - 1;
                Console.ForegroundColor = ConsoleColor.Cyan;
                if (padding < 0) padding = 0;
                string paddedTitle = title.PadLeft(title.Length + padding).PadRight(windowWidth - 2);
                Console.WriteLine($"│{paddedTitle}│");
                Console.ResetColor();
            }

            for (int i = 1; i < menuItems.Length; i++)
            {
                string menuItem = menuItems[i].PadRight(windowWidth - 4);
                Console.WriteLine($"│ {menuItem} │");
            }

            Console.WriteLine($"└{new string('─', windowWidth - 2)}┘");
        }


        private void DisplayMainMenu()
        {
            string[] menuItems = {
                rm.GetString("MainMenu_Title"),
                rm.GetString("MainMenu_Option1"),
                rm.GetString("MainMenu_Option2"),
                rm.GetString("MainMenu_Option3")
            };

            DisplayMenuWithFrame(menuItems);
            PromptForOption();
        }


        private void DisplaySaveMenu()
        {
            string[] menuItems =
            {
                rm.GetString("JobMenu_Title"),
                rm.GetString("JobMenu_Option1"),
                rm.GetString("JobMenu_Option2"),
                rm.GetString("JobMenu_Option3"),
                rm.GetString("JobMenu_Option4"),
                rm.GetString("JobMenu_Option5"),
                rm.GetString("Return")
                };

            DisplayMenuWithFrame(menuItems);

            PromptForOption();
        }

        private void DisplayGeneralSettingsMenu()
        {
            string[] menuItems =
            {
                rm.GetString("GeneralSettingsMenu_Title"),
                rm.GetString("GeneralSettingsMenu_Option1"),
                rm.GetString("GeneralSettingsMenu_Option2"),
                rm.GetString("Return")
            };
            DisplayMenuWithFrame(menuItems);
            PromptForOption();
        }

        private void DisplayChangementDeLangueMenu()
        {
            string[] menuItems =
            {
                rm.GetString("LanguageMenu_Title"),
                rm.GetString("LanguageMenu_Option1"),
                rm.GetString("LanguageMenu_Option2"),
                rm.GetString("Return")
            };
            DisplayMenuWithFrame(menuItems);
            PromptForOption();
        }

        private void DisplayLogMenu()
        {
            string[] menuItems =
            {
                rm.GetString("LogMenu_Title"),
                rm.GetString("LogMenu_Option1"),
                rm.GetString("LogMenu_Option2"),
                rm.GetString("Return"),
                rm.GetString("LogMenu_CurrentExtension")  + _viewModel.GetLogExtension()
            };
            DisplayMenuWithFrame(menuItems);
            PromptForOption();
        }

        public void CreateJob()
        {
            Console.WriteLine();
            Console.WriteLine(rm.GetString("CreateJobMenu_Title"));
            Console.WriteLine();
            Console.WriteLine(rm.GetString("CreateJobMenu_Option1"));
            string name = ReadLineOrEscape();
            if (name == null) return;
            Console.WriteLine(rm.GetString("CreateJobMenu_Option2"));
            string source = ReadLineOrEscape();
            if (source == null) return;
            Console.WriteLine(rm.GetString("CreateJobMenu_Option3"));
            string destination = ReadLineOrEscape();
            if (destination == null) return;
            Console.WriteLine(rm.GetString("CreateJobMenu_Option4"));
            string type = ReadLineOrEscape();
            if (type == null) return;

            try
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(_viewModel.CreateJob(name, source, destination, type));
                Console.ResetColor();
            }
            catch (ArgumentException e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                if (e.Message.Contains("|"))
                {
                    string[] error = e.Message.Split('|');
                    Console.WriteLine(rm.GetString(error[0]) + error[1]);
                    Console.ResetColor();
                    return;
                }
                else
                {
                    Console.WriteLine(rm.GetString(e.Message));
                    Console.ResetColor();
                    return;
                }
            }
        }
        private string ReadLineOrEscape()
        {
            StringBuilder input = new StringBuilder();
            while (true)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                if (keyInfo.Key == ConsoleKey.Escape || keyInfo.KeyChar == '0')
                {
                    return null;
                }
                else if (keyInfo.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine(); 
                    break;
                }
                else if (keyInfo.Key == ConsoleKey.Backspace && input.Length > 0)
                {
                    input.Remove(input.Length - 1, 1);
                    Console.Write("\b \b");
                }
                else if (!char.IsControl(keyInfo.KeyChar))
                {
                    input.Append(keyInfo.KeyChar);
                    Console.Write(keyInfo.KeyChar);
                }
            }
            return input.ToString();
        }

        public void EditJob()
        {
            Console.WriteLine();
            Console.WriteLine(rm.GetString("EditJobMenu_Title"));
            Console.WriteLine();
            bool empty = ShowAllJobs();
            if (empty)
            {
                return;
            }
            Console.WriteLine(rm.GetString("Return"));
            Console.WriteLine();

            Console.WriteLine(rm.GetString("EditJobMenu_Option1"));
            string index = ReadLineOrEscape();
            if (index == null) return;

            Console.WriteLine(rm.GetString("EditJobMenu_Option2"));
            string source = ReadLineOrEscape();
            if (source == null) return;

            Console.WriteLine(rm.GetString("EditJobMenu_Option3"));
            string destination = ReadLineOrEscape();
            if (destination == null) return; 

            try
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("_viewModel.UpdateJob(Int32.Parse(index), source, destination); => put in comment due to adding a parameter");
                //_viewModel.UpdateJob(Int32.Parse(index), source, destination);
                Console.WriteLine(rm.GetString("EditJobMenu_Success"));
                Console.ResetColor();
            }
            catch (ArgumentException e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                if (e.Message.Contains("|"))
                {
                    string[] error = e.Message.Split('|');
                    Console.WriteLine(rm.GetString(error[0]) + error[1]);
                    Console.ResetColor();
                    return;
                }
                else
                {
                    Console.WriteLine(rm.GetString(e.Message));
                    Console.ResetColor();
                    return;
                }
            }
        }


        public void DeleteJob()
        {
            Console.WriteLine();
            Console.WriteLine(rm.GetString("DeleteJobMenu_Title"));
            Console.WriteLine();
            bool empty = ShowAllJobs();
            if (empty)
            {
                return;
            }
            Console.WriteLine(rm.GetString("Return"));
            string index = ReadLineOrEscape();
            if (index == null) return;
            try
            {
                Console.ForegroundColor = ConsoleColor.Green;
                _viewModel.DeleteJob(Int32.Parse(index));
                Console.WriteLine(rm.GetString("DeleteJobMenu_Success"));
                Console.ResetColor();

            }
            catch (ArgumentException e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                if (e.Message.Contains("|"))
                {
                    string[] error = e.Message.Split('|');
                    Console.WriteLine(rm.GetString(error[0]) + error[1]);
                    Console.ResetColor();
                    return;
                }
                else
                {
                    Console.WriteLine(rm.GetString(e.Message));
                    Console.ResetColor();
                    return;
                }
            }
        }

        public void ExecuteJob()
        {
            Console.WriteLine();
            Console.WriteLine(rm.GetString("ExecuteJobMenu_Title"));
            Console.WriteLine();
            bool empty = ShowAllJobs();
            if (empty)
            {
                return;
            }
            Console.WriteLine(rm.GetString("Return"));
            Console.WriteLine();
            Console.WriteLine(rm.GetString("ExecuteJobMenu_Prompt"));
            string selection = ReadLineOrEscape();
            if (selection == null) return;
            try
            {
                _viewModel.RunJobs(selection);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(rm.GetString("ExecuteJobMenu_Success"));
                Console.ResetColor();
            }
            catch (ArgumentException e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                if (e.Message.Contains("|"))
                {
                    string[] error = e.Message.Split('|');
                    Console.WriteLine(rm.GetString(error[0]) + error[1]);
                }
                else
                {
                    Console.WriteLine(rm.GetString(e.Message));
                }
                Console.ResetColor();
            }
        }

        public void SetLanguage(string language)
        {
            _viewModel.SetLanguage(language);
        }

        private bool ShowAllJobs()
        { 
            List<String> jobs = _viewModel.AllInfo();
            Console.WriteLine();
            Console.WriteLine(rm.GetString("AllJobs_List"));
            Console.WriteLine();
            if (jobs.Count == 0)
            {
                Console.WriteLine(rm.GetString("AllJobs_Empty"));
                return true;
            }
            int i = 0;
            foreach (string job in jobs)
            {
                i++;
                Console.WriteLine($"{i}. {job}");
            }
            return false;
        }


        private void PromptForOption()
        {
            Console.WriteLine(rm.GetString("PromptForOption"));
            var optionInput = Console.ReadLine();
            if (int.TryParse(optionInput, out int option))
            {
                SelectOption(option);
            }
            else
            {
                Console.WriteLine(rm.GetString("InvalidOption"));
                WaitKey();
                DisplayMenu(); 
            }
        }
    }
}
