using System;
using Autodesk.Revit;

namespace RevitPythonShell
{
    class RevitPythonShellApplication: IExternalApplication
    {
        /// <summary>
        /// Hook into Revit to allow starting a command.
        /// </summary>
        IExternalApplication.Result IExternalApplication.OnStartup(ControlledApplication application)
        {
            RibbonPanel ribbonPanel = application.CreateRibbonPanel("RevitPythonShell");
            ribbonPanel.AddPushButton("RevitPythonShell", "Open Python Shell",
                                      typeof (RevitPythonShellApplication).Assembly.Location,
                                      "RevitPythonShell.StartShellCommand");
            return IExternalApplication.Result.Succeeded;
        }

        IExternalApplication.Result IExternalApplication.OnShutdown(ControlledApplication application)
        {
            // FIXME: deallocate the python shell...
            return IExternalApplication.Result.Succeeded;
        }
    }
}
