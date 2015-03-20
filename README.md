# RevitPythonShell

The RevitPythonShell adds an IronPython interpreter to Autodesk Revit. 

This lets you to write plugins for Revit in Python, my favourite scripting language! But even better, it provide you with an
interactive shell that lets you see the results of your code *as you type it*. This is great for exploring the Revit API while 
writing your Revit Addins - use this in combination with the (RevitLookup)[https://github.com/jeremytammik/RevitLookup] to 
become a Revit API Ninja :)

## Features

- interactive IronPython interpreter for exploring the API
  - with syntax highlighting
  - autocompletion (press CTRL+SPACE after a period)
  - based on the [IronLab](http://code.google.com/p/ironlab/) project
- Batteries included! (Python standard library is bundled as a resource in the `RpsRuntime.dll`)
- IronPython pad (also from the IronLab project)
- configurable "environment" variables that can be used in your scripts
- reusable scripts for collecting your awesome scripts
- run scripts at Revit startup
- deploy scripts as standalone Revit Addins

## Installation

## Contribute

## Support

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



