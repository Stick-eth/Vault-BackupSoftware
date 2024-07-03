using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using VaultGUI.Utilities;

namespace VaultGUI.ViewModel
{
    class NavigationVM : Utilities.ViewModelBase
    {
        private object _currentView;

        public object CurrentView
        {
            get { return _currentView; }
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }


        public ICommand HomeCommand { get; set; }
        public ICommand SettingsCommand { get; set; }
        public ICommand BackupsCommand { get; set; }
        public ICommand ConfigurationCommand { get; set; }

        private void Home(object obj) => CurrentView = new HomeVM();
        private void Settings(object obj) => CurrentView = new SettingsVM();
        private void Backups(object obj) => CurrentView = new BackupsVM();
        private void Configuration(object obj) => CurrentView = new ConfigurationVM();

        public NavigationVM()
        {
            HomeCommand = new RelayCommand(Home);
            SettingsCommand = new RelayCommand(Settings);
            BackupsCommand = new RelayCommand(Backups);
            ConfigurationCommand = new RelayCommand(Configuration);

            CurrentView = new HomeVM();
        }
    }
}
