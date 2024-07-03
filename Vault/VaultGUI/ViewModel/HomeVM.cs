using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaultGUI.Model;

namespace VaultGUI.ViewModel
{
    class HomeVM : Utilities.ViewModelBase
    {
        private readonly PageModel _pageModel;

        public HomeVM()
        {
            _pageModel = new PageModel();
        }
    }
}
