using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vault.Model.Config
{
    /**
     *\brief This class represent the Extension. The Extension is a class that will be used to store all the extensions that are currently monitored
     * It will be used to store the list of the monitored extensions
     *
     */
    public sealed class Extension
    {
        private List<string> extensionList;

        static private Extension _instance;

        /**
         * Constructor of the Extension
         */
        private Extension()
        {
            extensionList = new List<string>();
        }

        /**
         * Add an extension to the list
         */
        public void AddToList(string extension)
        {
            extensionList.Add(extension);
        }

        /**
         * Remove an extension from the list
         */
        public static Extension GetInstance()
        {
            if (_instance == null)
            {
                _instance = new Extension();
                return _instance;
            }
            else
            {
                return _instance;
            }
        }

        /**
        * Remove an extension from the list
        */
        public bool ExtensionInList(string pathFileTest)
        {
            string extensionFileTest = pathFileTest.Split('.')[1];
            if (extensionList.Contains(extensionFileTest))
            {
                return true;
            }
            return false;
        }

        /**
         * Load the list of the extension
         */
        public void loadListExtension(List<string> loadedList)
        {
            extensionList = loadedList;
        }

        /**
         * Get the list of the extension
         */
        public List<string> GetExtensionList()
        {
            return extensionList;
        }
    }
}
