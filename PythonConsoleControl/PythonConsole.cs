// Copyright (c) 2010 Joe Moorhouse

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using Microsoft.Scripting.Hosting.Shell;
using System.Windows.Input;
using Microsoft.Scripting;
using IronPython.Hosting;
using IronPython.Runtime;
using Microsoft.Scripting.Hosting;
using Microsoft.Scripting.Hosting.Providers;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Documents;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Utils;
using Style = Microsoft.Scripting.Hosting.Shell.Style;

namespace PythonConsoleControl
{
<<<<<<< .mine
    public delegate void ConsoleInitializedEventHandler(object sender, EventArgs e);    
    
    /// <summary>
    /// Custom IronPython console. The command dispacher runs on a separate UI thread from the REPL
    /// and also from the WPF control.
    /// </summary>
    public class PythonConsole : IConsole, IDisposable
    {
        public static Queue<Action> ReplCommands = new Queue<Action>();
=======
    public delegate void ConsoleInitializedEventHandler(object sender, EventArgs e);
    
    /// <summary>
    /// Custom IronPython console. The command dispacher runs on a separate UI thread from the REPL
    /// and also from the WPF control.
    /// </summary>
    public class PythonConsole : IConsole, IDisposable
    {
        PythonTextEditor textEditor;
        int lineReceivedEventIndex = 0; // The index into the waitHandles array where the lineReceivedEvent is stored.
        ManualResetEvent lineReceivedEvent = new ManualResetEvent(false);
        ManualResetEvent disposedEvent = new ManualResetEvent(false);
        AutoResetEvent statementsExecutionRequestedEvent = new AutoResetEvent(false);
        WaitHandle[] waitHandles;
        int promptLength = 4;
        List<string> previousLines = new List<string>();
        CommandLine commandLine;
        CommandLineHistory commandLineHistory = new CommandLineHistory();
        Thread consoleThread;
        Thread statementsExecutionThread;
        volatile bool statementsExecuting = false;
        string scriptText = "";
        bool allowFullAutocompletion = false;
        bool allowCtrlSpaceAutocompletion = true;
        bool consoleInitialized = false;
        string prompt;
        Window dispatcherWindow;
>>>>>>> .r128

<<<<<<< .mine
        PythonTextEditor textEditor;
        int lineReceivedEventIndex = 0; // The index into the waitHandles array where the lineReceivedEvent is stored.
        ManualResetEvent lineReceivedEvent = new ManualResetEvent(false);
        ManualResetEvent disposedEvent = new ManualResetEvent(false);
        AutoResetEvent statementsExecutionRequestedEvent = new AutoResetEvent(false);
        WaitHandle[] waitHandles;
        int promptLength = 4;
        List<string> previousLines = new List<string>();
        CommandLine commandLine;
        CommandLineHistory commandLineHistory = new CommandLineHistory();
        Thread consoleThread;
        Thread statementsExecutionThread;
        volatile bool statementsExecuting = false;
        string scriptText = "";
        bool allowFullAutocompletion = false;
        bool allowCtrlSpaceAutocompletion = true;
        bool consoleInitialized = false;
        string prompt;
        Window dispatcherWindow;
=======
        public event ConsoleInitializedEventHandler ConsoleInitialized;
>>>>>>> .r128

<<<<<<< .mine
        public event ConsoleInitializedEventHandler ConsoleInitialized;
=======
        public ScriptScope ScriptScope
        {
            get { return commandLine.ScriptScope; }
        }
>>>>>>> .r128

<<<<<<< .mine
        public ScriptScope ScriptScope
        {
            get { return commandLine.ScriptScope; }
        }
=======
        public PythonConsole(PythonTextEditor textEditor, CommandLine commandLine)
        {   
            waitHandles = new WaitHandle[] { lineReceivedEvent, disposedEvent };
>>>>>>> .r128

<<<<<<< .mine
        public PythonConsole(PythonTextEditor textEditor, CommandLine commandLine)
        {   
            waitHandles = new WaitHandle[] { lineReceivedEvent, disposedEvent };
=======
            this.commandLine = commandLine;
>>>>>>> .r128

<<<<<<< .mine
            this.commandLine = commandLine;
=======
            this.textEditor = textEditor;
            textEditor.CompletionProvider = new PythonConsoleCompletionDataProvider(commandLine);
            textEditor.PreviewKeyDown += textEditor_PreviewKeyDown;
            textEditor.TextEntered += textEditor_TextEntered;
            textEditor.TextEntering += textEditor_TextEntering;
            consoleThread = Thread.CurrentThread;
            
            statementsExecutionThread = new Thread(new ThreadStart(DispatcherThreadStartingPoint));
            statementsExecutionThread.SetApartmentState(ApartmentState.STA);
            statementsExecutionThread.IsBackground = true;
            statementsExecutionThread.Start();
>>>>>>> .r128

<<<<<<< .mine
            this.textEditor = textEditor;
            textEditor.CompletionProvider = new PythonConsoleCompletionDataProvider(commandLine);
            textEditor.PreviewKeyDown += textEditor_PreviewKeyDown;
            textEditor.TextEntered += textEditor_TextEntered;
            textEditor.TextEntering += textEditor_TextEntering;
            consoleThread = Thread.CurrentThread;
            
            statementsExecutionThread = new Thread(new ThreadStart(DispatcherThreadStartingPoint));
            statementsExecutionThread.SetApartmentState(ApartmentState.STA);
            statementsExecutionThread.IsBackground = true;
            statementsExecutionThread.Start();
=======
            // These definition is only required when running outside REP loop.
            prompt = ">>> ";
            this.textEditor.textArea.Dispatcher.Invoke(new Action(delegate()
            {
                CommandBinding pasteBinding = null;
                CommandBinding copyBinding = null;
                CommandBinding cutBinding = null;
                CommandBinding undoBinding = null;
                CommandBinding deleteBinding = null;
                foreach (CommandBinding commandBinding in (this.textEditor.textArea.CommandBindings))
                {
                    if (commandBinding.Command == ApplicationCommands.Paste) pasteBinding = commandBinding;
                    if (commandBinding.Command == ApplicationCommands.Copy) copyBinding = commandBinding;
                    if (commandBinding.Command == ApplicationCommands.Cut) cutBinding = commandBinding;
                    if (commandBinding.Command == ApplicationCommands.Undo) undoBinding = commandBinding;
                    if (commandBinding.Command == ApplicationCommands.Delete) deleteBinding = commandBinding;
                }
                // Remove current bindings completely from control. These are static so modifying them will cause other
                // controls' behaviour to change too.
                if (pasteBinding != null) this.textEditor.textArea.CommandBindings.Remove(pasteBinding);
                if (copyBinding != null) this.textEditor.textArea.CommandBindings.Remove(copyBinding);
                if (cutBinding != null) this.textEditor.textArea.CommandBindings.Remove(cutBinding);
                if (undoBinding != null) this.textEditor.textArea.CommandBindings.Remove(undoBinding);
                if (deleteBinding != null) this.textEditor.textArea.CommandBindings.Remove(deleteBinding);
                this.textEditor.textArea.CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, OnPaste, CanPaste));
                this.textEditor.textArea.CommandBindings.Add(new CommandBinding(ApplicationCommands.Copy, OnCopy, PythonEditingCommandHandler.CanCutOrCopy));
                this.textEditor.textArea.CommandBindings.Add(new CommandBinding(ApplicationCommands.Cut, PythonEditingCommandHandler.OnCut, CanCut));
                this.textEditor.textArea.CommandBindings.Add(new CommandBinding(ApplicationCommands.Undo, OnUndo, CanUndo));
                this.textEditor.textArea.CommandBindings.Add(new CommandBinding(ApplicationCommands.Delete, PythonEditingCommandHandler.OnDelete(ApplicationCommands.NotACommand), CanDeleteCommand));
            
            }));
            CodeContext codeContext = DefaultContext.Default;
            ClrModule.SetCommandDispatcher(codeContext, DispatchCommand);
        }
>>>>>>> .r128

<<<<<<< .mine
            // These definition is only required when running outside REP loop.
            prompt = ">>> ";
            this.textEditor.textArea.Dispatcher.Invoke(new Action(delegate()
            {
                CommandBinding pasteBinding = null;
                CommandBinding copyBinding = null;
                CommandBinding cutBinding = null;
                CommandBinding undoBinding = null;
                CommandBinding deleteBinding = null;
                foreach (CommandBinding commandBinding in (this.textEditor.textArea.CommandBindings))
                {
                    if (commandBinding.Command == ApplicationCommands.Paste) pasteBinding = commandBinding;
                    if (commandBinding.Command == ApplicationCommands.Copy) copyBinding = commandBinding;
                    if (commandBinding.Command == ApplicationCommands.Cut) cutBinding = commandBinding;
                    if (commandBinding.Command == ApplicationCommands.Undo) undoBinding = commandBinding;
                    if (commandBinding.Command == ApplicationCommands.Delete) deleteBinding = commandBinding;
                }
                // Remove current bindings completely from control. These are static so modifying them will cause other
                // controls' behaviour to change too.
                if (pasteBinding != null) this.textEditor.textArea.CommandBindings.Remove(pasteBinding);
                if (copyBinding != null) this.textEditor.textArea.CommandBindings.Remove(copyBinding);
                if (cutBinding != null) this.textEditor.textArea.CommandBindings.Remove(cutBinding);
                if (undoBinding != null) this.textEditor.textArea.CommandBindings.Remove(undoBinding);
                if (deleteBinding != null) this.textEditor.textArea.CommandBindings.Remove(deleteBinding);
                this.textEditor.textArea.CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, OnPaste, CanPaste));
                this.textEditor.textArea.CommandBindings.Add(new CommandBinding(ApplicationCommands.Copy, OnCopy, PythonEditingCommandHandler.CanCutOrCopy));
                this.textEditor.textArea.CommandBindings.Add(new CommandBinding(ApplicationCommands.Cut, PythonEditingCommandHandler.OnCut, CanCut));
                this.textEditor.textArea.CommandBindings.Add(new CommandBinding(ApplicationCommands.Undo, OnUndo, CanUndo));
                this.textEditor.textArea.CommandBindings.Add(new CommandBinding(ApplicationCommands.Delete, PythonEditingCommandHandler.OnDelete(ApplicationCommands.NotACommand), CanDeleteCommand));
            
            }));
            CodeContext codeContext = DefaultContext.Default;
            ClrModule.SetCommandDispatcher(codeContext, DispatchCommand);
        }
=======
        /// <summary>
        /// Set the command dispatcher to another dispatcher if necessary.
        /// </summary>        
        public void SetCommandDispatcher(Action<Action> dispatcher)
        {
            CodeContext codeContext = DefaultContext.Default;
            ClrModule.SetCommandDispatcher(codeContext, dispatcher);
        }
>>>>>>> .r128

<<<<<<< .mine
        protected void DispatchCommand(Delegate command)
        {
            if (command != null)
            {
                dispatcherWindow.Dispatcher.Invoke(DispatcherPriority.Normal, (Action) (delegate() { 
                    ReplCommands.Enqueue((Action)command); }));
            }
        }
=======
        protected void DispatchCommand(Delegate command)
        {
            if (command != null)
            {
                dispatcherWindow.Dispatcher.Invoke(DispatcherPriority.Normal, command);
            }
        }
>>>>>>> .r128

        public void Dispose()
        {
            disposedEvent.Set();
            textEditor.PreviewKeyDown -= textEditor_PreviewKeyDown;
            textEditor.TextEntered -= textEditor_TextEntered;
            textEditor.TextEntering -= textEditor_TextEntering;
        }

        public TextWriter Output
        {
            get { return null; }
            set { }
        }

        public TextWriter ErrorOutput
        {
            get { return null; }
            set { }
        }

        #region CommandHandling
        protected void CanPaste(object target, CanExecuteRoutedEventArgs args)
        {
            if (IsInReadOnlyRegion)
            {
                args.CanExecute = false;
            }
            else
                args.CanExecute = true;
        }

        protected void CanCut(object target, CanExecuteRoutedEventArgs args)
        {
            if (!CanDelete)
            {
                args.CanExecute = false;
            }
            else
                PythonEditingCommandHandler.CanCutOrCopy(target, args);
        }

        protected void CanDeleteCommand(object target, CanExecuteRoutedEventArgs args)
        {
            if (!CanDelete)
            {
                args.CanExecute = false;
            }
            else
                PythonEditingCommandHandler.CanDelete(target, args);
        }

        protected void CanUndo(object target, CanExecuteRoutedEventArgs args)
        {
            args.CanExecute = false;
        }

        protected void OnPaste(object target, ExecutedRoutedEventArgs args)
        {
            if (target != textEditor.textArea) return;
            TextArea textArea = textEditor.textArea;
            if (textArea != null && textArea.Document != null)
            {
                Debug.WriteLine(Clipboard.GetText(TextDataFormat.Html));

                // convert text back to correct newlines for this document
                string newLine = TextUtilities.GetNewLineFromDocument(textArea.Document, textArea.Caret.Line);
                string text = TextUtilities.NormalizeNewLines(Clipboard.GetText(), newLine);
                string[] commands = text.Split(new String[] {newLine}, StringSplitOptions.None);
                string scriptText = "";
                if (commands.Length > 1)
                {
                    text = newLine;
                    foreach (string command in commands)
                    {
                        text += "... " + command + newLine;
                        scriptText += command.Replace("\t", "   ") + newLine;
                    }
                }

                if (!string.IsNullOrEmpty(text))
                {
                    bool fullLine = textArea.Options.CutCopyWholeLine && Clipboard.ContainsData(LineSelectedType);
                    bool rectangular = Clipboard.ContainsData(RectangleSelection.RectangularSelectionDataType);
                    if (fullLine)
                    {
                        DocumentLine currentLine = textArea.Document.GetLineByNumber(textArea.Caret.Line);
                        if (textArea.ReadOnlySectionProvider.CanInsert(currentLine.Offset))
                        {
                            textArea.Document.Insert(currentLine.Offset, text);
                        }
                    }
                    else if (rectangular && textArea.Selection.IsEmpty)
                    {
                        if (!RectangleSelection.PerformRectangularPaste(textArea, textArea.Caret.Offset, text, false))
                            textEditor.Write(text, true);
                    }
                    else
                    {
                        textEditor.Write(text, true);
                    }
                }
                textArea.Caret.BringCaretToView();
                args.Handled = true;

                if (commands.Length > 1)
                {
                    lock (this.scriptText)
                    {
                        this.scriptText = scriptText;
                    }
                    dispatcherWindow.Dispatcher.BeginInvoke(new Action(delegate() { ExecuteStatements(); }));
                }
            }
        }

        protected void OnCopy(object target, ExecutedRoutedEventArgs args)
        {
            if (target != textEditor.textArea) return;
            if (textEditor.SelectionLength == 0)
            {
                // Send the 'Ctrl-C' abort 
                if (!IsInReadOnlyRegion)
                {
                    textEditor.Column = GetLastTextEditorLine().Length + 1;
                    textEditor.Write(Environment.NewLine);
                }
                if (statementsExecuting) statementsExecutionThread.Abort(new Microsoft.Scripting.KeyboardInterruptException(""));
                else consoleThread.Abort(new Microsoft.Scripting.KeyboardInterruptException(""));
                args.Handled = true;
            }
            else PythonEditingCommandHandler.OnCopy(target, args);
        }

        const string LineSelectedType = "MSDEVLineSelect";  // This is the type VS 2003 and 2005 use for flagging a whole line copy

        protected void OnUndo(object target, ExecutedRoutedEventArgs args)
        {
        }
        #endregion

        /// <summary>
        /// Run externally provided statements in the Console Engine. 
        /// </summary>
        /// <param name="statements"></param>
        public void RunStatements(string statements)
        {
            MoveToHomePosition();
            lock (this.scriptText)
            {
                this.scriptText = statements;
            }
            TextArea textArea = textEditor.textArea;
            textEditor.Write(TextUtilities.GetNewLineFromDocument(textArea.Document, textArea.Caret.Line));
            dispatcherWindow.Dispatcher.BeginInvoke(new Action(delegate() { ExecuteStatements(); }));
        }

        private void DispatcherThreadStartingPoint()
        {
            dispatcherWindow = new Window();
            System.Windows.Threading.Dispatcher.Run();
        }

        /// <summary>
        /// Run on the statement execution thread. 
        /// </summary>
        void ExecuteStatements()
        {
            lock (scriptText)
            {
                ScriptSource scriptSource = commandLine.ScriptScope.Engine.CreateScriptSourceFromString(scriptText, SourceCodeKind.Statements);
                string error = "";
                try
                {
                    statementsExecuting = true;
                    scriptSource.Execute(commandLine.ScriptScope);
                    statementsExecuting = false;
                }
                catch (ThreadAbortException tae)
                {
                    if (tae.ExceptionState is Microsoft.Scripting.KeyboardInterruptException) Thread.ResetAbort();
                    error = "KeyboardInterrupt" + System.Environment.NewLine;
                }
                catch (Microsoft.Scripting.SyntaxErrorException exception)
                {
                    ExceptionOperations eo;
                    eo = commandLine.ScriptScope.Engine.GetService<ExceptionOperations>();
                    error = eo.FormatException(exception);
                }
                catch (Exception exception)
                {
                    ExceptionOperations eo;
                    eo = commandLine.ScriptScope.Engine.GetService<ExceptionOperations>();
                    error = eo.FormatException(exception) + System.Environment.NewLine;
                }
                statementsExecuting = false;
                if (error != "") textEditor.Write(error);
                textEditor.Write(prompt);
            }
        }

        /// <summary>
        /// Returns the next line typed in by the console user. If no line is available this method
        /// will block.
        /// </summary>
        public string ReadLine(int autoIndentSize)
        {
            string indent = String.Empty;
            if (autoIndentSize > 0)
            {
                indent = String.Empty.PadLeft(autoIndentSize);
                Write(indent, Style.Prompt);
            }

            string line = ReadLineFromTextEditor();
            if (line != null)
            {
                return indent + line;
            }
            return null;
        }

        /// <summary>
        /// Writes text to the console.
        /// </summary>
        public void Write(string text, Style style)
        {
            textEditor.Write(text);
            if (style == Style.Prompt)
            {
                promptLength = text.Length;
                if (!consoleInitialized)
                {
                    consoleInitialized = true;
                    if (ConsoleInitialized != null) ConsoleInitialized(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Writes text followed by a newline to the console.
        /// </summary>
        public void WriteLine(string text, Style style)
        {
            Write(text + Environment.NewLine, style);
        }

        /// <summary>
        /// Writes an empty line to the console.
        /// </summary>
        public void WriteLine()
        {
            Write(Environment.NewLine, Style.Out);
        }

        /// <summary>
        /// Indicates whether there is a line already read by the console and waiting to be processed.
        /// </summary>
        public bool IsLineAvailable
        {
            get
            {
                lock (previousLines)
                {
                    return previousLines.Count > 0;
                }
            }
        }

        /// <summary>
        /// Gets the text that is yet to be processed from the console. This is the text that is being
        /// typed in by the user who has not yet pressed the enter key.
        /// </summary>
        public string GetCurrentLine()
        {
            string fullLine = GetLastTextEditorLine();
            return fullLine.Substring(promptLength);
        }

        /// <summary>
        /// Gets the lines that have not been returned by the ReadLine method. This does not
        /// include the current line.
        /// </summary>
        public string[] GetUnreadLines()
        {
            return previousLines.ToArray();
        }

        string GetLastTextEditorLine()
        {
            return textEditor.GetLine(textEditor.TotalLines - 1);
        }

        string ReadLineFromTextEditor()
        {
            int result = WaitHandle.WaitAny(waitHandles);
            if (result == lineReceivedEventIndex)
            {
                lock (previousLines)
                {
                    string line = previousLines[0];
                    previousLines.RemoveAt(0);
                    if (previousLines.Count == 0)
                    {
                        lineReceivedEvent.Reset();
                    }
                    return line;
                }
            }
            return null;
        }

        /// <summary>
        /// Processes characters entered into the text editor by the user.
        /// </summary>
        void textEditor_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Delete:
                    if (!CanDelete) e.Handled = true;
                    return;
                case Key.Tab:
                    if (IsInReadOnlyRegion) e.Handled = true;
                    return;
                case Key.Back:
                    if (!CanBackspace) e.Handled = true;
                    return;
                case Key.Home:
                    MoveToHomePosition();
                    e.Handled = true;
                    return;
                case Key.Down:
                    MoveToNextCommandLine();
                    e.Handled = true;
                    return;
                case Key.Up:
                    MoveToPreviousCommandLine();
                    e.Handled = true;
                    return;
            }
        }

        public bool AllowFullAutocompletion
        {
            get { return allowFullAutocompletion; }
            set { allowFullAutocompletion = value; }
        }

        public bool AllowCtrlSpaceAutocompletion
        {
            get { return allowCtrlSpaceAutocompletion; }
            set { allowCtrlSpaceAutocompletion = value; }
        }

        /// <summary>
        /// Processes characters entering into the text editor by the user.
        /// </summary>
        void textEditor_TextEntering(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Length > 0)
            {
                if (!char.IsLetterOrDigit(e.Text[0]))
                {
                    // Whenever a non-letter is typed while the completion window is open,
                    // insert the currently selected element.
                    textEditor.RequestCompletioninsertion(e);
                }
            }

            if (IsInReadOnlyRegion)
            {
                e.Handled = true;
            }
            else
            {
                if (e.Text[0] == '\n')
                {
                    OnEnterKeyPressed();
                }

                if (e.Text[0] == '.' && allowFullAutocompletion)
                {
                    textEditor.ShowCompletionWindow();
                }

                if ((e.Text[0] == ' ') && (Keyboard.Modifiers == ModifierKeys.Control))
                {
                    e.Handled = true;
                    if (allowCtrlSpaceAutocompletion) textEditor.ShowCompletionWindow();
                }
            }
        }

        /// <summary>
        /// Processes characters entered into the text editor by the user.
        /// </summary>
        void textEditor_TextEntered(object sender, TextCompositionEventArgs e)
        {
        }

        /// <summary>
        /// Move cursor to the end of the line before retrieving the line.
        /// </summary>
        void OnEnterKeyPressed()
        {
            textEditor.StopCompletion();
            if (textEditor.WriteInProgress) return;
            lock (previousLines)
            {
                // Move cursor to the end of the line.
                textEditor.Column = GetLastTextEditorLine().Length + 1;

                // Append line.
                string currentLine = GetCurrentLine();
                previousLines.Add(currentLine);
                commandLineHistory.Add(currentLine);

                lineReceivedEvent.Set();
            }
        }

        /// <summary>
        /// Returns true if the cursor is in a readonly text editor region.
        /// </summary>
        bool IsInReadOnlyRegion
        {
            get { return IsCurrentLineReadOnly || IsInPrompt; }
        }

        /// <summary>
        /// Only the last line in the text editor is not read only.
        /// </summary>
        bool IsCurrentLineReadOnly
        {
            get { return textEditor.Line < textEditor.TotalLines; }
        }

        /// <summary>
        /// Determines whether the current cursor position is in a prompt.
        /// </summary>
        bool IsInPrompt
        {
            get { return textEditor.Column - promptLength - 1 < 0; }
        }

        /// <summary>
        /// Returns true if the user can delete at the current cursor position.
        /// </summary>
        bool CanDelete
        {
            get
            {
                if (textEditor.SelectionLength > 0) return SelectionIsDeletable;
                else return !IsInReadOnlyRegion;
            }
        }

        /// <summary>
        /// Returns true if the user can backspace at the current cursor position.
        /// </summary>
        bool CanBackspace
        {
            get
            {
                if (textEditor.SelectionLength > 0) return SelectionIsDeletable;
                else
                {
                    int cursorIndex = textEditor.Column - promptLength - 1;
                    return !IsCurrentLineReadOnly && (cursorIndex > 0 || (cursorIndex == 0 && textEditor.SelectionLength > 0));
                }
            }
        }

        bool SelectionIsDeletable
        {
            get
            {
                return (!textEditor.SelectionIsMultiline
                    && !IsCurrentLineReadOnly
                    && (textEditor.SelectionStartColumn - promptLength - 1 >= 0)
                    && (textEditor.SelectionEndColumn - promptLength - 1 >= 0));
            }
        }

        /// <summary>
        /// The home position is at the start of the line after the prompt.
        /// </summary>
        void MoveToHomePosition()
        {
            textEditor.Line = textEditor.TotalLines;
            textEditor.Column = promptLength + 1;
        }

        /// <summary>
        /// Shows the previous command line in the command line history.
        /// </summary>
        void MoveToPreviousCommandLine()
        {
            if (commandLineHistory.MovePrevious())
            {
                ReplaceCurrentLineTextAfterPrompt(commandLineHistory.Current);
            }
        }

        /// <summary>
        /// Shows the next command line in the command line history.
        /// </summary>
        void MoveToNextCommandLine()
        {
            textEditor.Line = textEditor.TotalLines;
            if (commandLineHistory.MoveNext())
            {
                ReplaceCurrentLineTextAfterPrompt(commandLineHistory.Current);
            }
        }

        /// <summary>
        /// Replaces the current line text after the prompt with the specified text.
        /// </summary>
        void ReplaceCurrentLineTextAfterPrompt(string text)
        {
            string currentLine = GetCurrentLine();
            textEditor.Replace(promptLength, currentLine.Length, text);

            // Put cursor at end.
            textEditor.Column = promptLength + text.Length + 1;
        }
    }
}
