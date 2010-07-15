using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Xml.Linq;
using Autodesk.Revit;
using Autodesk.Revit.UI;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;

namespace RevitPythonShell
{
    [Regeneration(RegenerationOption.Manual)]
    [Transaction(TransactionMode.Manual)]
    class RevitPythonShellApplication : IExternalApplication
    {
        /// <summary>
        /// Hook into Revit to allow starting a command.
        /// </summary>
        Result IExternalApplication.OnStartup(UIControlledApplication application)
        {
            RibbonPanel ribbonPanel = application.CreateRibbonPanel("RevitPythonShell");
            ribbonPanel.AddStackedItems(
                new PushButtonData("RevitPythonShell", "Open Python Shell", typeof(RevitPythonShellApplication).Assembly.Location, "RevitPythonShell.StartShellCommand"),
                new PushButtonData("Configure", "Configure...", typeof(RevitPythonShellApplication).Assembly.Location, "RevitPythonShell.ConfigureCommand"));

            var dllfolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "RevitPythonShell");
            var dllname = "CommandLoaderAssembly.dll";
            var dllfullpath = Path.Combine(dllfolder, dllname);

            // add canned commands as stacked pushbuttons (try to pack 3 commands per pushbutton, then 2)
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

                ribbonPanel.AddStackedItems(
                    new PushButtonData(command0.Name, command0.Name, dllfullpath, "Command" + command0.Index),
                    new PushButtonData(command1.Name, command1.Name, dllfullpath, "Command" + command1.Index),
                    new PushButtonData(command2.Name, command2.Name, dllfullpath, "Command" + command2.Index));
            }
            if (commands.Count == 4)
            {
                // remove first two commands from the list
                var command0 = commands[0];
                var command1 = commands[1];
                commands.RemoveAt(0);
                commands.RemoveAt(0);
                ribbonPanel.AddStackedItems(
                    new PushButtonData(command0.Name, command0.Name, dllfullpath, "Command" + command0.Index),
                    new PushButtonData(command1.Name, command1.Name, dllfullpath, "Command" + command1.Index));
            }
            if (commands.Count == 2)
            {
                // remove first two commands from the list
                var command0 = commands[0];
                var command1 = commands[1];
                commands.RemoveAt(0);
                commands.RemoveAt(0);
                ribbonPanel.AddStackedItems(
                    new PushButtonData(command0.Name, command0.Name, dllfullpath, "Command" + command0.Index),
                    new PushButtonData(command1.Name, command1.Name, dllfullpath, "Command" + command1.Index));
            }
            if (commands.Count == 1)
            {
                // only one command defined, show as a big button...
                var command = commands[0];
                ribbonPanel.AddItem(
                    new PushButtonData(command.Name, command.Name, dllfullpath, "Command" + command.Index));
            }
            CreateCommandLoaderAssembly(dllfolder, dllname);
            return Result.Succeeded;
        }

        /// <summary>
        /// Creates a dynamic assembly that contains types for starting the canned commands.
        /// </summary>
        private static void CreateCommandLoaderAssembly(string dllfolder, string dllname)
        {
            var assemblyName = new AssemblyName { Name = "CommandLoaderAssembly", Version = new Version(1, 0, 0, 0) };
            var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave, dllfolder);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule("CommandLoaderModule", dllname);

            foreach (var command in GetCommands())
            {
                var typebuilder = moduleBuilder.DefineType("Command" + command.Index,
                                                        TypeAttributes.Class | TypeAttributes.Public,
                                                        typeof(CommandLoaderBase));

                // add RegenerationAttribute to type
                var regenerationConstrutorInfo = typeof(RegenerationAttribute).GetConstructor(new Type[] { typeof(RegenerationOption) });                
                var regenerationAttributeBuilder = new CustomAttributeBuilder(regenerationConstrutorInfo, new object[] {RegenerationOption.Manual});
                typebuilder.SetCustomAttribute(regenerationAttributeBuilder);

                // add TransactionAttribute to type
                var transactionConstructorInfo = typeof(TransactionAttribute).GetConstructor(new Type[] { typeof(TransactionMode) });
                var transactionAttributeBuilder = new CustomAttributeBuilder(transactionConstructorInfo, new object[] { TransactionMode.Manual });
                typebuilder.SetCustomAttribute(transactionAttributeBuilder);

                // call base constructor with script path
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
            assemblyBuilder.Save(dllname);
        }

        Result IExternalApplication.OnShutdown(UIControlledApplication application)
        {
            // FIXME: deallocate the python shell...
            return Result.Succeeded;
        }

        /// <summary>
        /// Returns a handle to the settings file.
        /// </summary>
        /// <returns></returns>
        public static XDocument GetSettings()
        {
            string settingsFile = GetSettingsFile();
            return XDocument.Load(settingsFile);
        }

        private static string GetSettingsFile()
        {
            string folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "RevitPythonShell");
            return Path.Combine(folder, "RevitPythonShell.xml");
        }      

        /// <summary>
        /// Returns a list of commands as defined in the settings file.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Command> GetCommands()
        {
            int i = 0;
            foreach (var commandNode in GetSettings().Root.Descendants("Command") ?? new List<XElement>())
            {
                var commandName = commandNode.Attribute("name").Value;
                var commandSrc = commandNode.Attribute("src").Value;

                yield return new Command { Name = commandName, Source = commandSrc, Index = i++ };                
            }
        }

        /// <summary>
        /// Saves a list of Command objects to the settings file, replacing the old commands.
        /// </summary>
        public static void SetCommands(
            IEnumerable<Command> commands, IEnumerable<string> searchPaths, IEnumerable<KeyValuePair<string, string>> variables)
        {
            var doc = GetSettings();

            // clean out current stuff
            foreach (var xmlExistingCommands in (doc.Root.Descendants("Commands") ?? new List<XElement>()).ToList())
            {
                xmlExistingCommands.Remove();
            }
            foreach (var xmlExistingSearchPaths in doc.Root.Descendants("SearchPaths").ToList())
            {
                xmlExistingSearchPaths.Remove();
            }
            foreach (var xmlExistingVariables in doc.Root.Descendants("Variables").ToList())
            {
                xmlExistingVariables.Remove();
            }

            // add commnads
            var xmlCommands = new XElement("Commands");
            foreach (var command in commands)
            {
                xmlCommands.Add(new XElement(
                    "Command", 
                        new XAttribute("name", command.Name), 
                        new XAttribute("src", command.Source)));

            }
            doc.Root.Add(xmlCommands);

            // add search paths
            var xmlSearchPaths = new XElement("SearchPaths");
            foreach (var path in searchPaths)
            {
                xmlSearchPaths.Add(new XElement(
                    "SearchPath",
                        new XAttribute("name", path)));

            }
            doc.Root.Add(xmlSearchPaths);

            // add variables
            var xmlVariables = new XElement("Variables");
            foreach (var variable in variables)
            {
                xmlVariables.Add(new XElement(
                    "StringVariable",
                        new XAttribute("name", variable.Key),
                        new XAttribute("value", variable.Value)));

            }
            doc.Root.Add(xmlVariables);

            doc.Save(GetSettingsFile());
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
    internal class Command
    {
        public string Name;
        public string Source;
        public int Index;

        public override string ToString()
        {
            return Name;
        }
    }
}
