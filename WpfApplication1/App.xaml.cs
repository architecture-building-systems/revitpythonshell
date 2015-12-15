using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Scripting;
using RevitPythonShell;
using RevitPythonShell.RpsRuntime;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            IronPythonConsole gui = new IronPythonConsole();
            gui.WithHost((host) =>
            {
                // now that the console is created and initialized, the script scope should
                // be accessible...
                //new ScriptExecutor(RevitPythonShellApplication.GetConfig(), commandData, messageCopy, elements)
                //    .SetupEnvironment(host.Engine, host.Console.ScriptScope);

                host.Console.ScriptScope.SetVariable("__window__", gui);

                // run the initscript
                var initScript = RevitPythonShellApplication.GetInitScript();
                if (initScript != null)
                {
                    var scriptSource = host.Engine.CreateScriptSourceFromString(initScript, SourceCodeKind.Statements);
                    scriptSource.Execute(host.Console.ScriptScope);
                }
            }); 
            gui.Show();
        }

        private void WOnConsoleInitialized(object sender, EventArgs eventArgs)
        {
            //PythonConsoleWindow pcw = (PythonConsoleWindow) sender;
            //pcw.PythonScope.SetVariable("Window", pcw);
            //ScriptSource  script = pcw.PythonScope.Engine.CreateScriptSourceFromString("print 2*3", SourceCodeKind.Statements);
            //script.Execute();
        }
    }
}
