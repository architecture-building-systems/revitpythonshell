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
using System.IO;
using System.Windows.Threading;
using System.Xml;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Rendering;
using Microsoft.Win32;


namespace PythonConsoleControl
{
    /// <summary>
    /// Interaction logic for PythonConsoleControl.xaml
    /// </summary>
    public partial class IronPythonConsoleControl : UserControl
    {
        PythonConsolePad pad;
        
        public IronPythonConsoleControl()
        {
            InitializeComponent();
            pad = new PythonConsolePad();
            grid.Children.Add(pad.Control);
            // Load our custom highlighting definition
            IHighlightingDefinition pythonHighlighting;
            using (Stream s = typeof(IronPythonConsoleControl).Assembly.GetManifestResourceStream("PythonConsoleControl.Resources.Python.xshd"))
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
            pad.Control.SyntaxHighlighting = pythonHighlighting;
            IList<IVisualLineTransformer> transformers = pad.Control.TextArea.TextView.LineTransformers;
            for (int i = 0; i < transformers.Count; ++i)
            {
                if (transformers[i] is HighlightingColorizer) transformers[i] = new PythonConsoleHighlightingColorizer(pythonHighlighting, pad.Control.Document);
            }

            pad.Control.Loaded += (sender, args) => pad.Control.Focus();

        }

        /// <summary>
        /// Performs the specified action on the console host but only once the console
        /// has initialized.
        /// </summary>
        public void WithHost(Action<PythonConsoleHost> hostAction)
        {
            this.hostAction = hostAction;
            Host.ConsoleCreated += new ConsoleCreatedEventHandler(Host_ConsoleCreated);
        }

        Action<PythonConsoleHost> hostAction;

        void Host_ConsoleCreated(object sender, EventArgs e)
        {
            Console.ConsoleInitialized += new ConsoleInitializedEventHandler(Console_ConsoleInitialized);
        }

        void Console_ConsoleInitialized(object sender, EventArgs e)
        {
            hostAction.Invoke(Host);
        }

        public PythonConsole Console
        {
            get { return pad.Console; }
        }

        public PythonConsoleHost Host
        {
            get { return pad.Host; }
        }

        public PythonConsolePad Pad
        {
            get { return pad; }
        }
    }
}
