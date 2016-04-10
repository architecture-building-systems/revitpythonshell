using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Xml.Linq;
using Autodesk.Revit;
using Autodesk.Revit.UI;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RevitPythonLoader
{
    [Regeneration(RegenerationOption.Manual)]
    [Transaction(TransactionMode.Manual)]
    class RevitPythonLoaderApplication : IExternalApplication
    {
        private static string versionNumber;

        /// <summary>
        /// Hook into Revit to allow starting a command.
        /// </summary>
        Result IExternalApplication.OnStartup(UIControlledApplication application)
        {

            try
            {
                versionNumber = application.ControlledApplication.VersionNumber;
                if (application.ControlledApplication.VersionName.ToLower().Contains("vasari"))
                {
                    versionNumber = "_Vasari";
                }

                ExecuteStartupScript(application);

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Error setting up RevitPythonLoader", ex.ToString());
                return Result.Failed;
            }
        }

        private static void ExecuteStartupScript(UIControlledApplication uiControlledApplication)
        {
            // we need a UIApplication object to assign as `__revit__` in python...
            var fi = uiControlledApplication.GetType().GetField("m_application", BindingFlags.NonPublic | BindingFlags.Instance);
            var uiApplication = (UIApplication)fi.GetValue(uiControlledApplication);
            // execute StartupScript
            var startupScript = GetStartupScript();
            if (startupScript != null)
            {
                var executor = new ScriptExecutor(uiApplication, uiControlledApplication);
                var result = executor.ExecuteScript(startupScript, GetStartupScriptPath() );
                if (result == (int)Result.Failed)
                {
                    TaskDialog.Show("RevitPythonLoader", executor.Message);
                }
            }
        }

        public static string GetStartupScriptPath()
        {
            var startupScriptName = "__init__.py";

            var pyrevitEnvVar = Environment.GetEnvironmentVariable("pyrevit");
            if (Directory.Exists(pyrevitEnvVar))
            {
                return Path.Combine(pyrevitEnvVar, startupScriptName);
            }
            else  // if location can not be aquired from the environment var, check dll folder for __init__.py script
            {
                var dllfolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                return Path.Combine(dllfolder, startupScriptName);
            }
        }

        public static string GetStartupScript()
        {
            var startupScriptFullPath = GetStartupScriptPath();
            if (File.Exists(startupScriptFullPath))
            {
                using (var reader = File.OpenText(startupScriptFullPath))
                {
                    var source = reader.ReadToEnd();
                    return source;
                }
            }
            // no startup script found
            return null;
        }

        Result IExternalApplication.OnShutdown(UIControlledApplication application)
        {
            // FIXME: deallocate the python shell...
            return Result.Succeeded;
        }
    }
}
