using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaultGUI.Model;

namespace VaultGUI.ViewModel
{
    class BackupsCreateVM : Utilities.ViewModelBase
    {
        private readonly PageModel _pageModel;

        public BackupsCreateVM()
        {
            _pageModel = new PageModel();
        }
    }
}
