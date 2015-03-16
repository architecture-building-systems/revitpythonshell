# Introduction #

RevitPythonShell is all about exploring the Revit API. One of the things that bugged me most, was that whenever I opened up a RevitPythonShell shell, I had to type in the same few commands to get going:

```
# minimum required commands to stuff to do anything with Revit
import clr
clr.AddReference('RevitAPI')
```

And that is just the bare minimum! This is where the InitScript feature comes in: This is a chunk of python script that is run _as if you had typed it in yourself_ whenever you open a shell.

# Details #

When you install RevitPythonShell, your InitScript should be set to something like this:

```
# these commands get executed in the current scope
# of each new shell (but not for canned commands)
import clr
clr.AddReference('RevitAPI')
clr.AddReference('RevitAPIUI')
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
```

This is the right place to add some shortcuts, like the `alert(msg)` definition for quickly displaying a message to the user. Also, you can configure all imports you use and also set up your shell to be where you left of the last time - instantiate objects you will probably be using etc.

**NOTE:** The InitScript is _not_ called for CannedScripts. This is to make CannedScripts more portable.