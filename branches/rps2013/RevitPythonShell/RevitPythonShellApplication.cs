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
using RevitPythonShell.RpsRuntime;

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

            try
            {
                var dllfolder = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                        "RevitPythonShell2013");
                var assemblyName = "CommandLoaderAssembly";
                var dllfullpath = Path.Combine(dllfolder, assemblyName + ".dll");

                var settings = GetSettings();

                CreateCommandLoaderAssembly(settings, dllfolder, assemblyName);
                BuildRibbonPanel(application, dllfullpath);

                foreach (var repository in GetRepositories())
                {
                    CreateCommandLoaderAssembly(XDocument.Load(repository.Url), dllfolder, repository.SafeName());
                    BuildRepositoryPanel(application, repository, Path.Combine(dllfolder, repository.SafeName() + ".dll"));
                }

                ExecuteStartupScript(application);

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Error setting up RevitPythonShell", ex.ToString());
                return Result.Failed;
            }
        }

        private static void ExecuteStartupScript(UIControlledApplication application)
        {
            // we need a UIApplication object to assign as `__revit__` in python...
            var fi = application.GetType().GetField("m_application", BindingFlags.NonPublic | BindingFlags.Instance);
            var uiApplication = (UIApplication)fi.GetValue(application);            
            // execute StartupScript
            var startupScript = GetStartupScript();
            if (startupScript != null)
            {
                var executor = new ScriptExecutor(GetConfig(), uiApplication);
                var result = executor.ExecuteScript(startupScript);
                if (result == (int)Result.Failed)
                {
                    TaskDialog.Show("RevitPythonShell - StartupScript", executor.Message);
                }
            }
        }

        private static void BuildRepositoryPanel(UIControlledApplication application, Repository repository, string dllfullpath)
        {
            var assembly = typeof(RevitPythonShellApplication).Assembly;
            var largeImage = GetEmbeddedPng(assembly, "RevitPythonShell.Resources.PythonConsole32x32.png");
            var smallImage = GetEmbeddedPng(assembly, "RevitPythonShell.Resources.PythonConsole16x16.png");

            RibbonPanel ribbonPanel = application.CreateRibbonPanel(repository.Name);

            var commands = GetCommands(XDocument.Load(repository.Url)).ToList();
            AddGroupedCommands(dllfullpath, ribbonPanel, commands.Where(c => !string.IsNullOrEmpty(c.Group)).GroupBy(c => c.Group));
            AddUngroupedCommands(dllfullpath, ribbonPanel, commands.Where(c => string.IsNullOrEmpty(c.Group)).ToList());
        }

        private static void BuildRibbonPanel(UIControlledApplication application, string dllfullpath)
        {
            var assembly = typeof(RevitPythonShellApplication).Assembly;
            //var largeImage = GetEmbeddedPng(assembly, "RevitPythonShell.Resources.PythonConsole32x32.png");
            var largeImage = GetEmbeddedPng(assembly, "RevitPythonShell.Resources.PythonConsole32x32.png");
            var smallImage = GetEmbeddedPng(assembly, "RevitPythonShell.Resources.PythonConsole16x16.png");

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

            PushButtonData pbdDeployRpsAddin = new PushButtonData(
                "DeployRpsAddin",
                "Deploy RpsAddin",
                assembly.Location,
                "RevitPythonShell.DeployRpsAddinCommand");
            pbdDeployRpsAddin.Image = smallImage;
            pbdDeployRpsAddin.LargeImage = largeImage;
            splitButton.AddPushButton(pbdDeployRpsAddin);

            var commands = GetCommands(GetSettings()).ToList();
            AddGroupedCommands(dllfullpath, ribbonPanel, commands.Where(c => !string.IsNullOrEmpty(c.Group)).GroupBy(c => c.Group));
            AddUngroupedCommands(dllfullpath, ribbonPanel, commands.Where(c => string.IsNullOrEmpty(c.Group)).ToList());
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
            Debug.WriteLine(source.Frames[0].DpiX);
            Debug.WriteLine(source.Frames[0].DpiY);
            return source.Frames[0];
        }

        private static void AddGroupedCommands(string dllfullpath, RibbonPanel ribbonPanel, IEnumerable<IGrouping<string, Command>> groupedCommands)
        {
            var largeImage = GetEmbeddedPng(typeof(RevitPythonShellApplication).Assembly, "RevitPythonShell.Resources.PythonScript32x32.png");
            var smallImage = GetEmbeddedPng(typeof(RevitPythonShellApplication).Assembly, "RevitPythonShell.Resources.PythonScript16x16.png");

            foreach (var group in groupedCommands)
            {
                SplitButtonData splitButtonData = new SplitButtonData(group.Key, group.Key);
                var splitButton = ribbonPanel.AddItem(splitButtonData) as SplitButton;
                foreach (var command in group)
                {
                    var pbd = new PushButtonData(command.Name, command.Name, dllfullpath, "Command" + command.Index);
                    pbd.Image = smallImage;
                    pbd.LargeImage = largeImage;
                    splitButton.AddPushButton(pbd);
                }
            }
        }


        private static void AddUngroupedCommands(string dllfullpath, RibbonPanel ribbonPanel, List<Command> commands)
        {
            var largeImage = GetEmbeddedPng(typeof(RevitPythonShellApplication).Assembly, "RevitPythonShell.Resources.PythonScript32x32.png");
            var smallImage = GetEmbeddedPng(typeof(RevitPythonShellApplication).Assembly, "RevitPythonShell.Resources.PythonScript16x16.png");

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

                PushButtonData pbdA = new PushButtonData(command0.Name, command0.Name, dllfullpath, "Command" + command0.Index);
                pbdA.Image = smallImage;
                pbdA.LargeImage = largeImage;

                PushButtonData pbdB = new PushButtonData(command1.Name, command1.Name, dllfullpath, "Command" + command1.Index);
                pbdB.Image = smallImage;
                pbdB.LargeImage = largeImage;

                PushButtonData pbdC = new PushButtonData(command2.Name, command2.Name, dllfullpath, "Command" + command2.Index);
                pbdC.Image = smallImage;
                pbdC.LargeImage = largeImage;

                ribbonPanel.AddStackedItems(pbdA, pbdB, pbdC);
            }
            if (commands.Count == 4)
            {
                // remove first two commands from the list
                var command0 = commands[0];
                var command1 = commands[1];
                commands.RemoveAt(0);
                commands.RemoveAt(0);

                PushButtonData pbdA = new PushButtonData(command0.Name, command0.Name, dllfullpath, "Command" + command0.Index);
                pbdA.Image = smallImage;
                pbdA.LargeImage = largeImage;

                PushButtonData pbdB = new PushButtonData(command1.Name, command1.Name, dllfullpath, "Command" + command1.Index);
                pbdB.Image = smallImage;
                pbdB.LargeImage = largeImage;

                ribbonPanel.AddStackedItems(pbdA, pbdB);
            }
            if (commands.Count == 2)
            {
                // remove first two commands from the list
                var command0 = commands[0];
                var command1 = commands[1];
                commands.RemoveAt(0);
                commands.RemoveAt(0);
                PushButtonData pbdA = new PushButtonData(command0.Name, command0.Name, dllfullpath, "Command" + command0.Index);
                pbdA.Image = smallImage;
                pbdA.LargeImage = largeImage;

                PushButtonData pbdB = new PushButtonData(command1.Name, command1.Name, dllfullpath, "Command" + command1.Index);
                pbdB.Image = smallImage;
                pbdB.LargeImage = largeImage;

                ribbonPanel.AddStackedItems(pbdA, pbdB);
            }
            if (commands.Count == 1)
            {
                // only one command defined, show as a big button...
                var command = commands[0];
                PushButtonData pbd = new PushButtonData(command.Name, command.Name, dllfullpath, "Command" + command.Index);
                pbd.Image = smallImage;
                pbd.LargeImage = largeImage;
                ribbonPanel.AddItem(pbd);
            }
        }

        /// <summary>
        /// Creates a dynamic assembly that contains types for starting the canned commands.
        /// </summary>
        private static void CreateCommandLoaderAssembly(XDocument repository, string dllfolder, string dllname)
        {
            var assemblyName = new AssemblyName { Name = dllname + ".dll", Version = new Version(1, 0, 0, 0) };
            var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave, dllfolder);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule("CommandLoaderModule", dllname + ".dll");

            foreach (var command in GetCommands(repository))
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
            assemblyBuilder.Save(dllname + ".dll");
        }

        Result IExternalApplication.OnShutdown(UIControlledApplication application)
        {
            // FIXME: deallocate the python shell...
            return Result.Succeeded;
        }

        private static IRpsConfig _config = null;
        public static IRpsConfig GetConfig()
        {
            if (_config == null)
            {
                _config = new RpsConfig(GetSettingsFile());
            }
            return _config;
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
            string folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "RevitPythonShell2013");
            return Path.Combine(folder, "RevitPythonShell.xml");
        }

        /// <summary>
        /// Returns a list of commands as defined in the repository file.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Command> GetCommands(XDocument repository)
        {
            int i = 0;
            foreach (var commandNode in repository.Root.Descendants("Command") ?? new List<XElement>())
            {
                var commandName = commandNode.Attribute("name").Value;
                var commandSrc = commandNode.Attribute("src").Value;
                var group = commandNode.Attribute("group") == null ? "" : commandNode.Attribute("group").Value;
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
        /// Returns a string to be executed, whenever the revit is started.
        /// If this is not specified in the XML file (under /RevitPythonShell/StartupScript),
        /// then null is returned.
        /// </summary>
        public static string GetStartupScript()
        {
            var startupScriptTags = GetSettings().Root.Descendants("StartupScript") ?? new List<XElement>();
            if (startupScriptTags.Count() == 0)
            {
                return null;
            }
            var firstScript = startupScriptTags.First();
            return firstScript.Value.Trim();
        }

        /// <summary>
        /// Writes settings to the settings file, replacing the old commands.
        /// </summary>
        public static void WriteSettings(
            IEnumerable<Command> commands, 
            IEnumerable<Repository> repositories,
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
                        new XAttribute("src", command.Source),
                        new XAttribute("group", command.Group)));

            }
            doc.Root.Add(xmlCommands);

            // add repositories
            var xmlRepositories = new XElement("Repositories");
            foreach (var repository in repositories)
            {
                xmlRepositories.Add(new XElement(
                    "Repository",
                        new XAttribute("name", repository.Name),
                        new XAttribute("url", repository.Url)));
            }
            doc.Root.Add(xmlRepositories);

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
                        
        public static IEnumerable<Repository> GetRepositories()
        {
            foreach (var repositoryNode in GetSettings().Root.Descendants("Repository"))
            {
                var name = repositoryNode.Attribute("name").Value;
                var url = repositoryNode.Attribute("url").Value;
                yield return new Repository() { Name = name, Url = url };
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

    /// <summary>
    /// A simple structure to hold information about repositories.
    /// </summary>
    internal class Repository
    {
        public string Name;
        public string Url;

        public override string ToString()
        {
            return Name;    
        }

        /// <summary>
        /// Tries to load a repository from a specified path
        /// (see documentation for XmlReader.Create(string))
        /// </summary>
        public static Repository FromPath(string path)
        {
            XDocument repositoryXml;
            try
            {
                repositoryXml = XDocument.Load(path);
            }
            catch
            {
                return null;
            }
            var repositoryNode = repositoryXml.Root.Descendants("Repository").FirstOrDefault();
            if (repositoryNode == null)
            {
                return null;
            }
            var nameAttribute = repositoryNode.Attribute("name");
            if (nameAttribute == null)
            {
                return null;
            }
            var name = nameAttribute.Value;
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }
            return new Repository() { Name = name, Url = path };
        }

        /// <summary>
        /// Return a version of the repositories name that can be used as
        /// a filename.
        /// </summary>
        public string SafeName()
        {
            var invalidChars = new HashSet<char>(
                System.IO.Path.GetInvalidFileNameChars())
                .Union(System.IO.Path.GetInvalidPathChars());
            var safeName = new string(
                Name.Select(c => invalidChars.Contains(c) ? '_' : c).ToArray());
            return safeName;
        }
    }
}
