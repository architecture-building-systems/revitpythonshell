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
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace RevitPythonShell
{
    public partial class ShellForm : System.Windows.Forms.Form
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
        /// 
        /// If an InitScript is defined in RevitPythonShell.xml, then it will be run first.
        /// </summary>
        public int ShowShell(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            _elements = elements;
            _message = message;
            _commandData = commandData;

            // provide a hook into Autodesk Revit
            new ScriptExecutor(_commandData, _message, _elements).SetupEnvironment(ironTextBoxControl.Engine, ironTextBoxControl.Scope);

            var initScript = RevitPythonShellApplication.GetInitScript();
            if (initScript != null)
            {
                var scriptSource = ironTextBoxControl.Engine.CreateScriptSourceFromString(initScript, SourceCodeKind.Statements);
                scriptSource.Execute(ironTextBoxControl.Scope);
            }

            ironTextBoxControl.CompletionRequested += new EventHandler<IronTextBox.CompletionRequestedEventArgs>(ironTextBoxControl_CompletionRequested);

            ShowDialog();

            message = (ironTextBoxControl.Scope.GetVariable("__message__") ?? "").ToString();
            return (int)(ironTextBoxControl.Scope.GetVariable("__result__") ?? Result.Succeeded);
        }

        [DllImport("user32.dll")]
        static extern bool GetCaretPos(out System.Drawing.Point lpPoint);
        /// <summary>
        /// Show a tooltip with the completions for the current text in the IronTextBoxControl.
        /// </summary>        
        void ironTextBoxControl_CompletionRequested(object sender, IronTextBox.CompletionRequestedEventArgs e)
        {
            string textAtPrompt = e.Uncompleted;
            var completions = PerformCompletion(textAtPrompt);
            if (completions == null)
            {
                return;
            }

            System.Drawing.Point location;
            if (!GetCaretPos(out location))
            {
                location = new System.Drawing.Point(0, 0);
            }
            location = ironTextBoxControl.Parent.PointToScreen(location);
            location.Offset(0, toolStrip.Height + ironTextBoxControl.FontHeight);

            var tooltip = new CompletionToolTip();
            var completed = tooltip.ShowTooltip(e.Uncompleted, completions, location);
            if (completed != null)
            {
                e.Completed = ReplaceUncompleted(textAtPrompt, GetUncompleted(textAtPrompt), completed);
            }
        }

        /// <summary>
        /// complete the current word, 
        /// but only if a __completer__ function is defined in the InitScript
        /// If no __completer__ function is defined, it return null.
        /// </summary>
        private List<string> PerformCompletion(string uncompleted)
        {            
            var engine = ironTextBoxControl.Engine;
            var scope = ironTextBoxControl.Scope;
            object completer;

            if (!scope.TryGetVariable("__completer__", out completer))
            {
                return null;
            }

            var ops = engine.CreateOperations(scope);
            if (!ops.IsCallable(completer))
            {
                return null;
            }

            var completion = (IList<object>)ops.Call(completer, uncompleted);
            if (completion == null)
            {
                return null;
            }
            var result  = completion.Cast<string>().Distinct().ToList();
            result.Sort();
            return result;
        }

        /// <summary>
        /// returns the uncompleted portion of the current text at the prompt.
        /// </summary>
        private string GetUncompleted(string textAtPrompt)
        {            
            var match = _regexLastWord.Match(textAtPrompt);
            return match.Groups[1].Value;
        }        
        private static Regex _regexLastWord = new Regex(@"^.*?\b((\w|\.)*)$");

        /// <summary>
        /// Replaces the uncompleted portion of textAtPrompt with the completed version.
        /// </summary>
        private string ReplaceUncompleted(string textAtPrompt, string uncompleted, string completed)
        {
            var textBefore = textAtPrompt.Substring(0, textAtPrompt.LastIndexOf(uncompleted));
            return textBefore + completed;
        }

        /// <summary>
        /// Show the dialog for configuring scripts.
        /// </summary>
        private void btnConfigureScripts_Click(object sender, EventArgs e)
        {
            var dialog = new ConfigureCommandsForm();
            dialog.ShowDialog(this);

            // reset toolbar (remove script buttons, then re-add them)
            while (toolStrip.Items.Count > 2) // leave "Configure Commands" and the separator
            {
                toolStrip.Items.RemoveAt(2);
            }
            LoadCommands();
        }
    }
}
