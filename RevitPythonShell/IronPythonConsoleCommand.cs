using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using System.Diagnostics;
using Microsoft.Scripting;
using System.Threading;
using System.Windows.Threading;
using RevitPythonShell.RpsRuntime;

namespace RevitPythonShell
{
    /// <summary>
    /// Start an interactive shell in a modal window.
    /// </summary>
    [Regeneration(RegenerationOption.Manual)]
    [Transaction(TransactionMode.Manual)]
    public class IronPythonConsoleCommand : IExternalCommand
    {
        /// <summary>
        /// Open a window to let the user enter python code.
        /// </summary>
        /// <returns></returns>
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {            
            var messageCopy = message;
            var gui = new IronPythonConsole();
            gui.consoleControl.WithConsoleHost((host) =>
            {
                // now that the console is created and initialized, the script scope should
                // be accessible...
                new ScriptExecutor(RevitPythonShellApplication.GetConfig(), commandData, messageCopy, elements)
                    .SetupEnvironment(host.Engine, host.Console.ScriptScope);

                host.Console.ScriptScope.SetVariable("__window__", gui);

                // run the initscript
                var initScript = RevitPythonShellApplication.GetInitScript();
                if (initScript != null)
                {
                    var scriptSource = host.Engine.CreateScriptSourceFromString(initScript, SourceCodeKind.Statements);
                    scriptSource.Execute(host.Console.ScriptScope);
                }                
            });

            var dispatcher = Dispatcher.FromThread(Thread.CurrentThread);
            gui.consoleControl.WithConsoleHost((host) =>
            {                
                host.Console.SetCommandDispatcher((command) =>
                {
                    if (command != null)
                    {
                        // Slightly involved form to enable keyboard interrupt to work.
                        var executing = true;
                        var operation = dispatcher.BeginInvoke(DispatcherPriority.Normal, command);
                        while (executing)
                        {
                            if (operation.Status != DispatcherOperationStatus.Completed)
                                operation.Wait(TimeSpan.FromSeconds(1));
                            if (operation.Status == DispatcherOperationStatus.Completed)
                                executing = false;
                        }
                    }                 
                });
            });
            gui.ShowDialog();
            return Result.Succeeded;
        }
    }    
}
