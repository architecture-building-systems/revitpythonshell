# Introduction #

RevitPythonShell exposes the RevitAPI in **Autodesk Revit** and **Project Vasari**
to IronPython (python on .NET) scripts.

Daren Thomas from the [Chair for Architecture & Sustainable Building Technologies](http://www.suat.arch.ethz.ch/en)
created the RevitPythonShell as a tool to
facilitate the development of energy analysis tools based on Revit models (see
[DPx - Design Performance Framework](http://www.suat.arch.ethz.ch/en/research/design-performance)).

With the RevitPythonShell, you can explore the Revit API interactively, without
leaving Revit or Vasari. Once your code works, you can save it as a
reusable script, which will show up in the Revit / Vasari ribbon. This removes
the need to restart Revit each time you compile your plugin. It also means you
get to use my favorite scripting language - [Python](http://www.python.org)!

# Features #

  * interactive IronPython interpreter for exploring the API
    * with syntax highlighting
    * autocompletion (press CTRL+SPACE after a period)
    * based on the [IronLab](http://code.google.com/p/ironlab/) project

  * Batteries included! (Python standard library is bundled in the RpsRuntime.dll)

  * IronPython pad (also from the IronLab project)

  * configurable "environment" variables that can be used in your scripts

  * reusable scripts for collecting your awesome scripts

  * [run scripts at Revit startup](http://darenatwork.blogspot.ch/2013/05/new-feature-startupscript-in.html)

  * [deploy scripts as standalone Revit addons](http://darenatwork.blogspot.ch/2013/05/deploying-rps-scripts-with.html)

  * example: the [Design Performance Viewer (DPV)](http://www.suat.arch.ethz.ch/en/research/design-performance)

# Downloads #

Download the appropriate installer, install, restart Revit / Vasari and get going!

  * [(r223) Installer for Autodesk Revit 2015  (\*experimental\*) ](http://sustain.arch.ethz.ch/DPV/Setup_RevitPythonShell_2015_r223.exe)
  * [Installer for Autodesk Revit 2015](http://sustain.arch.ethz.ch/DPV/Setup_RevitPythonShell_2015.exe)
  * [Installer for Autodesk Revit 2014](http://sustain.arch.ethz.ch/DPV/Setup_RevitPythonShell_2014.exe)
  * [Installer for Autodesk Revit 2013](http://sustain.arch.ethz.ch/DPV/Setup_RevitPythonShell_2013_r159.msi)
  * [Installer for Autodesk Vasari (Beta 3)](http://sustain.arch.ethz.ch/DPV/Setup_RevitPythonShell_Vasari_Beta3_r224.exe)


# Getting started #

Learn some python:

  * [The Python Tutorial](https://docs.python.org/2/tutorial/)
  * [Dive Into Python](http://www.diveintopython.net/)

Learn about the Revit API:

  * [Autodesk Developer Network](http://usa.autodesk.com/adsk/servlet/index?siteID=123112&id=2484975)
  * [Jeremy Tammiks blog "The Building Coder"](http://thebuildingcoder.typepad.com/)

You can find sample scripts here:

  * [Sample RPS Scripts on GitHub](https://github.com/daren-thomas/rps-sample-scripts)
    * feel free to send me your own scripts for inclusion!
  * [Nathan's Revit API Notebook using the RevitPythonShell](http://wiki.theprovingground.org/revit-api)
    * Nathan Miller even has a [Mobius Surface for Vasari](http://wiki.theprovingground.org/revit-api-py-parametric) sample
  * [dp stuff (Python Scripts Archives)](http://dp-stuff.org/category/python-scripts)
    * lots of scripts
  * [my own blog](http://darenatwork.blogspot.com/) contains the odd sample scripts

And ask questions / find answers here:

  * [RevitPythonShell group](http://groups.google.com/group/RevitPythonShell)
  * [stackoverflow](http://stackoverflow.com) (Note: use the ```revit```, ``revit-api`` and ``revitpythonshell`` tags)

# Get Involved #

Tell me about stuff you'd like to see in upcoming releases on the
[RevitPythonShell group](http://groups.google.com/group/RevitPythonShell).

Add bugs / issues to the [Issues page](http://code.google.com/p/revitpythonshell/issues/list).

[Download the source](http://code.google.com/p/revitpythonshell/source/checkout),
fix it and tell me about it. I'll try to work with you to get it into
trunk. Thank you.

# License #

This project is licensed under the terms of the [MIT License](http://opensource.org/licenses/MIT).

# Credits #

  * Daren Thomas (original version, maintainer)
  * Zachary Kron (Port to Vasari)
  * Akimitsu Hogge (Port to Vasari)
  * Joe Moorhouse (interactive shell was taken from his project [IronLab](http://ironlab.net/))
  * Jason Schaeffer (Port to Revit 2011)

Also, many thanks to the
[Assistant Chair for Sustainable Architecture and Building Technologies (SuAT)](http://suat.arch.ethz.ch)
for making this project possible.

NOTE: If you are not on this list, but believe you should be, please contact me!
