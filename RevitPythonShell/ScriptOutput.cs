using System;
using System.Windows.Forms;

namespace RevitPythonShell
{
    public partial class ScriptOutput : Form
    {
        public ScriptOutput()
        {
            InitializeComponent();
            txtStdOut.Text = "";            
        }

        /// <summary>
        /// Copies the output text to the clipboard (for pasting into
        /// an editor for further treating...)
        /// </summary>
        private void cmdCopyToClipboard_Click(object sender, EventArgs e)
        {
            Clipboard.Clear();
            Clipboard.SetText(txtStdOut.Text);
        }
    }
}
