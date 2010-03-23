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
            foreach (var command in RevitPythonShellApplication.GetCommands())
            {                
                var button = toolStrip.Items.Add(command.Name);
                button.Tag = command.Source;
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
                new ScriptExecutor(_commandData, _message, _elements).ExecuteScript(source);
            }
            catch (Exception ex)
            {
                // FIXME: we want to show exceptions in the toobar thingy...
                MessageBox.Show(ex.ToString(), ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
            new ScriptExecutor(_commandData, _message, _elements).SetupEnvironment(ironTextBoxControl.Engine, ironTextBoxControl.Scope);

            ShowDialog();

            message = (ironTextBoxControl.Scope.GetVariable("__message__") ?? "").ToString();
            return (int) (ironTextBoxControl.Scope.GetVariable("__result__") ?? IExternalCommand.Result.Succeeded);
        }                
    }
}
