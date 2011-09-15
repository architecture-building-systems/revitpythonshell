// Copyright (c) 2010 Joe Moorhouse

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.AvalonEdit;
using System.Windows.Media;

namespace PythonConsoleControl
{   
    public class PythonConsolePad 
    {
        PythonTextEditor pythonTextEditor;
        TextEditor textEditor;
        PythonConsoleHost host;

        public PythonConsolePad()
        {
            textEditor = new TextEditor();
            pythonTextEditor = new PythonTextEditor(textEditor);
            host = new PythonConsoleHost(pythonTextEditor);
            host.Run();
            textEditor.FontFamily = new FontFamily("Consolas");
            textEditor.FontSize = 12;

        }

        public TextEditor Control
        {
            get { return textEditor; }
        }

        public PythonConsoleHost Host
        {
            get { return host; }
        }

        public PythonConsole Console
        {
            get { return host.Console; }
        }

        public void Dispose()
        {
            host.Dispose();
        }
    }
}
