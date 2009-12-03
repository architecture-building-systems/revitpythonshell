using System;
using System.IO;
using System.Text;

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

        public ScriptOutputStream(ScriptOutput gui)
        {
            _gui = gui;
            _bomCharsLeft = 3; //0xef, 0xbb, 0xbf for UTF-8 (see http://en.wikipedia.org/wiki/Byte_order_mark#Representations_of_byte_order_marks_by_encoding)
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