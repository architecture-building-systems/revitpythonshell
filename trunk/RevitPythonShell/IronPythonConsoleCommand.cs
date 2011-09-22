using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using System.Diagnostics;
using Microsoft.Scripting;
<<<<<<< .mine
using System.Threading;
=======
using System.Threading;
using System.Windows.Threading;
>>>>>>> .r128

namespace RevitPythonShell
{
    /// <summary>
    /// An object of this class is instantiated every time the user clicks on the
    /// button for opening the shell.
    /// </summary>
    /// 
    [Regeneration(RegenerationOption.Manual)]
    [Transaction(TransactionMode.Manual)]
    public class IronPythonConsoleCommand : IExternalCommand
    {
        private Queue<Action> replCommands;

        /// <summary>
        /// Open a window to let the user enter python code.
        /// </summary>
        /// <returns></returns>
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            replCommands = new Queue<Action>();

            var messageCopy = message;
            var gui = new IronPythonConsole(
                (sender, e) => {
                    var host = (PythonConsoleControl.PythonConsoleHost)sender;
                    host.Console.ConsoleInitialized += (sender2, e2) => {

                        // make sure we're running in the same thread, since otherwise Revit will crash on Transaction.Commit()     
                        host.Console.SetCommandDispatcher((command) => { if (command != null) replCommands.Enqueue(command); });

                        // now that the console is created and initialized, the script scope should
                        // be accessible...
                        new ScriptExecutor(commandData, messageCopy, elements).SetupEnvironment(host.Engine, host.Console.ScriptScope);

                        // run the initscript
                        var initScript = RevitPythonShellApplication.GetInitScript();
                        if (initScript != null)
                        {
                            var scriptSource = host.Engine.CreateScriptSourceFromString(initScript, SourceCodeKind.Statements);
                            scriptSource.Execute(host.Console.ScriptScope);
                        }
                    };
                });
<<<<<<< .mine

            gui.ShowShell(commandData, ref message, elements);

            while (true)
            {
                if (PythonConsoleControl.PythonConsole.ReplCommands.Any())
                {
                    var command = PythonConsoleControl.PythonConsole.ReplCommands.Dequeue();
                    if (command != null)
                    {
                        command();
                    }
                    else
                    {
                        break;
                    }                    
                }
                else
                {
                    System.Windows.Forms.Application.DoEvents();
                    Thread.Sleep(100);
                    Thread.Yield();
                }
            }
            return Result.Succeeded;
=======
            
            gui.ShowShell(commandData, elements, () => replCommands.Enqueue(null));
            while (true)
            {
                if (replCommands.Count > 0)
                {
                    var command = replCommands.Dequeue();
                    if (command != null)
                    {
                        command();
                    }
                    else
                    {
                        // dialog closed
                        break;
                    }
                }
                else
                {
                    Thread.Sleep(10);
                }
            }
            message = gui.Message;
            return gui.ResultValue;
>>>>>>> .r128
        }

        void Console_ConsoleInitialized(object sender, EventArgs e)
        {
            
        }
    }    
}
