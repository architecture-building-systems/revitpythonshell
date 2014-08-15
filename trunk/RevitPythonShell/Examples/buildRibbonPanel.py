'''
buildRibbonPanel.py - demonstrate manually building a
RibbonPanel with stacked buttons on in Revit.

This script is meant to be used as a startup-script as altering
the RibbonPanel after the IExternalCommand.OnStartup() event
is not allowed.

You might want to change the paths to the scripts to reflect your system.
'''
from RevitPythonShell.RpsRuntime import ExternalCommandAssemblyBuilder
from Autodesk.Revit.UI import *  # noqa

import clr
clr.AddReference('PresentationCore')
from System.Windows.Media.Imaging import BitmapImage
from System import Uri

SCRIPT_PATH = r"Z:\projects\revitpythonshell\RevitPythonShell\Examples\helloworld.py"  # noqa
LARGE_IMG_PATH = r"Z:\projects\revitpythonshell\RevitPythonShell\Examples\PythonScript32x32.png"  # noqa
SMALL_IMG_PATH = r"Z:\projects\revitpythonshell\RevitPythonShell\Examples\PythonScript16x16.png"  # noqa

DLL_FOLDER = r"C:\Users\darthoma\AppData\Roaming\RevitPythonShell2015"
DLL_PATH = DLL_FOLDER + r"\HelloWorldRibbon.dll"

builder = ExternalCommandAssemblyBuilder()
builder.BuildExternalCommandAssembly(
    DLL_PATH,
    {'HelloWorld': SCRIPT_PATH})

panel = __uiControlledApplication__.CreateRibbonPanel('HelloWorldPanel')
pbd = PushButtonData('pb_HelloWorld', 'hello, world!', DLL_PATH, 'HelloWorld')
pbd.Image = BitmapImage(Uri(SMALL_IMG_PATH))
pbd.LargeImage = BitmapImage(Uri(LARGE_IMG_PATH))
panel.AddItem(pbd)
