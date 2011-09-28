using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Scripting.Hosting.Shell;
using ICSharpCode.AvalonEdit.Highlighting;
using System.IO;
using System.Xml;
using Microsoft.Win32;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Microsoft.Scripting;
using System.Threading;

namespace RevitPythonShell
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

            propertyGridComboBox.SelectedIndex = 0;

            expander.Expanded += new RoutedEventHandler(expander_Expanded);            
        }
        
        void MainWindow_Initialized(object sender, EventArgs e)
        {
            //propertyGridComboBox.SelectedIndex = 1;
        }

        void openFileClick(object sender, RoutedEventArgs e)
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

        void saveFileClick(object sender, EventArgs e)
        {
            if (currentFileName == null)
            {
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.DefaultExt = ".txt";
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

        void runClick(object sender, EventArgs e)
        {
            RunStatements();
        }

        void textEditor_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F5) RunStatements();
        }

        void RunStatements()
        {
            string statementsToRun = "";
            if (textEditor.TextArea.Selection.Length > 0)
                statementsToRun = textEditor.TextArea.Selection.GetText(textEditor.TextArea.Document);
            else
                statementsToRun = textEditor.TextArea.Document.Text;
            consoleControl.Pad.Console.RunStatements(statementsToRun);
        }

        void propertyGridComboBoxSelectionChanged(object sender, RoutedEventArgs e)
        {
            if (propertyGrid == null)
                return;
            switch (propertyGridComboBox.SelectedIndex)
            {
                case 0:
                    propertyGrid.SelectedObject = consoleOptionsProvider; // not .Instance
                    break;
                case 1:
                    //propertyGrid.SelectedObject = textEditor.Options; (for WPF native control)
                    propertyGrid.SelectedObject = textEditor.Options;
                    break;
            }
        }

        void expander_Expanded(object sender, RoutedEventArgs e)
        {
            propertyGridComboBoxSelectionChanged(sender, e);
        }      
    }
}
