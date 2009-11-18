using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Microsoft.Scripting;
using Application=Autodesk.Revit.Application;

namespace RevitPythonShell
{    
    public partial class ScriptInput : Form
    {
        

        private static Application _application;
        private static ScriptInput _instance;
        

        public ScriptInput(Application application)
        {
            _application = application;
            _instance = this;

            InitializeComponent();
            SetTabWidth(txtSource, 1);
        }

        /// <summary>
        /// Execute the piece of code.
        /// </summary>
        private void cmdExecute_Click(object sender, System.EventArgs e)
        {
            ExecuteScript();
        }

        private static void ExecuteScript()
        {
            try
            {                
                var engine = IronPython.Hosting.Python.CreateEngine();
                var searchPaths = engine.GetSearchPaths();
                searchPaths.Add(@"C:\Python25\Lib");
                searchPaths.Add(@"C:\RevitPythonShell");
                engine.SetSearchPaths(searchPaths);
                var scope = engine.CreateScope();
                scope.SetVariable("revit", _application);
                engine.Runtime.IO.SetOutput(new ScriptOutputStream(_instance), Encoding.UTF8);
                engine.Runtime.IO.SetErrorOutput(new ScriptOutputStream(_instance), Encoding.UTF8);    
                var script = engine.CreateScriptSourceFromString(_instance.txtSource.Text, SourceCodeKind.Statements);
                script.Execute(scope);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }            
        }

        /// <summary>
        /// Use CTRL+ENTER to execute the current script.
        /// </summary>
        private void ScriptInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Enter)
            {
                ExecuteScript();
                e.SuppressKeyPress = true;
            }
        }

        // set tab stops to a width of 4
        private const int EM_SETTABSTOPS = 0x00CB;

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr h, int msg, int wParam, int[] lParam);

        public static void SetTabWidth(TextBox textbox, int tabWidth)
        {
            Graphics graphics = textbox.CreateGraphics();
            var characterWidth = (int)graphics.MeasureString("M", textbox.Font).Width;

            SendMessage(textbox.Handle, EM_SETTABSTOPS, 1, new int[] { tabWidth * characterWidth });
        }

        /// <summary>
        /// Clear the output text
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdClear_Click(object sender, EventArgs e)
        {
            txtStdOut.Text = "";
        }
    }

    /// <summary>
    /// A stream to write output to...
    /// </summary>
    class ScriptOutputStream: Stream
    {
        private readonly ScriptInput _gui;

        public ScriptOutputStream(ScriptInput gui)
        {
            _gui = gui;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            var actualBuffer = new byte[count]; 
            Array.Copy(buffer, offset, actualBuffer, 0, count);
            Encoding encoding = Encoding.UTF8;            
            _gui.txtStdOut.AppendText(Encoding.UTF8.GetString(actualBuffer));
            _gui.txtStdOut.SelectionStart = _gui.txtStdOut.Text.Length;
            _gui.txtStdOut.ScrollToCaret();
        }

        public override void Flush()
        {
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

       
        public override bool CanRead
        {
            get { return false; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override long Length
        {
            get { return _gui.txtStdOut.Text.Length; }
        }

        public override long Position
        {
            get { return 0; }
            set { }
        }
    }
}
