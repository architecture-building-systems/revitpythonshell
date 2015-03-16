# Introduction #

This page provides a list of variables supplied by RevitPythonShell. These are injected into the scope, whenever an InteractiveShell or a CannedCommand is created. Use these variables to interact with Autodesk Revit.

# Variables for host integration #

| variable | description |
|:---------|:------------|
| `__revit__` | a reference to the `Autodesk.Revit.Application` instance, obtained from the `ExternalCommandData` argument passed to plugins. |
| `__commandData__` | the actual `ExternalCommandData` argument passed to the RevitPythonShell plugin when you clicked "Open Python Shell" |
| `__message__` | contains the contents of the `message` parameter passed to the RevitPythonShell plugin when you clicked "Open Python Shell". On closing the InteractiveShell window, the contents of `__message__` will be assigned back, so Revit has access to it |
| `__elements__` | the `ElementSet` passed to the RevitPythonShell plugin when you clicked "Open Python Shell". |
| `__result__` | This is set to `IExternalCommand.Result.Succeeded`, but you can change it if you want. When the InteractiveShell is closed, the RevitPythonShell returns the contents of this variable as the result of the `IExternalCommand.Execute` method. |
| `__vars__` | a dictionary (string -> string) of user defined variables defined in the xml file `RevitPythonShell.xml` |

# User defined variables #

You can configure the contents of the `__vars__` variable in the RevitPythonShellXml file.

```
<?xml version="1.0" encoding="utf-8" ?>
<RevitPythonShell>
   <Variables>
    <!-- a list of string variables to add to the scope -->
    <StringVariable name="test" value="hello, world"/>
    <StringVariable name="another" value="I'm another value"/>
  </Variables>
```

These strings can then be used in scripts by indexing the `__vars__` variable:

```
print __vars__['test'] # prints "hello, world"
```