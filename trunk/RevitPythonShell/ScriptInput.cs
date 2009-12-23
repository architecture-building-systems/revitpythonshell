using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using Application=Autodesk.Revit.Application;

namespace RevitPythonShell
{    
    public partial class ScriptInput : Form
    {
        private readonly Application _application;
        

        public ScriptInput(Application application)
        {
            _application = application;

            InitializeComponent();
            SetTabWidth(txtSource, 1);

            LoadCommands();
            LoadDefaultScript();
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

        private XDocument GetSettings()
        {
            string assemblyFolder = new FileInfo(GetType().Assembly.Location).DirectoryName;
            string settingsFile = Path.Combine(assemblyFolder, "RevitPythonShell.xml");
            return XDocument.Load(settingsFile);
        }

        /// <summary>
        /// Loads a default script from the XML file.
        /// </summary>
        private void LoadDefaultScript()
        {            
            var defaultScripts = GetSettings().Root.Descendants("DefaultScript");
            txtSource.Text = defaultScripts.Count() > 0 ? defaultScripts.First().Value.Replace("\n", "\r\n") : "";
        }

        /// <summary>
        /// Run one of the configured scripts.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

                MessageBox.Show(ex.ToString(), ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Execute the piece of code.
        /// </summary>
        private void cmdExecute_Click(object sender, EventArgs e)
        {
            ExecuteScript(txtSource.Text);
        }

        /// <summary>
        /// Run the script and print the output to a new output window.
        /// </summary>
        private void ExecuteScript(string source)
        {
            try
            {                
                var engine = IronPython.Hosting.Python.CreateEngine();                
                AddSearchPaths(engine);
                var scope = engine.CreateScope();
                scope.SetVariable("__revit__", _application);

                var scriptOutput = new ScriptOutput();
                scriptOutput.Show();
                var outputStream = new ScriptOutputStream(scriptOutput);                

                engine.Runtime.IO.SetOutput(outputStream, Encoding.UTF8);
                engine.Runtime.IO.SetErrorOutput(outputStream, Encoding.UTF8);
                engine.Runtime.IO.SetInput(outputStream, Encoding.UTF8);
                var script = engine.CreateScriptSourceFromString(source, SourceCodeKind.Statements);
                script.Execute(scope);          
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }            
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

        /// <summary>
        /// Use CTRL+ENTER to execute the current script.
        /// </summary>
        private void ScriptInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Enter)
            {
                ExecuteScript(txtSource.Text);
                e.SuppressKeyPress = true;
            }
        }

        // set tab stops to a width of 4
// ReSharper disable InconsistentNaming
        private const int EM_SETTABSTOPS = 0x00CB;
// ReSharper restore InconsistentNaming

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr h, int msg, int wParam, int[] lParam);

        public static void SetTabWidth(TextBox textbox, int tabWidth)
        {
            Graphics graphics = textbox.CreateGraphics();
            var characterWidth = (int)graphics.MeasureString("M", textbox.Font).Width;

            SendMessage(textbox.Handle, EM_SETTABSTOPS, 1, new[] { tabWidth * characterWidth });
        }        
    }
}
