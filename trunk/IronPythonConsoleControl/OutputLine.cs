using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace IronPythonConsoleControl
{
    /// <summary>
    /// A line of output. Caches the rendering of this line for a given 
    /// console width.
    /// </summary>
    public class OutputLine
    {
        private List<OutputSpan> _spans; // a list of writes (with possibly different

        public OutputLine()
        {
            _spans = new List<OutputSpan>();
        }

        public void Append(OutputSpan span)
        {
            var lastSpan = _spans.LastOrDefault();
            if (lastSpan == null)
            {
                _spans.Add(span);
            }
            else if (lastSpan.Font == span.Font)
            {
                // join two spans with same style
                var combinedSpan = new OutputSpan(_spans.Last().Text + span.Text, span.Font);
                _spans[_spans.Count - 1] = combinedSpan;
            }
            else
            {
                _spans.Add(span);
            }            
        }

        /// <summary>
        /// Given a width, returns the height of this line, including wrapped lines.
        /// </summary>
        public int GetHeight(Graphics g, int Width)
        {
            if (_spans.Count == 0)
            {
                return 0;
            }
            return _spans.First().GetSize(g).Height;
        }

        /// <summary>
        /// Draws the content of the line to the graphics at the
        /// position (0, yPosition)
        /// </summary>
        public void Print(Graphics graphics, int yPosition)
        {
            var xPosition = 0;
            foreach (var span in _spans)
            {
                span.Print(graphics, xPosition, yPosition);
                xPosition += span.GetSize(graphics).Width;
            }
        }
    }
}
