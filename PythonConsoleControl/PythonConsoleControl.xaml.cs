using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.IO;
using System.Xml;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Rendering;


namespace PythonConsoleControl
{
    /// <summary>
    /// Interaction logic for PythonConsoleControl.xaml
    /// </summary>
    public partial class IronPythonConsoleControl : UserControl
    {
        PythonConsolePad _pad;                

        /// <summary>
        /// Perform the action on an already instantiated PythonConsoleHost.
        /// </summary>
        public void WithConsoleHost(Action<PythonConsoleHost> action)
        {
            _pad.Host.WhenConsoleCreated(action);
        }

        public IronPythonConsoleControl()
        {
            InitializeComponent();
            _pad = new PythonConsolePad();
            Grid.Children.Add(_pad.Control);
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
            _pad.Control.SyntaxHighlighting = pythonHighlighting;
            IList<IVisualLineTransformer> transformers = _pad.Control.TextArea.TextView.LineTransformers;
            for (int i = 0; i < transformers.Count; ++i)
            {
                if (transformers[i] is HighlightingColorizer) transformers[i] = new PythonConsoleHighlightingColorizer(pythonHighlighting, _pad.Control.Document);
            }
        }

        public PythonConsolePad Pad
        {
            get { return _pad; }
        }
    }
}
