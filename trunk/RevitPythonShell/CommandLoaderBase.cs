using System;
using System.IO;
using Autodesk.Revit;

namespace RevitPythonShell
{
    /// <summary>
    /// Starts up a ScriptOutput window for a given canned command.
    /// 
    /// It is expected that this will be inherited by dynamic types that have the field
    /// _scriptSource set to point to a python file that will be executed in the constructor.
    /// </summary>
    public abstract class CommandLoaderBase: IExternalCommand
    {
        protected string _scriptSource = "";

        public CommandLoaderBase(string scriptSource)
        {
            _scriptSource = scriptSource;
        }

        /// <summary>
        /// Overload this method to implement and external command within Revit.
        /// </summary>
        /// <returns>
        /// The result indicates if the execution fails, succeeds, or was canceled by user. If it does not
        /// succeed, Revit will undo any changes made by the external command. 
        /// </returns>
        /// <param name="commandData">An ExternalCommandData object which contains reference to Application and View
        /// needed by external command.</param><param name="message">Error message can be returned by external command. This will be displayed only if the command status
        /// was "Failed".  There is a limit of 1023 characters for this message; strings longer than this will be truncated.</param><param name="elements">Element set indicating problem elements to display in the failure dialog.  This will be used
        /// only if the command status was "Failed".</param>
        public IExternalCommand.Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // FIXME: somehow fetch back message after script execution...
            var executor = new ScriptExecutor(commandData, message, elements);

            string source;
            using (var reader = File.OpenText(_scriptSource))
            {
                source = reader.ReadToEnd();
            }

            var result = executor.ExecuteScript(source);
            message = executor.Message;
            switch (result)
            {
                case (int)IExternalCommand.Result.Succeeded:
                    return IExternalCommand.Result.Succeeded;
                case (int)IExternalCommand.Result.Cancelled:
                    return IExternalCommand.Result.Cancelled;
                case (int)IExternalCommand.Result.Failed:
                    return IExternalCommand.Result.Failed;
                default:
                    return IExternalCommand.Result.Succeeded;
            }
        }
    }
    public class TestInheritor: CommandLoaderBase
    {
        public TestInheritor(): base("test")
        {
        }
    }
}
