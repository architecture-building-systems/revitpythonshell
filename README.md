# RevitPythonShell

The RevitPythonShell adds an IronPython interpreter to Autodesk Revit and Vasari. 

The RevitPythonShell (RPS) lets you to write plugins for Revit in Python, my favourite scripting language! But even better, it provides you with an
interactive shell that lets you see the results of your code *as you type it*. This is great for exploring the Revit API while 
writing your Revit Addins - use this in combination with the [RevitLookup](https://github.com/jeremytammik/RevitLookup) database exploration tool to become a Revit API Ninja :)

## Features

- interactive IronPython interpreter for exploring the API
  - with syntax highlighting
  - autocompletion (press CTRL+SPACE after a period)
  - based on the [IronLab](http://code.google.com/p/ironlab/) project
- batteries included! (Python standard library is bundled as a resource in the `RpsRuntime.dll`)
- full access to the .NET framework and the Revit API
- configurable "environment" variables that can be used in your scripts
- save "external scripts" for reuse and start collecting your awesome hacks!
- run scripts at Revit startup
- deploy scripts as standalone Revit Addins

## Installation

- [Installer for Autodesk Revit 2016](https://github.com/architecture-building-systems/revitpythonshell/releases/download/2015.04.29/2015.04.29_Setup_RevitPythonShell_2016.exe)
- [Installer for Autodesk Revit 2015](https://github.com/architecture-building-systems/revitpythonshell/releases/download/2015.04.29/2015.04.29_Setup_RevitPythonShell_2015.exe)

Older versions:
- [Release 2015.03.20](Revit 2014, 2015 & Vasari Beta 3)](https://github.com/architecture-building-systems/revitpythonshell/releases/tag/2015.03.20)
- [Installer for Autodesk Revit 2015](http://sustain.arch.ethz.ch/DPV/Setup_RevitPythonShell_2015.exe)
- [Installer for Autodesk Revit 2014](http://sustain.arch.ethz.ch/DPV/Setup_RevitPythonShell_2014.exe)
- [Installer for Autodesk Revit 2013](http://sustain.arch.ethz.ch/DPV/Setup_RevitPythonShell_2013_r159.msi)
- [Installer for Autodesk Vasari (Beta 3)](http://sustain.arch.ethz.ch/DPV/Setup_RevitPythonShell_Vasari_Beta3_r224.exe)

## Contribute

- Issue Tracker: https://github.com/architecture-building-systems/revitpythonshell/issues
- Source Code: https://github.com/architecture-building-systems/revitpythonshell

## Support

- [RevitPythonShell discussion group](http://groups.google.com/group/RevitPythonShell)
- [stackoverflow](http://stackoverflow.com) (Note: use the ```revit```, ``revit-api`` and ``revitpythonshell`` tags)

## Getting started:

Learn some python:

  * [The Python Tutorial](https://docs.python.org/2/tutorial/)
  * [Dive Into Python](http://www.diveintopython.net/)

Learn about the Revit API:

  * [Autodesk Developer Network](T)
  * [Jeremy Tammiks blog "The Building Coder"](http://thebuildingcoder.typepad.com/)

You can find sample scripts here:

  * [Sample RPS Scripts on GitHub](https://github.com/daren-thomas/rps-sample-scripts)
    * feel free to send me your own scripts for inclusion!
  * [Nathan's Revit API Notebook using the RevitPythonShell](http://wiki.theprovingground.org/revit-api)
    * Nathan Miller even has a [Mobius Surface for Vasari](http://wiki.theprovingground.org/revit-api-py-parametric) sample
  * [dp stuff (Python Scripts Archives)](http://dp-stuff.org/category/python-scripts)
    * lots of scripts
  * [my own blog](http://darenatwork.blogspot.com/) contains the odd sample script

## License

This project is licensed under the terms of the [MIT License](http://opensource.org/licenses/MIT).

## Credits

  * Daren Thomas (original version, maintainer)
  * Zachary Kron (original port to Vasari)
  * Akimitsu Hogge (original port to Vasari)
  * Joe Moorhouse (interactive shell was taken from his project [IronLab](http://ironlab.net/))
  * Jason Schaeffer (port to Revit 2011)
  * many, many users with questions, bug reports etc!

Also, many thanks to the
[Chair for Architecture & Building Systems](http://systems.arch.ethz.ch) for making this project possible.

**NOTE**: If you are not on this list, but believe you should be, please contact me!



