using System.Windows.Forms;

namespace RpsRuntime
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
