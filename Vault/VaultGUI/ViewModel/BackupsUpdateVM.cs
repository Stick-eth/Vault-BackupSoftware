using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaultGUI.Model;

namespace VaultGUI.ViewModel
{
    class BackupsUpdateVM : Utilities.ViewModelBase
    {
        private readonly PageModel _pageModel;

        public BackupsUpdateVM()
        {
            _pageModel = new PageModel();
        }
    }
}
