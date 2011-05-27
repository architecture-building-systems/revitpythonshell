using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Scripting.Hosting.Shell;

namespace IronPythonConsole
{
    public partial class IronPythonConsole : Form
    {
        public IronPythonConsole()
        {
            InitializeComponent();
        }

        private void IronPythonConsole_Load(object sender, EventArgs e)
        {
            // place console controle to fill out form
            console.Width = ClientSize.Width;
            console.Height = ClientSize.Height;
            console.Left = 0;
            console.Top = 0;

            // first step, just print some stuff
            console.WriteLine("Hello, world", Style.Out);
            console.WriteLine("Here is another line of text", Style.Error);

            string line = console.ReadLine(0);
            while (line != null)
            {
                console.WriteLine(line, Style.Warning);
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            console.Width = ClientSize.Width;
            console.Height = ClientSize.Height;
            console.Invalidate();
        }
    }
}
