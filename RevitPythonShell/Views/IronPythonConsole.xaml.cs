using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls.Ribbon;
using System.Windows.Input;
using System.Xml;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using Microsoft.Win32;

namespace RevitPythonShell.Views
{
    /// <summary>
    /// Interaction logic for IronPythonConsole.xaml
    /// </summary>
    public partial class IronPythonConsole : Window
    {
        private ConsoleOptions consoleOptionsProvider;

        // this is the name of the file currently being edited in the pad
        private string currentFileName;

        public IronPythonConsole()
        {
            Initialized += new EventHandler(MainWindow_Initialized);

            IHighlightingDefinition pythonHighlighting;
            using (Stream s = typeof(IronPythonConsole).Assembly.GetManifestResourceStream("RevitPythonShell.Resources.Python.xshd"))
            {
                if (s == null)
                    throw new InvalidOperationException("Could not find embedded resource");
                using (XmlReader reader = new XmlTextReader(s))
                {
                    pythonHighlighting = ICSharpCode.AvalonEdit.Highlighting.Xshd.
                        HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }
            // and register it in the HighlightingManager
            HighlightingManager.Instance.RegisterHighlighting("Python Highlighting", new string[] { ".cool" }, pythonHighlighting);

            InitializeComponent();

            textEditor.SyntaxHighlighting = pythonHighlighting;
            textEditor.PreviewKeyDown += new KeyEventHandler(textEditor_PreviewKeyDown);
            consoleOptionsProvider = new ConsoleOptions(consoleControl.Pad);

            // get application version and show in title
            Title = String.Format("RevitPythonShell | {0}", Assembly.GetExecutingAssembly().GetName().Version.ToString());
        }

        private void MainWindow_Initialized(object sender, EventArgs e)
        {
            //propertyGridComboBox.SelectedIndex = 1;
            textEditor.ShowLineNumbers = true;
        }
        private void newFileClick(object sender, RoutedEventArgs e)
        {
            currentFileName = null;
            textEditor.Text = string.Empty;
        }
        private void openFileClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.CheckFileExists = true;
            if (dlg.ShowDialog() ?? false)
            {
                currentFileName = dlg.FileName;
                textEditor.Load(currentFileName);
                //textEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinitionByExtension(Path.GetExtension(currentFileName));
            }
        }
        private void saveAsFileClick(object sender, EventArgs e)
        {
            currentFileName = null;
            SaveFile();
        }
        private void saveFileClick(object sender, EventArgs e)
        {
           SaveFile();
        }
        private void SaveFile()
        {
            if (currentFileName == null)
            {
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.Filter = "Save Files (*.py)|*.py";
                dlg.DefaultExt = "py";
                dlg.AddExtension = true;
                if (dlg.ShowDialog() ?? false)
                {
                    currentFileName = dlg.FileName;
                }
                else
                {
                    return;
                }
            }
            textEditor.Save(currentFileName);
        }

        private void runClick(object sender, EventArgs e)
        {
            RunStatements();
        }

        private void textEditor_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F5) RunStatements();
        }

        private void RunStatements()
        {
            string statementsToRun = "";
            if (textEditor.TextArea.Selection.Length > 0)
                statementsToRun = textEditor.TextArea.Selection.GetText();
            else
                statementsToRun = textEditor.TextArea.Document.Text;
            consoleControl.Pad.Console.RunStatements(statementsToRun);
        }

        // Clear the contents on first click inside the editor
        private void textEditor_GotFocus(object sender, RoutedEventArgs e)
        {
            if (this.currentFileName == null)
            {
                TextEditor tb = (TextEditor)sender;
                tb.Text = string.Empty;
                // Remove the handler from the list otherwise this handler will clear
                // editor contents every time the editor gains focus.
                tb.GotFocus -= textEditor_GotFocus;
            }
        }

    }
}