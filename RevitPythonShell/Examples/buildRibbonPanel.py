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

# a normal pushbutton
pbd = PushButtonData('pb_HelloWorld', 'hello, world!', DLL_PATH, 'HelloWorld')
pbd.Image = BitmapImage(Uri(SMALL_IMG_PATH))
pbd.LargeImage = BitmapImage(Uri(LARGE_IMG_PATH))
panel.AddItem(pbd)

# a splitbutton
sb = panel.AddItem(SplitButtonData('sp_HelloWorld', 'HelloBananaSplit'))
for i in range(3):
    pbd = PushButtonData('pb_HelloWorld_split%i' % i,
                         'hello, world!', DLL_PATH, 'HelloWorld')
    pbd.Image = BitmapImage(Uri(SMALL_IMG_PATH))
    pbd.LargeImage = BitmapImage(Uri(LARGE_IMG_PATH))
    sb.AddPushButton(pbd)

# stacked buttons!
pbd0 = PushButtonData('pb_HelloWorld_stack0',
                      'hello, world!', DLL_PATH, 'HelloWorld')
pbd0.Image = BitmapImage(Uri(SMALL_IMG_PATH))
pbd0.LargeImage = BitmapImage(Uri(LARGE_IMG_PATH))
pbd1 = PushButtonData('pb_HelloWorld_stack1',
                      'hello, world!', DLL_PATH, 'HelloWorld')
pbd1.Image = BitmapImage(Uri(SMALL_IMG_PATH))
pbd1.LargeImage = BitmapImage(Uri(LARGE_IMG_PATH))
pbd2 = PushButtonData('pb_HelloWorld_stack2',
                      'hello, world!', DLL_PATH, 'HelloWorld')
pbd2.Image = BitmapImage(Uri(SMALL_IMG_PATH))
pbd2.LargeImage = BitmapImage(Uri(LARGE_IMG_PATH))
panel.AddStackedItems(pbd0, pbd1, pbd2)
