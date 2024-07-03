using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Vault.Model.Uilities
{
	public class BandWidthManager
	{
		public static Mutex BandWidthMutex = new Mutex();
		public long BandWidthLimit;
        public static BandWidthManager instance;

		public static BandWidthManager GetInstance()
        { 
			if(instance == null)
            { 
				instance = new BandWidthManager();
			}
			return instance;
        }

		public void GetMutex()
		{
			BandWidthMutex.WaitOne();
		}

		public void ReleaseMutex()
		{
			BandWidthMutex.ReleaseMutex();
		}

		public long GetLimit() 
		{ 
			return BandWidthLimit;
		}

		public void SetLimit(long bd)
		{
			BandWidthLimit = bd; 
		}
	}
}
