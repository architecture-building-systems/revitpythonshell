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
        private static string versionNumber;

        /// <summary>
        /// Hook into Revit to allow starting a command.
        /// </summary>
        Result IExternalApplication.OnStartup(UIControlledApplication application)
        {

            try
            {
                versionNumber = application.ControlledApplication.VersionNumber;
                if (application.ControlledApplication.VersionName.ToLower().Contains("vasari"))
                {
                    versionNumber = "_Vasari";
                }
                var dllfolder = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                        "RevitPythonShell" + versionNumber);
                var assemblyName = "CommandLoaderAssembly";
                var dllfullpath = Path.Combine(dllfolder, assemblyName + ".dll");

                var settings = GetSettings();

                CreateCommandLoaderAssembly(settings, dllfolder, assemblyName);
                BuildRibbonPanel(application, dllfullpath);                

                ExecuteStartupScript(application);

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Error setting up RevitPythonShell", ex.ToString());
                return Result.Failed;
            }
        }

        private static void ExecuteStartupScript(UIControlledApplication uiControlledApplication)
        {
            // we need a UIApplication object to assign as `__revit__` in python...
            var versionNumber = uiControlledApplication.ControlledApplication.VersionNumber;
            var fieldName = versionNumber == "2017" ? "m_uiapplication": "m_application";
            var fi = uiControlledApplication.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            var uiApplication = (UIApplication)fi.GetValue(uiControlledApplication);            
            // execute StartupScript
            var startupScript = GetStartupScript();
            if (startupScript != null)
            {
                var executor = new ScriptExecutor(GetConfig(), uiApplication, uiControlledApplication);
                var result = executor.ExecuteScript(startupScript, GetStartupScriptPath());
                if (result == (int)Result.Failed)
                {
                    TaskDialog.Show("RevitPythonShell - StartupScript", executor.Message);
                }
            }
        }

        private static void BuildRibbonPanel(UIControlledApplication application, string dllfullpath)
        {
            var assembly = typeof(RevitPythonShellApplication).Assembly;
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

            PushButtonData pbdOpenNonModalShell = new PushButtonData(
                            "NonModalRevitPythonShell",
                            "Non-modal Shell",
                            assembly.Location,
                            "RevitPythonShell.NonModalConsoleCommand");
            pbdOpenNonModalShell.Image = smallImage;
            pbdOpenNonModalShell.LargeImage = largeImage;
            splitButton.AddPushButton(pbdOpenNonModalShell);


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
                    var pbd = new PushButtonData(command.Name, command.Name, dllfullpath, "Command" + command.Index);
                    pbd.Image = command.SmallImage;
                    pbd.LargeImage = command.LargeImage;
                    splitButton.AddPushButton(pbd);
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

                PushButtonData pbdA = new PushButtonData(command0.Name, command0.Name, dllfullpath, "Command" + command0.Index);
                pbdA.Image = command0.SmallImage;
                pbdA.LargeImage = command0.LargeImage;

                PushButtonData pbdB = new PushButtonData(command1.Name, command1.Name, dllfullpath, "Command" + command1.Index);
                pbdB.Image = command1.SmallImage;
                pbdB.LargeImage = command1.LargeImage;

                PushButtonData pbdC = new PushButtonData(command2.Name, command2.Name, dllfullpath, "Command" + command2.Index);
                pbdC.Image = command2.SmallImage;
                pbdC.LargeImage = command2.LargeImage;

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
                pbdA.Image = command0.SmallImage;
                pbdA.LargeImage = command0.LargeImage;

                PushButtonData pbdB = new PushButtonData(command1.Name, command1.Name, dllfullpath, "Command" + command1.Index);
                pbdB.Image = command0.SmallImage;
                pbdB.LargeImage = command0.LargeImage;

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
                pbdA.Image = command0.SmallImage;
                pbdA.LargeImage = command0.LargeImage;

                PushButtonData pbdB = new PushButtonData(command1.Name, command1.Name, dllfullpath, "Command" + command1.Index);
                pbdB.Image = command1.SmallImage;
                pbdB.LargeImage = command1.LargeImage;

                ribbonPanel.AddStackedItems(pbdA, pbdB);
            }
            if (commands.Count == 1)
            {
                // only one command defined, show as a big button...
                var command = commands[0];
                PushButtonData pbd = new PushButtonData(command.Name, command.Name, dllfullpath, "Command" + command.Index);
                pbd.Image = command.SmallImage;
                pbd.LargeImage = command.LargeImage;
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
        
        public static IRpsConfig GetConfig()
        {           
            return new RpsConfig(GetSettingsFile());
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
            string folder = GetSettingsFolder();
            return Path.Combine(folder, "RevitPythonShell.xml");
        }

        /// <summary>
        /// Returns the name of the folder with the settings file. This folder
        /// is also the default folder for relative paths in StartupScript and InitScript tags.
        /// </summary>
        private static string GetSettingsFolder()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "RevitPythonShell" + versionNumber);
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
                var addinAssembly = typeof(RevitPythonShellApplication).Assembly;
                var commandName = commandNode.Attribute("name").Value;
                var commandSrc = commandNode.Attribute("src").Value;
                var group = commandNode.Attribute("group") == null ? "" : commandNode.Attribute("group").Value;
                
                ImageSource largeImage = null;
                if (IsValidPath(commandNode.Attribute("largeImage")))
                {
                    var largeImagePath = GetAbsolutePath(commandNode.Attribute("largeImage").Value);
                    largeImage = BitmapDecoder.Create(File.OpenRead(largeImagePath), BitmapCreateOptions.None, BitmapCacheOption.None).Frames[0];
                }
                else
                {
                    largeImage = GetEmbeddedPng(addinAssembly, "RevitPythonShell.Resources.PythonScript32x32.png");
                }

                ImageSource smallImage = null;
                if (IsValidPath(commandNode.Attribute("smallImage")))
                {
                    var smallImagePath = GetAbsolutePath(commandNode.Attribute("smallImage").Value);
                    smallImage = BitmapDecoder.Create(File.OpenRead(smallImagePath), BitmapCreateOptions.None, BitmapCacheOption.None).Frames[0];
                }
                else
                {
                    smallImage = GetEmbeddedPng(addinAssembly, "RevitPythonShell.Resources.PythonScript16x16.png");
                }
                
                yield return new Command { 
                        Name = commandName, 
                        Source = commandSrc, 
                        Group = group,
                        LargeImage = largeImage,
                        SmallImage = smallImage,
                        Index = i++
                };
            }
        }

        /// <summary>
        /// True, if the contents of the attribute is a valid absolute path (or relative path to the assembly) is
        /// an existing path.
        /// </summary>
        private static bool IsValidPath(XAttribute pathAttribute)
        {
            if (pathAttribute != null && !string.IsNullOrEmpty(pathAttribute.Value))
            {
                return File.Exists(GetAbsolutePath(pathAttribute.Value));
            }
            return false;
        }

        /// <summary>
        /// Return an absolute path for input path, with relative paths seen as
        /// relative to the assembly location. No guarantees are made as to
        /// wether the path exists or not.
        /// </summary>
        private static string GetAbsolutePath(string path)
        {
            if (Path.IsPathRooted(path))
            {
                return path;
            }
            else
            {
                var assembly = typeof(RevitPythonShellApplication).Assembly;
                return Path.Combine(Path.GetDirectoryName(assembly.Location), path);
            }
        }

        /// <summary>
        /// Returns a string to be executed, whenever the interactive shell is started.
        /// If this is not specified in the XML file (under /RevitPythonShell/InitScript),
        /// then null is returned.
        /// </summary>
        public static string GetInitScript()
        {
            var path = GetInitScriptPath();
            if (File.Exists(path))
            {
                using (var reader = File.OpenText(path))
                {
                    var source = reader.ReadToEnd();
                    return source;
                }
            }

            // backwards compatibility: InitScript used to have a CDATA section directly
            // embedded in the settings xml file
            var initScriptTags = GetSettings().Root.Descendants("InitScript") ?? new List<XElement>();
            if (initScriptTags.Count() == 0)
            {
                return null;
            }
            var firstScript = initScriptTags.First();
            // backwards compatibility: InitScript used to be included as CDATA in the config file
            return firstScript.Value.Trim();
        }

        /// <summary>
        /// Returns the path to the InitScript as configured in the settings file or "" if not
        /// configured. This is used in the ConfigureCommandsForm.
        /// </summary>
        public static string GetInitScriptPath()
        {
            return GetScriptPath("InitScript");
        }


        /// <summary>
        /// Returns the path to the StartupScript as configured in the settings file or "" if not
        /// configured. This is used in the ConfigureCommandsForm.
        /// </summary>
        public static string GetStartupScriptPath()
        {
            return GetScriptPath("StartupScript");
        }

        /// <summary>
        /// Returns the value of the "src" attribute for the tag "tagName" in the settings file
        /// or "" if not configured.
        /// </summary>        
        private static string GetScriptPath(string tagName)
        {
            var tags = GetSettings().Root.Descendants(tagName) ?? new List<XElement>();
            if (tags.Count() == 0)
            {
                return "";
            }
            var firstScript = tags.First();
            if (firstScript.Attribute("src") != null)
            {
                var path = firstScript.Attribute("src").Value;
                if (Path.IsPathRooted(path))
                {
                    return path;
                }
                else
                {
                    return Path.Combine(GetSettingsFolder(), path);
                }
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// Returns a string to be executed, whenever the revit is started.
        /// If this is not specified as a path to an existing file in the XML file (under /RevitPythonShell/StartupScript/@src),
        /// then null is returned.
        /// </summary>
        public static string GetStartupScript()
        {
            var path = GetStartupScriptPath();
            if (File.Exists(path))
            {
                using (var reader = File.OpenText(path))
                {
                    var source = reader.ReadToEnd();
                    return source;
                }
            }
            // no startup script found
            return null;
        }

        /// <summary>
        /// Writes settings to the settings file, replacing the old commands.
        /// </summary>
        public static void WriteSettings(
            IEnumerable<Command> commands,
            IEnumerable<string> searchPaths, 
            IEnumerable<KeyValuePair<string, string>> variables,
            string initScript,
            string startupScript)
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
            foreach (var xmlExistingStartupScript in doc.Root.Descendants("StartupScript").ToList())
            {
                xmlExistingStartupScript.Remove();
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
            xmlInitScript.Add(new XAttribute("src", initScript));
            doc.Root.Add(xmlInitScript);

            // add startup script
            var xmlStartupScript = new XElement("StartupScript");
            xmlStartupScript.Add(new XAttribute("src", startupScript));
            doc.Root.Add(xmlStartupScript);

            doc.Save(GetSettingsFile());
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
        public ImageSource LargeImage;
        public ImageSource SmallImage;        

        public override string ToString()
        {
            return Name;
        }
    }
}
