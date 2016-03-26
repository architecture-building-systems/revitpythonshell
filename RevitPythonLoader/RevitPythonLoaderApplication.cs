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
            // var dllfolder = AppDomain.CurrentDomain.BaseDirectory;
            var dllfolder = Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location );
            var dllfullpath = Path.Combine( dllfolder, "__init__.py");
            return dllfullpath;
        }

        public static string GetStartupScript()
        {
            var path = GetStartupScriptPath();
            if (File.Exists(path))
            {
                using (var reader = File.OpenText(path))
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
