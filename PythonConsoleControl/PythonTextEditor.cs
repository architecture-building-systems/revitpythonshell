// Copyright (c) 2010 Joe Moorhouse

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.CodeCompletion;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Input;
using System.Threading;
using System.Diagnostics;

namespace PythonConsoleControl
{
    /// <summary>
    /// Interface console to AvalonEdit and handle autocompletion.
    /// </summary>
    public class PythonTextEditor
    {
        internal TextEditor textEditor;
        internal TextArea textArea;
        StringBuilder writeBuffer = new StringBuilder();
        volatile bool writeInProgress = false;
        PythonConsoleCompletionWindow completionWindow = null;
        int completionEventIndex = 0;
        int descriptionEventIndex = 1;
        WaitHandle[] completionWaitHandles;
        AutoResetEvent completionRequestedEvent = new AutoResetEvent(false);
        AutoResetEvent descriptionRequestedEvent = new AutoResetEvent(false);
        Thread completionThread;
        PythonConsoleCompletionDataProvider completionProvider = null;
        Action<Action> completionDispatcher = new Action<Action>((command) => command()); // dummy completion dispatcher

        public PythonTextEditor(TextEditor textEditor)
        {
            this.textEditor = textEditor;
            this.textArea = textEditor.TextArea;
            completionWaitHandles = new WaitHandle[] { completionRequestedEvent, descriptionRequestedEvent };
            completionThread = new Thread(new ThreadStart(Completion));
            completionThread.Priority = ThreadPriority.Lowest;
            //completionThread.SetApartmentState(ApartmentState.STA);
            completionThread.IsBackground = true;
            completionThread.Start();
        }

        /// <summary>
        /// Set the dispatcher to use to force code completion to happen on a specific thread
        /// if necessary.
        /// </summary>
        public void SetCompletionDispatcher(Action<Action> newDispatcher)
        {
            completionDispatcher = newDispatcher;
        }

        public bool WriteInProgress
        {
            get { return writeInProgress; }
        }

        public ICollection<CommandBinding> CommandBindings
        {
            get { return (this.textArea.ActiveInputHandler as TextAreaDefaultInputHandler).CommandBindings; } 
        }

        public void Write(string text)
        {
            Write(text, false);
        }

        Stopwatch sw;

        public void Write(string text, bool allowSynchronous)
        {
            //text = text.Replace("\r\r\n", "\r\n");
            text = text.Replace("\r\r\n", "\r");
            text = text.Replace("\r\n", "\r");
            if (allowSynchronous)
            {
                MoveToEnd();
                PerformTextInput(text);
                return;
            }
            lock (writeBuffer)
            {
                writeBuffer.Append(text);
            }
            if (!writeInProgress)
            {
                writeInProgress = true;
                ThreadPool.QueueUserWorkItem(new WaitCallback(CheckAndOutputWriteBuffer));
                sw = Stopwatch.StartNew();
            }
        }

        private void CheckAndOutputWriteBuffer(Object stateInfo)
        {
            AutoResetEvent writeCompletedEvent = new AutoResetEvent(false);
            Action action = new Action(delegate()
            {
                string toWrite;
                lock (writeBuffer)
                {
                    toWrite = writeBuffer.ToString();
                    writeBuffer.Remove(0, writeBuffer.Length);
                    //writeBuffer.Clear();
                }
                MoveToEnd();
                PerformTextInput(toWrite);
                writeCompletedEvent.Set();
            });
            while (true)
            {
                // Clear writeBuffer and write out.
                textArea.Dispatcher.BeginInvoke(action, DispatcherPriority.Normal);
                // Check if writeBuffer has refilled in the meantime; if so clear and write out again.
                writeCompletedEvent.WaitOne();
                lock (writeBuffer)
                {
                    if (writeBuffer.Length == 0)
                    {
                        writeInProgress = false;
                        break;
                    }
                }
            }
        }

        private void MoveToEnd()
        {
            int lineCount = textArea.Document.LineCount;
            if (textArea.Caret.Line != lineCount) textArea.Caret.Line = textArea.Document.LineCount;
            int column = textArea.Document.Lines[lineCount - 1].Length + 1;
            if (textArea.Caret.Column != column) textArea.Caret.Column = column;
        }

        private void PerformTextInput(string text)
        {
            if (text == "\n" || text == "\r\n")
            {
                string newLine = TextUtilities.GetNewLineFromDocument(textArea.Document, textArea.Caret.Line);
                using (textArea.Document.RunUpdate())
                {
                    textArea.Selection.ReplaceSelectionWithText(textArea, newLine);
                }
            }
            else
                textArea.Selection.ReplaceSelectionWithText(textArea, text);
            textArea.Caret.BringCaretToView();
        }

        public int Column
        {
            get { return textArea.Caret.Column; }
            set { textArea.Caret.Column = value; }
        }

        /// <summary>
        /// Gets the current cursor line.
        /// </summary>
        public int Line
        {
            get { return textArea.Caret.Line; }
            set { textArea.Caret.Line = value; }
        }

        /// <summary>
        /// Gets the total number of lines in the text editor.
        /// </summary>
        public int TotalLines
        {
            get { return textArea.Document.LineCount; }
        }

        public delegate string StringAction();
        /// <summary>
        /// Gets the text for the specified line.
        /// </summary>
        public string GetLine(int index)
        {
            return (string)textArea.Dispatcher.Invoke(new StringAction(delegate()
            {
                DocumentLine line = textArea.Document.Lines[index];
                return textArea.Document.GetText(line);
            }));
        }

        /// <summary>
        /// Replaces the text at the specified index on the current line with the specified text.
        /// </summary>
        public void Replace(int index, int length, string text)
        {
            //int currentLine = textArea.Caret.Line - 1;
            int currentLine = textArea.Document.LineCount - 1;
            int startOffset = textArea.Document.Lines[currentLine].Offset;
            textArea.Document.Replace(startOffset + index, length, text); 
        }

        public event TextCompositionEventHandler TextEntering
        {
            add { textArea.TextEntering += value; }
            remove { textArea.TextEntering -= value; }
        }

        public event TextCompositionEventHandler TextEntered
        {
            add { textArea.TextEntered += value; }
            remove { textArea.TextEntered -= value; }
        }

        public event KeyEventHandler PreviewKeyDown
        {
            add { textArea.PreviewKeyDown += value; }
            remove { textArea.PreviewKeyDown -= value; }
        }

        public int SelectionStart
        {
            get
            {
                return textArea.Selection.SurroundingSegment.Offset;
            }
        }

        public int SelectionLength
        {
            get
            {
                return textArea.Selection.Length;
            }
        }

        public bool SelectionIsMultiline
        {
            get
            {
                return textArea.Selection.IsMultiline(textArea.Document);
            }
        }

        public int SelectionStartColumn
        {
            get
            {
                int startOffset = textArea.Selection.SurroundingSegment.Offset;
                return startOffset - textArea.Document.GetLineByOffset(startOffset).Offset + 1;
            }
        }

        public int SelectionEndColumn
        {
            get
            {
                int endOffset = textArea.Selection.SurroundingSegment.EndOffset;
                return endOffset - textArea.Document.GetLineByOffset(endOffset).Offset + 1;
            }
        }

        public PythonConsoleCompletionDataProvider CompletionProvider
        {
            get { return completionProvider; }
            set { completionProvider = value; }
        }

        public Thread CompletionThread
        {
            get { return completionThread; }
        }

        public bool StopCompletion()
        {
            if (completionProvider.AutocompletionInProgress)
            {
                // send Ctrl-C abort
                completionThread.Abort(new Microsoft.Scripting.KeyboardInterruptException(""));
                return true;
            }
            return false;
        }

        public PythonConsoleCompletionWindow CompletionWindow
        {
            get { return completionWindow; }
        }

        public void ShowCompletionWindow()
        {
            completionRequestedEvent.Set();
        }

        public void UpdateCompletionDescription()
        {
            descriptionRequestedEvent.Set();
        }

        /// <summary>
        /// Perform completion actions on the background completion thread.
        /// </summary>
        void Completion()
        {
            while (true)
            {
                int action = WaitHandle.WaitAny(completionWaitHandles);
                if (action == completionEventIndex && completionProvider != null) BackgroundShowCompletionWindow();
                if (action == descriptionEventIndex && completionProvider != null && completionWindow != null) BackgroundUpdateCompletionDescription();
            }
        }

        /// <summary>
        /// Obtain completions (this runs in its own thread)
        /// </summary>
        internal void BackgroundShowCompletionWindow() //ICompletionItemProvider
        {
			// provide AvalonEdit with the data:
            string itemForCompletion = "";
            textArea.Dispatcher.Invoke(new Action(delegate()
            {
                DocumentLine line = textArea.Document.Lines[textArea.Caret.Line - 1];
                itemForCompletion = textArea.Document.GetText(line);
            }));

            
            completionDispatcher.Invoke(new Action(delegate()
            {
                try
                {
                    ICompletionData[] completions = completionProvider.GenerateCompletionData(itemForCompletion);
                            
                    if (completions != null && completions.Length > 0) textArea.Dispatcher.BeginInvoke(new Action(delegate()
                    {
                        completionWindow = new PythonConsoleCompletionWindow(textArea, this);
                        IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
                        foreach (ICompletionData completion in completions)
                        {
                            data.Add(completion);
                        }
                        completionWindow.Show();
                        completionWindow.Closed += delegate
                        {
                            completionWindow = null;
                        };
                    }));
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.ToString(), "Error");
                }

            }));            
        }

        internal void BackgroundUpdateCompletionDescription()
        {
            completionDispatcher.Invoke(new Action(delegate()
            {
                try
                {
                    completionWindow.UpdateCurrentItemDescription();
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.ToString(), "Error");
                }

            }));            
        }

        public void RequestCompletioninsertion(TextCompositionEventArgs e)
        {
            if (completionWindow != null) completionWindow.CompletionList.RequestInsertion(e);
            // if autocompletion still in progress, terminate
            StopCompletion();
        }

    }
}

    
   
