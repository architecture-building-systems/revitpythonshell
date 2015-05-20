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
using System.Threading.Tasks;
using IronPython.Runtime;
using Microsoft.Scripting.Hosting;

namespace RevitPythonShell
{
    /// <summary>
    /// An object of this class is instantiated every time the user clicks on the
    /// button for opening the shell.
    /// </summary>
    /// 
    [Regeneration(RegenerationOption.Manual)]
    [Transaction(TransactionMode.Manual)]
    public class NonModalConsoleCommand : IExternalCommand
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
            var commandCompletedEvent = new AutoResetEvent(false);
            var externalEventHandler = new IronPythonExternalEventDispatcher(gui, commandCompletedEvent);
            var externalEvent = ExternalEvent.Create(externalEventHandler);
            gui.consoleControl.WithConsoleHost((host) =>
            {
                var oldDispatcher = host.Console.GetCommandDispatcher();
                host.Console.SetCommandDispatcher((command) =>
                {
                    //externalEventHandler.Enqueue(() => oldDispatcher(command));                    
                    externalEventHandler.Enqueue(command);
                    externalEvent.Raise();
                    commandCompletedEvent.WaitOne();
                });

                host.Editor.SetCompletionDispatcher((command) =>
                {
                    externalEventHandler.Enqueue(command);
                    externalEvent.Raise();
                    commandCompletedEvent.WaitOne();                    
                });
            });
            gui.Topmost = true;
            gui.Title = "RevitPythonShell (non-modal)";
            gui.Show();
            return Result.Succeeded;
        }
    }

    /// <summary>
    /// Make sure commands are executed in a RevitAPI context for non-modal RPS interactive shells.
    /// </summary>
    public class IronPythonExternalEventDispatcher : IExternalEventHandler
    {
        private IronPythonConsole _gui;
        private Queue<Action> _commands = new Queue<Action>();
        private AutoResetEvent _commandCompletedEvent;

        public void Enqueue(Action command)
        {
            _commands.Enqueue(command);
        }

        public IronPythonExternalEventDispatcher(IronPythonConsole gui, AutoResetEvent commandCompletedEvent)
        {
            _gui = gui;
            _commandCompletedEvent = commandCompletedEvent;
        }

        public void Execute(UIApplication app)
        {
            while (_commands.Count > 0)
            {
                var command = _commands.Dequeue();
                try
                {
                    command();
                }
                catch (Exception ex)
                {
                    try
                    {
                        _gui.consoleControl.WithConsoleHost((host) =>
                        {
                            ExceptionOperations eo;
                            eo = host.Engine.GetService<ExceptionOperations>();
                            var error = eo.FormatException(ex);
                            host.Console.WriteLine(error, Microsoft.Scripting.Hosting.Shell.Style.Error);
                            //TaskDialog.Show("Error", error);
                        });
                    }
                    catch (Exception exception)
                    {                       
                        Debugger.Launch();
                        Trace.WriteLine(exception.ToString());
                    }
                }
                finally
                {
                    _commandCompletedEvent.Set();
                }
            }
        }

        public string GetName()
        {
            return "IronPythonExternalEventDispatcher";
        }
    }
}
