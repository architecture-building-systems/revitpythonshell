﻿using System;
using System.Linq;
using System.Text;
using Autodesk.Revit;
using IronPython.Runtime.Exceptions;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using System.Collections.Generic;

namespace RevitPythonShell.RpsRuntime
{
    /// <summary>
    /// Executes a script scripts
    /// </summary>
    public class ScriptExecutor
    {
        private readonly ExternalCommandData _commandData;
        private string _message;
        private readonly ElementSet _elements;
        private readonly UIApplication _revit;
        private readonly IRpsConfig _config;
        private readonly UIControlledApplication _uiControlledApplication;

        public ScriptExecutor(IRpsConfig config, UIApplication uiApplication, UIControlledApplication uiControlledApplication)
        {
            _config = config;

            _revit = uiApplication;
            _uiControlledApplication = uiControlledApplication;

            // note, if this constructor is used, then this stuff is all null
            // (I'm just setting it here to be explete - this constructor is
            // only used for the startupscript)
            _commandData = null;
            _elements = null;
            _message = null;
        }

        public ScriptExecutor(IRpsConfig config, ExternalCommandData commandData, string message, ElementSet elements)
        {
            _config = config;

            _revit = commandData.Application;
            _commandData = commandData;
            _elements = elements;
            _message = message;

            _uiControlledApplication = null;
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
                var engine = CreateEngine();
                var scope = SetupEnvironment(engine);

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

        private ScriptEngine CreateEngine()
        {
            var engine = IronPython.Hosting.Python.CreateEngine(new Dictionary<string, object>() { { "Frames", true }, { "FullFrames", true } });                        
            return engine;
        }

        private void AddEmbeddedLib(ScriptEngine engine)
        {
            // use embedded python lib
            var asm = this.GetType().Assembly;
            var resQuery = from name in asm.GetManifestResourceNames()
                           where name.ToLowerInvariant().EndsWith("python_27_lib.zip")
                           select name;
            var resName = resQuery.Single();
            var importer = new IronPython.Modules.ResourceMetaPathImporter(asm, resName);
            dynamic sys = IronPython.Hosting.Python.GetSysModule(engine);
            sys.meta_path.append(importer);            
        }

        /// <summary>
        /// Set up an IronPython environment - for interactive shell or for canned scripts
        /// </summary>
        public ScriptScope SetupEnvironment(ScriptEngine engine)
        {
            var scope = IronPython.Hosting.Python.CreateModule(engine, "__main__");

            SetupEnvironment(engine, scope);

            return scope;
        }

        public void SetupEnvironment(ScriptEngine engine, ScriptScope scope)
        {
            // these variables refer to the signature of the IExternalCommand.Execute method
            scope.SetVariable("__commandData__", _commandData);
            scope.SetVariable("__message__", _message);
            scope.SetVariable("__elements__", _elements);
            scope.SetVariable("__result__", (int)Result.Succeeded);

            // add two special variables: __revit__ and __vars__ to be globally visible everywhere:            
            var builtin = IronPython.Hosting.Python.GetBuiltinModule(engine);
            builtin.SetVariable("__revit__", _revit);
            builtin.SetVariable("__vars__", _config.GetVariables());

            // allow access to the UIControlledApplication in the startup script...
            if (_uiControlledApplication != null)
            {
                builtin.SetVariable("__uiControlledApplication__", _uiControlledApplication);
            }
            
            // add the search paths
            AddSearchPaths(engine);
            AddEmbeddedLib(engine);

            // reference RevitAPI and RevitAPIUI
            engine.Runtime.LoadAssembly(typeof(Autodesk.Revit.DB.Document).Assembly);
            engine.Runtime.LoadAssembly(typeof(Autodesk.Revit.UI.TaskDialog).Assembly);

            // also, allow access to the RPS internals
            engine.Runtime.LoadAssembly(typeof(RevitPythonShell.RpsRuntime.ScriptExecutor).Assembly);
        }        

        /// <summary>
        /// Be nasty and reach into the ScriptScope to get at its private '_scope' member,
        /// since the accessor 'ScriptScope.Scope' was defined 'internal'.
        /// </summary>
        private Microsoft.Scripting.Runtime.Scope GetScope(ScriptScope scriptScope)
        {
            var field = scriptScope.GetType().GetField(
                "_scope",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            return (Microsoft.Scripting.Runtime.Scope)field.GetValue(scriptScope);
        }

        /// <summary>
        /// Add the search paths defined in the ini file to the engine.
        /// The data folder (%APPDATA%/RevitPythonShell20XX) is also added
        /// </summary>
        private void AddSearchPaths(ScriptEngine engine)
        {
            var searchPaths = engine.GetSearchPaths();
            foreach (var path in _config.GetSearchPaths())
            {
                searchPaths.Add(path);
            }
            engine.SetSearchPaths(searchPaths);
        }
    }
}