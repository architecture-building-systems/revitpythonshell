using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Scripting.Hosting.Shell;
using System.IO;

namespace IronPythonConsoleControl
{
    /// <summary>
    /// The IronPythonConsoleControl is a reusable control for implementing the IConsole
    /// interface used by IronPython.
    /// </summary>
    public class IronPythonConsoleControl: Control, IConsole
    {
        private TextWriter _stderr;
        private TextWriter _stdout;
        private List<OutputLine> _lines; // the actual output, before rendering

#region IConsole memebers
        public TextWriter ErrorOutput { get { return _stderr; } set { _stderr = value; } }
        public TextWriter Output { get { return _stdout; } set { _stdout = value; } }

        /// <summary>
        /// Blocks until the user has entered a line in the console.
        /// </summary>
        public string ReadLine(int autoIndentSize)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Write some text to the output. For now, we ignore style, but
        /// we will be revisiting that soon!
        /// </summary>
        public void Write(string text, Style style)
        {
            throw new NotImplementedException();
        }

        public void WriteLine(string text, Style style)
        {
            Write(Text + Environment.NewLine, style);
        }

        public void WriteLine()
        {
            Write(Environment.NewLine, Style.Out);
        }
 
#endregion IConsole members
    }
}
