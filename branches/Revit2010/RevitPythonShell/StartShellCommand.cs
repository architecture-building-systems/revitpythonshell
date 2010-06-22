using Autodesk.Revit;

namespace RevitPythonShell
{
    /// <summary>
    /// An object of this class is instantiated every time the user clicks on the
    /// button for opening the shell.
    /// </summary>
    public class StartShellCommand: IExternalCommand
    {
        /// <summary>
        /// Open a window to let the user enter python code.
        /// </summary>
        /// <returns></returns>
        public IExternalCommand.Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var gui = new ShellForm();

            return (IExternalCommand.Result) gui.ShowShell(commandData, ref message, elements);
        }
    }
}
