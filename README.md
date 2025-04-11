# RevitPythonShell
![Revit API](https://img.shields.io/badge/Revit%20API%202024-blue.svg) ![Revit API](https://img.shields.io/badge/Revit%20API%202025-blue.svg) ![Revit API](https://img.shields.io/badge/Revit%20API%202026-blue.svg) ![Platform](https://img.shields.io/badge/platform-Windows-lightgray.svg) [![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

![ReSharper](https://img.shields.io/badge/ReSharper-2021.3.3-yellow) ![Rider](https://img.shields.io/badge/Rider-2021.3.3-yellow) ![Visual Studio 2022](https://img.shields.io/badge/Visual_Studio_2022_Preview_2.0-17.1.0-yellow) ![.NET Framework](https://img.shields.io/badge/.NET_6.0-yellow)

[![Publish](../../actions/workflows/Workflow.yml/badge.svg)](../../actions)
[![Github All Releases](https://img.shields.io/github/downloads/architecture-building-systems/revitpythonshell/total?color=blue&label=Download)]()
[![HitCount](https://hits.dwyl.com/architecture-building-systems/revitpythonshell.svg?style=flat-square)](http://hits.dwyl.com/architecture-building-systems/revitpythonshell)

The RevitPythonShell adds an IronPython interpreter to Autodesk Revit and Vasari.

The RevitPythonShell (RPS) lets you to write plugins for Revit in Python, my favourite scripting language! But even
better, it provides you with an
interactive shell that lets you see the results of your code *as you type it*. This is great for exploring the Revit API
while
writing your Revit Addins - use this in combination with the [RevitLookup](https://github.com/jeremytammik/RevitLookup)
database exploration tool to become a Revit API Ninja :)

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
- `lookup()` function for snooping `Element`, `ElementSet` and `ElementId` objects
  in [RevitLookup](https://github.com/jeremytammik/RevitLookup)

## IronPython 3

IronPython 3.4 uses Python 3.4 syntax and standard libraries and so your Python code will need to be updated accordingly. There are numerous tools and guides available on the web to help porting from Python 2 to 3.

IronPython 3 targets Python 3, including the re-organized standard library, Unicode strings, and all of the other new features.with user upgrade from **IronPython 2** to **IronPython 3**, please follow [Upgrade from IronPython 2 to IronPython 3](https://github.com/IronLanguages/ironpython3/blob/master/Documentation/upgrading-from-ipy2.md).

Various differences between IronPython and CPython can follow at [Differences IronPython and CPython](https://github.com/IronLanguages/ironpython3/blob/master/Documentation/differences-from-c-python.md).

## Installation

Please follow last release at section [Release](https://github.com/architecture-building-systems/revitpythonshell/releases/latest) support version Support From Revit 2018-2026.

Older versions:
- [Installer for Autodesk Revit 2022](https://github.com/architecture-building-systems/revitpythonshell/releases/download/2021.06.20/2021.06.20_Setup_RevitPythonShell_2022.exe)
- [Installer for Autodesk Revit 2021](https://github.com/architecture-building-systems/revitpythonshell/releases/download/2021.03.22/2021.03.22_Setup_RevitPythonShell_2021.exe)
- [Installer for Autodesk Revit 2020](https://github.com/architecture-building-systems/revitpythonshell/releases/download/2019.01.27/2020.01.19_Setup_RevitPythonShell_2020.exe)
- [Installer for Autodesk Revit 2019](https://github.com/architecture-building-systems/revitpythonshell/releases/download/2018.09.19/2018.09.19_Setup_RevitPythonShell_2019.exe)
- [Installer for Autodesk Revit 2018 (and 2018.1)](https://github.com/architecture-building-systems/revitpythonshell/releases/download/2017.07.24/2017.07.24_Setup_RevitPythonShell_2018.exe)
- [Installer for Autodesk Revit 2017](https://github.com/architecture-building-systems/revitpythonshell/releases/download/2017.04.06/2017.04.06_Setup_RevitPythonShell_2017.exe)
- [Installer for Autodesk Revit 2016](https://github.com/architecture-building-systems/revitpythonshell/releases/download/2017.03.07/2017.03.07_Setup_RevitPythonShell_2016.exe)
- [Installer for Autodesk Revit 2015](https://github.com/architecture-building-systems/revitpythonshell/releases/download/2017.03.07/2017.03.07_Setup_RevitPythonShell_2015.exe)
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
- read up on [How To Ask Questions The Smart Way](http://www.catb.org/esr/faqs/smart-questions.html) first

## Getting started:

Learn some python:

* [The Python Tutorial](https://docs.python.org/2/tutorial/)
* [Dive Into Python](http://www.diveintopython.net/)

Learn about the Revit API:

* [Autodesk Developer Network](https://www.autodesk.com/developer-network/open)
* [Jeremy Tammiks blog "The Building Coder"](http://thebuildingcoder.typepad.com/)

Tutorials recommended by the community:

* [Mono IronPython Winforms Tutorial](http://zetcode.com/tutorials/ironpythontutorial/) - recommended by Callum

You can find sample scripts here:

* [Sample RPS Scripts on GitHub](https://github.com/daren-thomas/rps-sample-scripts)
    * feel free to send me your own scripts for inclusion!
* [Nathan's Revit API Notebook using the RevitPythonShell](http://wiki.theprovingground.org/revit-api)
    * Nathan Miller even has a [Mobius Surface for Vasari](http://wiki.theprovingground.org/revit-api-py-parametric)
      sample
* [dp stuff (Python Scripts Archives)](http://dp-stuff.org/category/python-scripts)
    * lots of scripts
* [my own blog](http://darenatwork.blogspot.com/) contains the odd sample script
* [Check out pyRevit](http://www.pyrevitlabs.io) by Ehsan Iran-Nejad - it includes a library of
  interesting scripts and some additions to make writing your own easier!

## License

This project is licensed under the terms of the [MIT License](http://opensource.org/licenses/MIT).

## Credits

* Daren Thomas (original version, maintainer)
* Zachary Kron (original port to Vasari)
* Akimitsu Hogge (original port to Vasari)
* Joe Moorhouse (interactive shell was taken from his project [IronLab](http://ironlab.net/))
* Jason Schaeffer (port to Revit 2011)
* [Ehsan Iran-Nejad (@eirannejad)](https://github.com/eirannejad) countless improvements, the
  awesome [pyRevit](https://github.com/eirannejad/pyRevit) tool and a special thanks for helping maintain RPS!
* [@DanRumery](https://github.com/danrumery) improved autocompletion with PR #59
* [Petr Mitev (@mitevpi)](https://github.com/mitevpi) ported to Revit 2019 with RP #86
* [Alvaro Ortega Pickmans (@alvpickmans)](https://github.com/alvpickmans) refactor to sdk csproject and release for
  Revit 2020 PR #101
* [@hdm-dt-fb](https://github.com/hdm-dt-fb) added `set_font_sizes` function for presenting
  RPS ([PR #77](https://github.com/architecture-building-systems/revitpythonshell/pull/77))
* [Nice3Point](https://github.com/Nice3point) for process CI/CD
* [Chuong Mep](https://github.com/chuongmep/) a people like maintain for project open source.
* [Roman Golev](https://github.com/romangolev/) (port to Revit 2025)
* [Jean-Marc Couffin](https://github.com/jmcouffin/) (port to Revit 2026)
* many, many users with questions, bug reports etc!

Also, many thanks to the
[Chair for Architecture & Building Systems](http://systems.arch.ethz.ch) for making this project possible.

**NOTE**: If you are not on this list, but believe you should be, please contact me!



