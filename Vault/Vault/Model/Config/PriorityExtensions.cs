using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vault.Model.Config
{
    public sealed class PriorityExtensions
    {
        public List<string> priorityList;

        static private PriorityExtensions _instance;

        /**
         * Constructor of the Extension
         */
        private PriorityExtensions()
        {
            priorityList = new List<string>();
        }

        /**
         * Add an extension to the list
         */
        public void AddToList(string extension)
        {
            priorityList.Add(extension);
        }

  
        public static PriorityExtensions GetInstance()
        {
            if (_instance == null)
            {
                _instance = new PriorityExtensions();
                return _instance;
            }
            else
            {
                return _instance;
            }
        }
/*
        public bool ExtensionInList(string pathFileTest)
        {
            string extensionFileTest = pathFileTest.Split('.')[1];
            if (priorityList.Contains(extensionFileTest))
            {
                return true;
            }
            return false;
        }
*/
        /**
         * Load the list of the extension
         */
        public void loadListExtension(List<string> loadedList)
        {
            priorityList = loadedList;
        }

        /**
         * Get the list of the extension
         */
        public List<string> GetExtensionList()
        {
            return priorityList;
        }

        public bool IsInList(string extension)
        {
            return priorityList.Contains(extension);
        }
    }
}
