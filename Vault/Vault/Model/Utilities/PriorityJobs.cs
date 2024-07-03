using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vault.ViewModel;

namespace Vault.Model.Uilities
{
    public class PriorityJobs
    {
        public static PriorityJobs _instance;
        public Mutex _mutex = new Mutex();

        public static PriorityJobs GetInstance()
        {
            if (_instance == null)
            {
                _instance = new PriorityJobs();
                return _instance;
            }
            else
            {
                return _instance;
            }
        }
    }
 
}
