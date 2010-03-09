using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using Autodesk.Revit;
using IronPython.Runtime.Exceptions;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;

namespace RevitPythonShell
{
    public partial class ShellForm : Form
    {
        private ExternalCommandData _commandData;
        private string _message;
        private ElementSet _elements;

        public ShellForm()
        {
            InitializeComponent();
            LoadCommands();
        }

        /// <summary>
        /// Loads a list of commands from the XML file and
        /// adds them to the tool strip. These allow the user to
        /// save frequently used commands.
        /// </summary>
        private void LoadCommands()
        {
            foreach (var commandNode in GetSettings().Root.Descendants("Command"))
            {
                var commandName = commandNode.Attribute("name").Value;
                var commandSrc = commandNode.Attribute("src").Value;

                var button = toolStrip.Items.Add(commandName);
                button.Tag = commandSrc;
                button.Click += ToolStripItemClick;
            }
        }

        /// <summary>
        /// Run one of the configured scripts.
        /// </summary>
        void ToolStripItemClick(object sender, EventArgs e)
        {
            try
            {
                var commandSrc = (string)(((ToolStripItem)sender).Tag);
                string source;
                using (var reader = File.OpenText(commandSrc))
                {
                    source = reader.ReadToEnd();
                }
                ExecuteScript(source);
            }
            catch (Exception ex)
            {
                // FIXME: we want to show exceptions in the toobar thingy...
                MessageBox.Show(ex.ToString(), ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Run the script and print the output to a new output window.
        /// </summary>
        private void ExecuteScript(string source)
        {
            try
            {
                var engine = IronPython.Hosting.Python.CreateEngine();                
                var scope = engine.CreateScope();
                SetupEnvironment(scope, engine);

                var scriptOutput = new ScriptOutput();
                scriptOutput.Show();
                var outputStream = new ScriptOutputStream(scriptOutput, engine);

                // FIXME: do we really need this?
                scope.SetVariable("__window__", scriptOutput);

                engine.Runtime.IO.SetOutput(outputStream, Encoding.UTF8);
                engine.Runtime.IO.SetErrorOutput(outputStream, Encoding.UTF8);
                engine.Runtime.IO.SetInput(outputStream, Encoding.UTF8);

                var script = engine.CreateScriptSourceFromString(source, SourceCodeKind.Statements);
                try
                {
                    script.Execute(scope);
                }
                catch (SystemExitException exception)
                {
                    // ok, so the system exited. That was bound to happen...
                }
                catch (Exception exception)
                {
                    // show (power) user everything!
                    MessageBox.Show(exception.ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private XDocument GetSettings()
        {
            string assemblyFolder = new FileInfo(GetType().Assembly.Location).DirectoryName;
            string settingsFile = Path.Combine(assemblyFolder, "RevitPythonShell.xml");
            return XDocument.Load(settingsFile);
        }

        /// <summary>
        /// Returns the list of variables to be included with the scope in RevitPythonShell scripts.
        /// </summary>
        /// <returns></returns>
        private IDictionary<string, string> ReadConfigVariables()
        {
            return GetSettings().Root.Descendants("StringVariable").ToDictionary(v => v.Attribute("name").Value,
                                                                                  v => v.Attribute("value").Value);
        }

        /// <summary>
        /// Displays the shell form modally until the user closes it.
        /// Provides the user with access to the parameters passed to the IExternalCommand implementation
        /// in RevitPythonShell so that it can be passed on.
        /// 
        /// For convenience and backwards compatibility, commandData.Application is mapped to the variable "__revit__"
        /// </summary>
        public int ShowShell(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            _elements = elements;
            _message = message;
            _commandData = commandData;

            // provide a hook into Autodesk Revit
            SetupEnvironment(ironTextBoxControl.Scope, ironTextBoxControl.Engine);

            ShowDialog();

            message = (ironTextBoxControl.Scope.GetVariable("__message__") ?? "").ToString();
            return (int) (ironTextBoxControl.Scope.GetVariable("__result__") ?? IExternalCommand.Result.Succeeded);
        }

        /// <summary>
        /// Set up an IronPython environment - for interactive shell or for canned scripts
        /// </summary>
        private void SetupEnvironment(ScriptScope scope, ScriptEngine engine)
        {
            // add variables from Revit
            scope.SetVariable("__revit__", _commandData.Application);
            scope.SetVariable("__commandData__", _commandData);
            scope.SetVariable("__message__", _message);
            scope.SetVariable("__elements__", _elements);
            scope.SetVariable("__result__", (int)IExternalCommand.Result.Succeeded);

            // add preconfigures variables
            scope.SetVariable("__vars__", ReadConfigVariables());

            // add the search paths
            AddSearchPaths(engine);
        }

        /// <summary>
        /// Add the search paths defined in the ini file to the engine.
        /// </summary>
        private void AddSearchPaths(ScriptEngine engine)
        {
            var searchPaths = engine.GetSearchPaths();
            foreach (var searchPathNode in GetSettings().Root.Descendants("SearchPath"))
            {
                searchPaths.Add(searchPathNode.Attribute("name").Value);
            }
            engine.SetSearchPaths(searchPaths);
        }
    }
}
