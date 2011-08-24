using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Scripting.Hosting.Shell;
using System.Drawing;

namespace IronPythonConsoleControl
{
    /// <summary>
    /// Maintains the content and visual representation of a piece of chunk
    /// of text. For wraping, this text must be splittable, by character count.
    /// </summary>
    public class OutputSpan
    {
        private Font _font;
        private string _text;

        public Font Font { get { return _font; } }
        public string Text { get { return _text; } }

        public OutputSpan(string text, Font font)
        {
            _font = font;
            _text = text;
        }

        public Size GetSize(Graphics g)
        {
            var sizef = g.MeasureString(_text, _font, int.MaxValue, StringFormat.GenericTypographic);
            return new Size((int)sizef.Width, (int)sizef.Height);
        }

        /// <summary>
        /// Prints a span to the graphics, at the position
        /// (xPosition, yPosition)
        /// </summary>
        public void Print(Graphics graphics, int xPosition, int yPosition)
        {
            graphics.DrawString(_text, _font, Brushes.Black, xPosition, yPosition);
        }
    }
}
