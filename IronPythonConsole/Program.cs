using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace IronPythonConsole
{
    static class Program
    {
        /// <summary>
        /// Show a form with the IronPythonConsoleControl hooked up to 
        /// an IronPythonEngine.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new IronPythonConsole());
        }
    }
}
