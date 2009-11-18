using Autodesk.Revit;

namespace RevitPythonShell
{
    class StartShellCommand: IExternalCommand
    {
        private static ScriptInput _gui;

        /// <summary>
        /// Open a window to let the user enter python code.
        /// </summary>
        /// <returns></returns>
        public IExternalCommand.Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            _gui = new ScriptInput(commandData.Application);
            _gui.Show();
            _gui.BringToFront();
            return IExternalCommand.Result.Succeeded;
        }
    }
}
