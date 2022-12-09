using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMLViewer
{
    public class SelectedItemChangedEventArgs : EventArgs
    {
        /// <summary>
        /// A string that keeps the selected node's path
        /// </summary>
        private readonly string _selectedNodePath;
        
        /// <summary>
        /// Constructor with one argument
        /// </summary>
        /// <param name="selectedNodePath"></param>
        public SelectedItemChangedEventArgs(string selectedNodePath) { 
            _selectedNodePath = selectedNodePath;
        }

        /// <summary>
        /// Returns the value of selected node's path
        /// </summary>
        public string SelectedNodePath { 
            get { return _selectedNodePath; }
        }
    }
}
