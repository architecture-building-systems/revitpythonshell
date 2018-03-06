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
        private TextBox _lblDocumentation;
        private CompletionToolTipWindow _dialog;
        private bool _cancel = false;
        IEnumerable<string> _documentations;

        private class CompletionToolTipWindow: Form
        {
            private ListBox _completions;
            private TextBox _documentation;

            public CompletionToolTipWindow(ListBox completions,TextBox documentation)
            {
                _completions = completions;
                _documentation = documentation;

                FormBorderStyle = FormBorderStyle.None;
                StartPosition = FormStartPosition.Manual;
                TopMost = true;
                ShowInTaskbar = false;
                BackColor = Color.White;
                Opacity = 0.9;
                
                Controls.Add(completions);
                Controls.Add(documentation);

                Width = completions.PreferredSize.Width;
                Height = completions.Height + documentation.Height;
                completions.Width = Width;
                documentation.Width = Width;
                documentation.Location = new Point(completions.Location.X,completions.Location.Y + completions.Height);
                
                completions.Show();
                documentation.Show();
            }
            public void resize()
            {
                Width = _completions.PreferredSize.Width;
                Height = _completions.Height + _documentation.Height;
                _documentation.Location = new Point(_completions.Location.X, _completions.Location.Y + _completions.Height);
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
        public string ShowTooltip(string uncompleted, IEnumerable<string> completions, IEnumerable<string> documentations,Point location)
        {
            _lstCompletions = new ListBox();            
            _lstCompletions.ScrollAlwaysVisible = true;
            _lstCompletions.Items.AddRange(completions.ToArray());
            _lstCompletions.SelectionMode = SelectionMode.One;
            _lstCompletions.AutoSize = false;
            _lstCompletions.SelectedIndexChanged += new EventHandler(selectedIndexChanged);
            _lstCompletions.Click += new EventHandler(lstCompletionsClicked);

            int maxWidth = 0;
            for (int i = 0; i < _lstCompletions.Items.Count; i++)
            {
                if (_lstCompletions.GetItemRectangle(i).Width > maxWidth)
                    maxWidth = _lstCompletions.GetItemRectangle(i).Width;
            }
            _lstCompletions.Width = maxWidth;
            if (_lstCompletions.Items.Count > 0)
                _lstCompletions.Height = _lstCompletions.GetItemHeight(0) * 10;

            _documentations = documentations;
            _lblDocumentation = new TextBox();
            _lblDocumentation.WordWrap = true;
            _lblDocumentation.Width = _lstCompletions.Width;
            _lblDocumentation.BackColor = SystemColors.ControlLight;
            if (_documentations!=null && _documentations.Count() > 0)
                _lblDocumentation.Text = _documentations.ElementAt(0);
            _lblDocumentation.ScrollBars = ScrollBars.Vertical;
            _lblDocumentation.Multiline = true;
            _lblDocumentation.AutoSize = true;
            _lblDocumentation.Height = 100;
            _lblDocumentation.ReadOnly = true;

            _dialog = new CompletionToolTipWindow(_lstCompletions,_lblDocumentation);
            _dialog.KeyDown += new KeyEventHandler(dialog_KeyDown);
            _dialog.Location = location;
            _dialog.KeyPreview = true;
            _dialog.ShowDialog();
            

            if (_cancel || _lstCompletions.SelectedIndex < 0)
                return null;

            return (string)_lstCompletions.SelectedItem;
        }
        void selectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                _lblDocumentation.Text = _documentations.ElementAt(_lstCompletions.SelectedIndex);
            }
            catch
            {
                _lblDocumentation.Text = "";
            }
            _dialog.resize();
        }

        void dialog_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab || e.KeyCode == Keys.Return)
            {
                _dialog.Hide();
            }
            else if (e.KeyCode == Keys.Escape)
            {
                _cancel = true;
                _dialog.Hide();
            }
        }
        void lstCompletionsClicked(object sender, EventArgs e)
        {
            _dialog.Hide();
        }
    }
}
