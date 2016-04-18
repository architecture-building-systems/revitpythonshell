# IronPython Startup Script Loader for Autodesk Revit and Vasari®
This is a heavily modified and scaled down version of [RevitPythonShell](https://github.com/architecture-building-systems/revitpythonshell) that will load a script named `__init__.py` at runtime. This project has been created to accompany the [pyRevit](https://github.com/eirannejad/pyRevit) IronPython script library and to provide an independent runtime loader for pyRevit startup script. There are two ways to direct RevitPythonLoader to the startup script:
* define a user environment variable named 'pyRevit' and set the pyRevit scripts folder address as the value, OR
* The startup script must be named `__init__.py` and must be located in the same folder as `RevitPythonLoader.dll`.

Please notice that this addin does not include the python shell from original [RevitPythonShell](https://github.com/architecture-building-systems/revitpythonshell). For that, you still need to install the RPS since it's the best at what it does and I have no intention of creating another one. This addin will resolve [pyRevit](https://github.com/eirannejad/pyRevit)'s dependency on the existence of [RevitPythonShell](https://github.com/architecture-building-systems/revitpythonshell) as a host and make it easier to use in a larger environment with users of varied skill levels that might not have the knowledge to use the  [RevitPythonShell](https://github.com/architecture-building-systems/revitpythonshell) but still want to use the pyRevit tools through the Revit interface.


## License
Similar to its parent project, this project is also licensed under the terms of the [MIT License](https://github.com/eirannejad/revitpythonloader/blob/master/LICENSE.txt).

## Credits

I'd like to thank people listed here for their great contributions:
  * [Daren Thomas](https://github.com/daren-thomas) (original version, maintainer of [RevitPythonShell](https://github.com/architecture-building-systems/revitpythonshell) for creating RPS that this package is heavily relying on)
  * Everyone else who worked on the [RevitPythonShell](https://github.com/architecture-building-systems/revitpythonshell) project and are listed under its credits.

**NOTE**: If you are not on this list, but believe you should be, please contact me!
