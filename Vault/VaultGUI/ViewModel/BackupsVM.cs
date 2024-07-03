using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaultGUI.Model;

namespace VaultGUI.ViewModel
{
    class BackupsVM : Utilities.ViewModelBase
    {
        private readonly PageModel _pageModel;

        public BackupsVM()
        {
            _pageModel = new PageModel();
        }
    }
}
