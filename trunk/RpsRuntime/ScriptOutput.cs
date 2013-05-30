using System;
using System.Windows.Forms;

namespace RevitPythonShell.RpsRuntime
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
