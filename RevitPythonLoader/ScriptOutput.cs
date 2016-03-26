using System;
using System.Windows.Forms;

namespace RevitPythonLoader
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
