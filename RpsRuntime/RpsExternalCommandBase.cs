using System;
using System.IO;
using System.Xml.Linq;
using Autodesk.Revit.UI;

namespace RpsRuntime
{
    /// <summary>
    /// An abstract base class for ExternalCommand instances created by the DeployRpsAddin projects.
    /// All that has to be done is subclass from this class - the script to run will
    /// be found inside the assembly embedded resources of the subclass.
    /// </summary>
    public abstract class RpsExternalCommandBase: IExternalCommand
    {

        /// <summary>
        /// Find the script in the resources and run it.
        /// </summary>
        Result IExternalCommand.Execute(ExternalCommandData commandData, ref string message, Autodesk.Revit.DB.ElementSet elements)
        {
            var executor = new ScriptExecutor(GetConfig(), commandData, message, elements);

            var className = this.GetType().Name;            // e.g. "ec_helloworld"
            var scriptName = className.Substring(3) + ".py"; // e.g. "helloworld.py
            var assembly = this.GetType().Assembly;

            var source = new StreamReader(assembly.GetManifestResourceStream(scriptName)).ReadToEnd();

            var result = executor.ExecuteScript(source, Path.Combine(assembly.Location, scriptName));
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
        /// </summary>
        private RpsConfig GetConfig()
        {
            var addinName = Path.GetFileNameWithoutExtension(this.GetType().Assembly.Location);
            var fileName =  addinName + ".xml";
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
