using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace RevitPythonShell
{
    /// <summary>
    /// Display a listbox with a list of completions for a given string.
    /// </summary>
    public class CompletionToolTip
    {
        private ListBox _lstCompletions;
        private CompletionToolTipWindow _dialog;
        private bool _cancel = false;

        private class CompletionToolTipWindow: Form
        {
            private ListBox _completions;

            public CompletionToolTipWindow(ListBox completions)
            {
                _completions = completions;

                FormBorderStyle = FormBorderStyle.None;
                StartPosition = FormStartPosition.Manual;
                TopMost = true;
                ShowInTaskbar = false;
                Opacity = 0.9;
                
                Controls.Add(completions);

                Width = Math.Max(completions.PreferredSize.Width, 150);
                Height = Math.Min(completions.PreferredHeight, 100);
                completions.Height = Height;
                completions.Width = Width;
                
                completions.Show();
            }

            protected override void OnShown(EventArgs e)
            {
                base.OnShown(e);
                _completions.Focus();
            }
        }
       

        /// <summary>
        /// Show a listbox with possible completions for the uncompleted string.
        /// When the user chooses one and presses enter (or clicks it with the mouse),
        /// return the chosen completion. Or, when the user presses escape, then 
        /// close the window and return null.
        /// </summary>        
        public string ShowTooltip(string uncompleted, IEnumerable<string> completions, Point location)
        {
            _lstCompletions = new ListBox();
            _lstCompletions.ScrollAlwaysVisible = true;
            _lstCompletions.Items.AddRange(completions.ToArray());
            _lstCompletions.SelectionMode = SelectionMode.One;

            _dialog = new CompletionToolTipWindow(_lstCompletions);
            _dialog.KeyDown += new KeyEventHandler(dialog_KeyDown);
            _dialog.Location = location;
            _dialog.KeyPreview = true;
            _dialog.ShowDialog();

            if (_cancel || _lstCompletions.SelectedIndex < 0)
            {
                return null;
            }

            return (string)_lstCompletions.SelectedItem;
        }

        void dialog_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
            {
                _dialog.Hide();
            }
            else if (e.KeyCode == Keys.Escape)
            {
                _cancel = true;
                _dialog.Hide();
            }
        }
    }
}
