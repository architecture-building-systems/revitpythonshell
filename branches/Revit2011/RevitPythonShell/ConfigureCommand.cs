using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.Revit;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using System.Windows.Forms;

namespace RevitPythonShell
{
    /// <summary>
    /// Open the configuration dialog.
    /// </summary>
    [Regeneration(RegenerationOption.Manual)]
    [Transaction(TransactionMode.Manual)]
    class ConfigureCommand: IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var dialog = new ConfigureCommandsForm();
            dialog.ShowDialog();

            MessageBox.Show("Restart Revit to see changes to the commands in the Ribbon", "Configure RevitPythonShell", MessageBoxButtons.OK, MessageBoxIcon.Information);

            return Result.Succeeded;
        }
    }
}
