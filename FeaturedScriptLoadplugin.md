# Introduction #

Jeremy Tammik mentions a solution by John Morse for [reloading an addin to debug](http://thebuildingcoder.typepad.com/blog/2010/03/reload-an-addin-to-debug.html). That solution requires a plugin to do some bridge work and basically just loads the assembly as a byte array.

Using RevitPythonShell and the script `loadplugin.py`, you can do that without installing a separate plugin - and get python scripting functionality for your assembly at the same time!

This script is slightly better than John Morse's version, since it also registers for assembly resolution - this is necessary for multi-assembly-projects.

Additionally (as of 12 march 2010), debugging symbols are loaded from the PDB file if present.

# Details #

Save the script (see below) into your search path. Your search path is defined in the `RevitPythonShell.xml` file found in the installation directory:

```
<?xml version="1.0" encoding="UTF-8"?>
<RevitPythonShell>
  <SearchPaths>
    <!-- a list of paths to add to the engines search path -->
    <SearchPath name="C:\Python25\Lib"/>
    <SearchPath name="C:\RevitPythonShell"/>
  </SearchPaths>
  <!-- ... other stuff ... -->
</RevitPythonShell>
```

You can then import `loadplugin.py` in your scripts / commands or in the interactive shell:

```
>>>import loadplugin
>>># use actual path here... (if you don't already have RvtMgdDbg.dll installed, you probably want to go and do so right now - check ADN!)
>>>assembly = loadplugin.loadAssembly(r'C:\...\Visual Studio 2008\Projects\RvtMgdDbg\bin\RvtMgdDbg.dll')
>>># result is an assembly object
>>>assembly

<Assembly RvtMgdDbg, Version=1.0.3671.26458, Culture=neutral, PublicKeyToken=null>

>>># pretty cool: IronPython lets you use the assembly like a namespace
>>># also note the use of the special predefined variables for executing plugins!
>>>assembly.RvtMgdDbg.CmdSnoopDb().Execute(__commandData__, __message__, __elements__)

(<Autodesk.Revit.IExternalCommand+Result object at 0x000000000000002D [Succeeded]>, '')
>>>
```

# The Script #

```
'''
loadplugin.py

Using the technique described here: 
http://thebuildingcoder.typepad.com/blog/2010/03/reload-an-addin-to-debug.html,
load a revit plugin for scripting.
'''
import clr
clr.AddReference('mscorlib')
clr.AddReference('System')

import System
import System.Reflection

def loadAssembly(path):
    # load the assembly into a byte array
    assemblyBytes = System.IO.File.ReadAllBytes(path)
    pdbPath = path[:-3] + 'pdb'
    if System.IO.File.Exists(pdbPath):
        # load debugging symbols
        pdbBytes = System.IO.File.ReadAllBytes(pdbPath)
        assembly = System.Reflection.Assembly.Load(assemblyBytes, pdbBytes)
    else:
        # no debugging symbols found
        assembly = System.Reflection.Assembly.Load(assemblyBytes)
    # make sure we can resolve assemblies from that directory
    folder = System.IO.Path.GetDirectoryName(path)
    System.AppDomain.CurrentDomain.AssemblyResolve += resolveAssemblyGenerator(folder)
    return assembly

def resolveAssemblyGenerator(folder):
    '''
    returns a function that loads assemblies when needed from the folder 'folder'.
    '''
    def result(sender, args):
        name = args.Name.split(',')[0]
        try:            
            path = System.IO.Path.Combine(folder, name + '.dll')
            if not System.IO.File.Exists(path):
                return None
            return loadAssembly(path)
        except:
            import traceback
            traceback.print_exc()
            return None
    return result
```

# Changelog #

(dates are in format dd.mm.yyyy)

  * 12.03.2010 added loading of debugging symbols if present
  * 11.03.2010 Fixed a bug in `resolveAssemblyGenerator` (s/System.IO.Path.Exists/System.IO.**File**.Exists)