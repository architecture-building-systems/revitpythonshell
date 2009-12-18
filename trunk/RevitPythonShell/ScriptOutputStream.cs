using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace RevitPythonShell
{
    /// <summary>
    /// A stream to write output to...
    /// This can be passed into the python interpreter to render all output to.
    /// Only a minimal subset is actually implemented - this is all we really
    /// expect to use.
    /// </summary>
    public class ScriptOutputStream: Stream
    {
        private readonly ScriptOutput _gui;
        private int _bomCharsLeft; // we want to get rid of pesky UTF8-BOM-Chars on write
        private readonly Queue<MemoryStream> _completedLines; // one memorystream per line of input
        private MemoryStream _inputBuffer;

        public ScriptOutputStream(ScriptOutput gui)
        {
            _gui = gui;
            _gui.txtStdOut.KeyPress += KeyPressEventHandler;
            _gui.txtStdOut.KeyDown += KeyDownEventHandler;
            _gui.txtStdOut.Focus();

            _completedLines = new Queue<MemoryStream>();
            _inputBuffer = new MemoryStream();

            _bomCharsLeft = 3; //0xef, 0xbb, 0xbf for UTF-8 (see http://en.wikipedia.org/wiki/Byte_order_mark#Representations_of_byte_order_marks_by_encoding)
        }

        /// <summary>
        /// Complete a line when the enter key is pressed...
        /// </summary>
        void KeyDownEventHandler(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
            {
                var line = _inputBuffer;
                var newLine = new byte[] {/*0x0d,*/ 0x0a};
                line.Write(newLine, 0, newLine.Length); // append new-line
                line.Seek(0, SeekOrigin.Begin); // rewind the line for later reading...
                _completedLines.Enqueue(line);
                _inputBuffer = new MemoryStream();
            }
        }

        /// <summary>
        /// Stash away any printable characters for later...
        /// </summary>
        void KeyPressEventHandler(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar))
            {
                var bytes = Encoding.UTF8.GetBytes(new[] {e.KeyChar});
                _inputBuffer.Write(bytes, 0, bytes.Length);
                _gui.txtStdOut.Focus();
            }
        }



        /// <summary>
        /// Append the text in the buffer to gui.txtStdOut
        /// </summary>
        public override void Write(byte[] buffer, int offset, int count)
        {
            while (_bomCharsLeft > 0 && count > 0)
            {
                _bomCharsLeft--;
                count--;
                offset++;
            }

            var actualBuffer = new byte[count]; 
            Array.Copy(buffer, offset, actualBuffer, 0, count);
            var text = Encoding.UTF8.GetString(actualBuffer);            

            _gui.txtStdOut.AppendText(text);
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

        /// <summary>
        /// Read from the _inputBuffer, block until a new line has been entered...
        /// </summary>
        public override int Read(byte[] buffer, int offset, int count)
        {
            while (_completedLines.Count < 1)
            {
                // wait for user to complete a line
                Application.DoEvents();
            }
            var line = _completedLines.Dequeue();
            return line.Read(buffer, offset, count);
        }

       
        public override bool CanRead
        {
            get { return true; }
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