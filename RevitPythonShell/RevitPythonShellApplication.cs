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
using System.Windows.Media;
using System.Windows.Media.Imaging;

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

            var dllfolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "RevitPythonShell");
            var dllname = "CommandLoaderAssembly.dll";
            var dllfullpath = Path.Combine(dllfolder, dllname);

            BuildRibbonPanel(application, dllfullpath);
            CreateCommandLoaderAssembly(dllfolder, dllname);
            return Result.Succeeded;
        }

        private static void BuildRibbonPanel(UIControlledApplication application, string dllfullpath)
        {
            var assembly = typeof(RevitPythonShellApplication).Assembly;
            //var largeImage = GetEmbeddedPng(assembly, "RevitPythonShell.Resources.PythonConsole32x32.png");
            var largeImage = GetEmbeddedPng(assembly, "RevitPythonShell.Resources.PythonConsole36x36.png");
            var smallImage = GetEmbeddedBmp(assembly, "RevitPythonShell.Resources.PythonConsole16x16.bmp");

            RibbonPanel ribbonPanel = application.CreateRibbonPanel("RevitPythonShell");
            var splitButton = ribbonPanel.AddItem(new SplitButtonData("splitButtonRevitPythonShell", "RevitPythonShell")) as SplitButton;

            PushButtonData pbdOpenPythonShell = new PushButtonData(
                            "RevitPythonShell", 
                            "Interactive Python Shell", 
                            assembly.Location, 
                            "RevitPythonShell.IronPythonConsoleCommand");
            pbdOpenPythonShell.Image = smallImage;
            pbdOpenPythonShell.LargeImage = largeImage;
            splitButton.AddPushButton(pbdOpenPythonShell);


            PushButtonData pbdConfigure = new PushButtonData(
                            "Configure", 
                            "Configure...", 
                            assembly.Location, 
                            "RevitPythonShell.ConfigureCommand");
            pbdConfigure.Image = smallImage;
            pbdConfigure.LargeImage = largeImage;
            splitButton.AddPushButton(pbdConfigure);

            var commands = GetCommands().ToList();
            AddGroupedCommands(dllfullpath, ribbonPanel, commands.Where(c => c.Group != null).GroupBy(c => c.Group));
            AddUngroupedCommands(dllfullpath, ribbonPanel, commands.Where(c => c.Group == null).ToList());
        }

        private static ImageSource GetEmbeddedBmp(System.Reflection.Assembly app, string imageName)
        {
            var file = app.GetManifestResourceStream(imageName);
            var source = BmpBitmapDecoder.Create(file, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
            return source.Frames[0];
        }

        private static ImageSource GetEmbeddedPng(System.Reflection.Assembly app, string imageName)
        {
            var file = app.GetManifestResourceStream(imageName);
            var source = PngBitmapDecoder.Create(file, BitmapCreateOptions.None, BitmapCacheOption.None);
            return source.Frames[0];
        }

        private static void AddGroupedCommands(string dllfullpath, RibbonPanel ribbonPanel, IEnumerable<IGrouping<string, Command>> groupedCommands)
        {
            foreach (var group in groupedCommands)
            {
                SplitButtonData splitButtonData = new SplitButtonData(group.Key, group.Key);
                var splitButton = ribbonPanel.AddItem(splitButtonData) as SplitButton;
                foreach (var command in group)
                {
                    splitButton.AddPushButton(new PushButtonData(command.Name, command.Name, dllfullpath, "Command" + command.Index));
                }
            }
        }


        private static void AddUngroupedCommands(string dllfullpath, RibbonPanel ribbonPanel, List<Command> commands)
        {
            // add canned commands as stacked pushbuttons (try to pack 3 commands per pushbutton, then 2)            
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
                var group = commandNode.Attribute("group") == null ? null : commandNode.Attribute("group").Value;
                yield return new Command { Name = commandName, Source = commandSrc, Group = group, Index = i++ };                
            }
        }

        /// <summary>
        /// Returns a string to be executed, whenever the interactive shell is started.
        /// If this is not specified in the XML file (under /RevitPythonShell/InitScript),
        /// then null is returned.
        /// </summary>
        public static string GetInitScript()
        {
            var initScriptTags = GetSettings().Root.Descendants("InitScript") ?? new List<XElement>();
            if (initScriptTags.Count() == 0)
            {
                return null;
            }
            var firstScript = initScriptTags.First();
            return firstScript.Value.Trim();
        }

        /// <summary>
        /// Writes settings to the settings file, replacing the old commands.
        /// </summary>
        public static void WriteSettings(
            IEnumerable<Command> commands, 
            IEnumerable<string> searchPaths, 
            IEnumerable<KeyValuePair<string, string>> variables,
            string initScript)
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
            foreach (var xmlExistingInitScript in doc.Root.Descendants("InitScript").ToList())
            {
                xmlExistingInitScript.Remove();
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

            // add init script
            var xmlInitScript = new XElement("InitScript");
            xmlInitScript.Add(new XCData(initScript));
            doc.Root.Add(xmlInitScript);

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
        public string Group;
        public string Source;
        public int Index;        

        public override string ToString()
        {
            return Name;
        }
    }
}
