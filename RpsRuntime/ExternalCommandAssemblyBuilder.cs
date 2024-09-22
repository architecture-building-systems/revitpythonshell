using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Autodesk.Revit.Attributes;

namespace RpsRuntime
{
    /// <summary>
    /// The ExternalCommandAssemblyBuilder creates an assembly (.net dll) for
    /// a list of python scripts that can be used as IExternalCommand implementations
    /// in the Revit user interface (PushButtonData).
    /// </summary>
    public class ExternalCommandAssemblyBuilder
    {
        /// <summary>
        /// Build a new assembly and save it to disk as "pathToDll". Create a type (implementing IExternalCommand) for
        /// each class name in classNamesToScriptPaths that, when "Execute()" is called on it, will load the corresponding python script
        /// from disk and execute it. By deriving from RpsExternalCommandScriptBase, our dynamically generated types only need to
        /// implement a public default constructor that calls the base constructor providing the path to the script.
        /// </summary>
        public void BuildExternalCommandAssembly(string pathToDll, IDictionary<string, string> classNamesToScriptPaths)
        {

            const string fileHeaderTemplate = """
                                    using Autodesk.Revit.Attributes;
                                    using RevitPythonShell.RevitCommands;
                                
                                    #nullable disable
                                    """;
            
            const string classTemplate = """
                                              using Autodesk.Revit.Attributes;
                                              using RevitPythonShell.RevitCommands;

                                              #nullable disable

                                              [Regeneration]
                                              [Transaction]
                                              public class CLASSNAME : CommandLoaderBase
                                              {
                                                public CLASSNAME}()
                                                  : base("SCRIPTPATH")
                                                {
                                                }
                                              }
                                              """;

            StringBuilder sourceCode = new StringBuilder();
            sourceCode.Append(fileHeaderTemplate);
            
            
            foreach (var (className, scriptPath) in classNamesToScriptPaths)
            {
                var classCode = classTemplate.Replace("CLASSNAME", className).Replace("SCRIPTPATH", scriptPath);
                sourceCode.Append(classTemplate);
            }

            DynamicAssemblyCompiler.CompileAndSave(sourceCode.ToString(), pathToDll);
        }
    }
}
