using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaultGUI.Model;

namespace VaultGUI.ViewModel
{
    class SettingsVM : Utilities.ViewModelBase
    {
        private readonly PageModel _pageModel;

        public SettingsVM()
        {
            _pageModel = new PageModel();
        }
    }
}
