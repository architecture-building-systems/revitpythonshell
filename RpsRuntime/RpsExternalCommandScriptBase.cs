using System;
using System.IO;
using System.Xml.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RpsRuntime
{
    /// <summary>
    /// This class is very much like the RpsExternalCommandBase, but instead of looking for the
    /// source of the command in the RpsAddin assembly, it uses a path to the script in the
    /// filesystem.
    /// </summary>
    [Regeneration(RegenerationOption.Manual)]
    [Transaction(TransactionMode.Manual)]
    public class RpsExternalCommandScriptBase: IExternalCommand
    {

        protected string _scriptSource = "";

        public RpsExternalCommandScriptBase(string scriptSource)
        {
            _scriptSource = scriptSource;
        }

        /// <summary>
        /// Overload this method to implement an external command within Revit.
        /// </summary>
        /// <returns>
        /// The result indicates if the execution fails, succeeds, or was canceled by user. If it does not
        /// succeed, Revit will undo any changes made by the external command. 
        /// </returns>
        /// <param name="commandData">An ExternalCommandData object which contains reference to Application and View
        /// needed by external command.</param><param name="message">Error message can be returned by external command. This will be displayed only if the command status
        /// was "Failed".  There is a limit of 1023 characters for this message; strings longer than this will be truncated.</param><param name="elements">Element set indicating problem elements to display in the failure dialog.  This will be used
        /// only if the command status was "Failed".</param>
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // FIXME: somehow fetch back message after script execution...
            var executor = new ScriptExecutor(GetConfig(), commandData, message, elements);

            string source;
            using (var reader = File.OpenText(_scriptSource))
            {
                source = reader.ReadToEnd();
            }

            var result = executor.ExecuteScript(source, _scriptSource);
            message = executor.Message;
            switch (result)
            {
                case (int)Result.Succeeded:
                    return Result.Succeeded;
                case (int)Result.Cancelled:
                    return Result.Cancelled;
                case (int)Result.Failed:
                    return Result.Failed;
                default:
                    return Result.Succeeded;
            }
        }

        /// <summary>
        /// Search for the config file first in the user preferences,
        /// then in the all users preferences.
        /// If not found, a new (empty) config file is created in the user preferences.
        /// The config file has the same name as the assembly containing the ExternalCommand.
        /// </summary>
        private RpsConfig GetConfig()
        {
            var addinName = Path.GetFileNameWithoutExtension(this.GetType().Assembly.Location);
            var fileName = addinName + ".xml";
            var userFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), addinName);

            var userFolderFile = Path.Combine(userFolder, fileName);
            if (File.Exists(userFolderFile))
            {
                return new RpsConfig(userFolderFile);
            }

            var allUserFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), addinName);
            var allUserFolderFile = Path.Combine(allUserFolder, addinName);
            if (File.Exists(allUserFolderFile))
            {
                return new RpsConfig(allUserFolderFile);
            }

            // create a new file in users appdata and return that
            var doc = new XDocument(
                new XElement("RevitPythonShell",
                    new XElement("SearchPaths"),
                    new XElement("Variables")));

            if (!Directory.Exists(userFolder))
            {
                Directory.CreateDirectory(userFolder);
            }

            doc.Save(userFolderFile);
            return new RpsConfig(userFolderFile);
        }
    }
}
