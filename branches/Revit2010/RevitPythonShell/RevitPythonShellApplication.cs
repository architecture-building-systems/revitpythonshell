using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Xml.Linq;
using Autodesk.Revit;

namespace RevitPythonShell
{
    class RevitPythonShellApplication: IExternalApplication
    {
        /// <summary>
        /// Hook into Revit to allow starting a command.
        /// </summary>
        IExternalApplication.Result IExternalApplication.OnStartup(ControlledApplication application)
        {
            RibbonPanel ribbonPanel = application.CreateRibbonPanel("RevitPythonShell");
            ribbonPanel.AddPushButton("RevitPythonShell", "Open Python Shell",
                                      typeof (RevitPythonShellApplication).Assembly.Location,
                                      "RevitPythonShell.StartShellCommand");
         
            // add canned commands as stacked pushbuttons
            var commands = GetCommands().ToList();
            while (commands.Count > 4 || commands.Count == 3)
            {
                // remove first three commands from the list
                var command0 = commands[0];
                var command1 = commands[1];
                var command2 = commands[2];
                commands.RemoveAt(0);
                commands.RemoveAt(0);
                commands.RemoveAt(0);
                ribbonPanel.AddStackedButtons(
                    new PushButtonData(command0.Name, command0.Name, "CommandLoaderAssembly.dll", "Command" + command0.Index),
                    new PushButtonData(command1.Name, command1.Name, "CommandLoaderAssembly.dll", "Command" + command1.Index),
                    new PushButtonData(command2.Name, command2.Name, "CommandLoaderAssembly.dll", "Command" + command2.Index));
            }
            if (commands.Count == 4)
            {
                // remove first two commands from the list
                var command0 = commands[0];
                var command1 = commands[1];
                commands.RemoveAt(0);
                commands.RemoveAt(0);
                ribbonPanel.AddStackedButtons(
                    new PushButtonData(command0.Name, command0.Name, "CommandLoaderAssembly.dll", "Command" + command0.Index),
                    new PushButtonData(command1.Name, command1.Name, "CommandLoaderAssembly.dll", "Command" + command1.Index));
            }
            if (commands.Count == 2)
            {
                // remove first two commands from the list
                var command0 = commands[0];
                var command1 = commands[1];
                commands.RemoveAt(0);
                commands.RemoveAt(0);
                ribbonPanel.AddStackedButtons(
                    new PushButtonData(command0.Name, command0.Name, "CommandLoaderAssembly.dll", "Command" + command0.Index),
                    new PushButtonData(command1.Name, command1.Name, "CommandLoaderAssembly.dll", "Command" + command1.Index));
            }
            CreateCommandLoaderAssembly();
            return IExternalApplication.Result.Succeeded;
        }

        /// <summary>
        /// Creates a dynamic assembly that contains types for starting the canned commands.
        /// </summary>
        private static void CreateCommandLoaderAssembly()
        {
            var assemblyName = new AssemblyName {Name = "CommandLoaderAssembly", Version = new Version(1, 0, 0, 0)};
            var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule("CommandLoaderModule", "CommandLoaderAssembly.dll");

            foreach (var command in GetCommands())
            {
                var typebuilder = moduleBuilder.DefineType("Command" + command.Index,
                                                        TypeAttributes.Class | TypeAttributes.Public,
                                                        typeof(CommandLoaderBase));

                var ci = typeof(CommandLoaderBase).GetConstructor(new[] { typeof(string) });

                var constructorBuilder = typebuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new Type[0]);
                var gen = constructorBuilder.GetILGenerator();
                gen.Emit(OpCodes.Ldarg_0);                // Load "this" onto eval stack
                gen.Emit(OpCodes.Ldstr, command.Source);  // Load the path to the command as a string onto stack
                gen.Emit(OpCodes.Call, ci);               // call base constructor (consumes "this" and the string)
                gen.Emit(OpCodes.Nop);                    // Fill some space - this is how it is generated for equivalent C# code
                gen.Emit(OpCodes.Nop);
                gen.Emit(OpCodes.Nop);
                gen.Emit(OpCodes.Ret);                    // return from constructor
                typebuilder.CreateType();
            }
            assemblyBuilder.Save("CommandLoaderAssembly.dll");            
        }

        IExternalApplication.Result IExternalApplication.OnShutdown(ControlledApplication application)
        {
            // FIXME: deallocate the python shell...
            return IExternalApplication.Result.Succeeded;
        }

        /// <summary>
        /// Returns a handle to the settings file.
        /// </summary>
        /// <returns></returns>
        public static XDocument GetSettings()
        {
            string assemblyFolder = new FileInfo(typeof(RevitPythonShellApplication).Assembly.Location ?? ".").DirectoryName ?? ".";
            string settingsFile = Path.Combine(assemblyFolder, "RevitPythonShell.xml");
            return XDocument.Load(settingsFile);
        }

        /// <summary>
        /// Returns a list of commands as defined in the settings file.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Command> GetCommands()
        {
            int i = 0;
            foreach (var commandNode in GetSettings().Root.Descendants("Command"))
            {
                var commandName = commandNode.Attribute("name").Value;
                var commandSrc = commandNode.Attribute("src").Value;

                yield return new Command {Name = commandName, Source = commandSrc, Index = i++};
            }
        }

        /// <summary>
        /// Returns the list of variables to be included with the scope in RevitPythonShell scripts.
        /// </summary>
        /// <returns></returns>
        public static IDictionary<string, string> GetVariables()
        {
            return GetSettings().Root.Descendants("StringVariable").ToDictionary(v => v.Attribute("name").Value,
                                                                                  v => v.Attribute("value").Value);
        }

        /// <summary>
        /// Returns a list of search paths to be added to python interpreter engines.
        /// </summary>
        public static IEnumerable<string> GetSearchPaths()
        {
            foreach (var searchPathNode in GetSettings().Root.Descendants("SearchPath"))
            {
                yield return searchPathNode.Attribute("name").Value;
            }
        }
    }

    /// <summary>
    /// A simple structure to hold information about canned commands.
    /// </summary>
    internal struct Command
    {
        public string Name;
        public string Source;
        public int Index;
    }
}
