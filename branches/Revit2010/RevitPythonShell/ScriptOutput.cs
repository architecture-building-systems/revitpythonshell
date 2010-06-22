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
    }
}
