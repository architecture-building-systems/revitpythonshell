# these commands get executed in the current scope
# of each new shell (but not for canned commands)
from Autodesk.Revit.DB import *
from Autodesk.Revit.DB.Architecture import *
from Autodesk.Revit.DB.Analysis import *

uidoc = __revit__.ActiveUIDocument
doc = __revit__.ActiveUIDocument.Document
selection = list(__revit__.ActiveUIDocument.Selection.Elements)

from Autodesk.Revit.UI import TaskDialog
from Autodesk.Revit.UI import UIApplication
def alert(msg):
    TaskDialog.Show('RevitPythonShell', msg)

def quit():
    __window__.Close()
exit = quit

# a fix for the __window__.Close() bug introduced with the non-modal console
class WindowWrapper(object):
    def __init__(self, win):
        self.win = win
    def Close(self):
        self.win.Dispatcher.Invoke(lambda *_: self.win.Close())
    def __getattr__(self, name):
        return getattr(self.win, name)
 __window__ = WindowWrapper(__window__)