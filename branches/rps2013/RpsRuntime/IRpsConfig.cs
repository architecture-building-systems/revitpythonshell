using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RpsRuntime
{
    public interface IRpsConfig
    {
        /// <summary>
        /// Returns a list of string variables that the Runtime will add to
        /// the scripts scope under "__vars__".
        /// 
        /// In RevitPythonShell, these are read from the RevitPythonShell.xml file.
        /// </summary>
        IDictionary<string, string> GetVariables();

        /// <summary>
        /// Returns a list of paths to add to the python engine search paths.
        /// 
        /// In RevitPythonShell, these are read from the RevitPythonShell.xml file.
        /// </summary>
        IEnumerable<string> GetSearchPaths();
    }
}
