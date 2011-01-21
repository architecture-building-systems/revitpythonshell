using System;
using System.Text;
using Autodesk.Revit;
using IronPython.Runtime.Exceptions;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;

namespace RevitPythonShell
{
    /// <summary>
    /// Executes a script scripts
    /// </summary>
    public class ScriptExecutor
    {
        private readonly ExternalCommandData _commandData;
        private string _message;
        private readonly ElementSet _elements;

        public ScriptExecutor(ExternalCommandData commandData, string message, ElementSet elements)
        {
            _commandData = commandData;
            _elements = elements;
            _message = message;
        }

        public string Message
        {
            get
            {
                return _message;
            }
        }

        /// <summary>
        /// Run the script and print the output to a new output window.
        /// </summary>
        public int ExecuteScript(string source)
        {
            try
            {
                var engine = IronPython.Hosting.Python.CreateEngine();
                var scope = engine.CreateScope();
                SetupEnvironment(engine, scope);

                var scriptOutput = new ScriptOutput();
                scriptOutput.Show();
                var outputStream = new ScriptOutputStream(scriptOutput, engine);
                
                scope.SetVariable("__window__", scriptOutput);

                engine.Runtime.IO.SetOutput(outputStream, Encoding.UTF8);
                engine.Runtime.IO.SetErrorOutput(outputStream, Encoding.UTF8);
                engine.Runtime.IO.SetInput(outputStream, Encoding.UTF8);

                var script = engine.CreateScriptSourceFromString(source, SourceCodeKind.Statements);
                try
                {
                    script.Execute(scope);

                    _message = (scope.GetVariable("__message__") ?? "").ToString();
                    return (int)(scope.GetVariable("__result__") ?? Result.Succeeded);
                }
                catch (SystemExitException)
                {
                    // ok, so the system exited. That was bound to happen...
                    return (int)Result.Succeeded;
                }
                catch (Exception exception)
                {
                    // show (power) user everything!
                    _message = exception.ToString();
                    return (int)Result.Failed;
                }

            }
            catch (Exception ex)
            {
                _message = ex.ToString();
                return (int)Result.Failed;
            }
        }

        /// <summary>
        /// Set up an IronPython environment - for interactive shell or for canned scripts
        /// </summary>
        public void SetupEnvironment(ScriptEngine engine, ScriptScope scope)
        {                        
            // add variables from Revit
            scope.SetVariable("__revit__", _commandData.Application);
            scope.SetVariable("__commandData__", _commandData);
            scope.SetVariable("__message__", _message);
            scope.SetVariable("__elements__", _elements);
            scope.SetVariable("__result__", (int)Result.Succeeded);

            // add preconfigures variables
            scope.SetVariable("__vars__", RevitPythonShellApplication.GetVariables());

            // add the search paths
            AddSearchPaths(engine);
        }

        /// <summary>
        /// Add the search paths defined in the ini file to the engine.
        /// </summary>
        private static void AddSearchPaths(ScriptEngine engine)
        {
            var searchPaths = engine.GetSearchPaths();
            foreach (var path in RevitPythonShellApplication.GetSearchPaths())
            {
                searchPaths.Add(path);
            }
            engine.SetSearchPaths(searchPaths);
        }
    }
}
