using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Resources;
using System.Windows.Forms;
using System.Xml.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using RpsRuntime;

namespace RevitPythonShell.RevitCommands
{
    /// <summary>
    /// Ask the user for an RpsAddin xml file. Create a subfolder
    /// with timestamp containing the deployable version of the RPS scripts.
    /// 
    /// This includes the RpsRuntime.dll (see separate project) that recreates some
    /// of the RPS experience for canned commands.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class DeployRpsAddinCommand: IExternalCommand
    {
        private string _outputFolder;
        private string _rootFolder;
        private string _addinName;
        private XDocument _doc;

        Result IExternalCommand.Execute(ExternalCommandData commandData, ref string message, Autodesk.Revit.DB.ElementSet elements)
        {
            try
            {
                // read in rpsaddin.xml
                var rpsAddinXmlPath = GetAddinXmlPath(); // FIXME: do some argument checking here            

                _addinName = Path.GetFileNameWithoutExtension(rpsAddinXmlPath);
                _rootFolder = Path.GetDirectoryName(rpsAddinXmlPath);

                _doc = XDocument.Load(rpsAddinXmlPath);

                // create subfolder
                _outputFolder = CreateOutputFolder();

                // copy static stuff (rpsaddin runtime, ironpython dlls etc., addin installation utilities)
                CopyFile(typeof(RpsExternalApplicationBase).Assembly.Location);          // RpsRuntime.dll

                var ironPythonPath = Path.GetDirectoryName(this.GetType().Assembly.Location);
                CopyFile(Path.Combine(ironPythonPath, "IronPython.dll"));                    // IronPython.dll
                CopyFile(Path.Combine(ironPythonPath, "IronPython.Modules.dll"));            // IronPython.Modules.dll            
                CopyFile(Path.Combine(ironPythonPath, "Microsoft.Scripting.dll"));           // Microsoft.Scripting.dll
                CopyFile(Path.Combine(ironPythonPath, "Microsoft.Scripting.Metadata.dll"));  // Microsoft.Scripting.Metadata.dll
                CopyFile(Path.Combine(ironPythonPath, "Microsoft.Dynamic.dll"));             // Microsoft.Dynamic.dll

                // copy files mentioned (they must all be unique)
                CopyIcons();

                CopyExplicitFiles();

                // create addin assembly
                CreateAssembly();

                Autodesk.Revit.UI.TaskDialog.Show("Deploy RpsAddin", "Deployment complete - see folder: " + _outputFolder);

                return Result.Succeeded;
            }
            catch (Exception exception)
            {

                Autodesk.Revit.UI.TaskDialog.Show("Deploy RpsAddin", "Error deploying addin: " + exception.ToString());
                return Result.Failed;
            }
        }

        /// <summary>
        /// Copy any icon files mentioned in PushButton tags. 
        /// 
        /// The PythonScript16x16.png and PythonScript32x32.png icons will be used as default,
        /// if no icons are found (they are embedded in the RpsRuntime.dll)
        /// 
        /// as always, relative paths are assumed to be relative to rootFolder, that
        /// is the folder that the RpsAddin xml file came from.
        /// </summary>
        private void CopyIcons()
        {
            HashSet<string> copiedIcons = new HashSet<string>();

            foreach (var pb in _doc.Descendants("PushButton"))
            {
                CopyReferencedFileToOutputFolder(pb.Attribute("largeImage"));
                CopyReferencedFileToOutputFolder(pb.Attribute("smallImage"));
            }
        }        

        /// <summary>
        /// Copy a file to the output folder ("flat" folder structure!)
        /// </summary>
        private void CopyFile(string path)
        {
            File.Copy(path, Path.Combine(_outputFolder, Path.GetFileName(path)));
        }

        /// <summary>
        /// Copy all files mentioned in /Files/File tags.
        /// </summary>
        private void CopyExplicitFiles()
        {
            foreach (var xmlFile in _doc.Descendants("Files").SelectMany(f => f.Descendants("File")))
            {
                var source = xmlFile.Attribute("src").Value;
                var sourcePath = GetRootedPath(_rootFolder, source);

                if (!File.Exists(sourcePath))
                {
                    throw new FileNotFoundException(
                        "Could not find the explicitly referenced file",
                        source);
                }

                var fileName = Path.GetFileName(sourcePath);
                File.Copy(sourcePath, Path.Combine(_outputFolder, fileName));

                // remove path information for deployment
                xmlFile.Attribute("src").Value = fileName;
            }
        }


        /// <summary>
        /// Copies a referenced file to the output folder, unless it could not find that
        /// file.
        /// </summary>
        private void CopyReferencedFileToOutputFolder(XAttribute attr)
        {
            if (attr == null)
            {
                return;
            }

            var path = GetRootedPath(_rootFolder, attr.Value);
            if (path != null)
            {
                if (!File.Exists(path))
                {
                    throw new FileNotFoundException(
                        "Could not find the file referenced by attribute " + attr.Name,
                        attr.Value);
                }

                var fileName = Path.GetFileName(path);
                File.Copy(path, Path.Combine(_outputFolder, fileName));
                
                // make the new value relative, for the embedded RpsAddin xml
                attr.Value = fileName;
            }                           
        }

        /// <summary>
        /// Show a FileDialog for the RpsAddinXml file and return the path.
        /// </summary>
        private string GetAddinXmlPath()
        {
            var dialog = new OpenFileDialog();
            dialog.CheckFileExists = true;
            dialog.CheckPathExists = true;
            dialog.Multiselect = false;
            dialog.DefaultExt = "xml";
            dialog.Filter = "RpsAddin xml files (*.xml)|*.xml";

            dialog.ShowDialog();
            return dialog.FileName;
        }

        /// <summary>
        /// Create a new dll Assembly in the outputFolder with the addinName and
        /// add the RpsAddin xml file and all script files referenced by PushButton tags
        /// as embedded resources, plus, for each such script, add a subclass of
        /// RpsExternalCommand to load the script from.    
        /// </summary>

        private void CreateAssembly()
        {
            var assemblyName = new AssemblyName { Name = _addinName + ".dll", Version = new Version(1, 0, 0, 0) };

#if !NET8_0
            var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave, _outputFolder);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule("RpsAddinModule", _addinName + ".dll");
#else
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndCollect);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule("RpsAddinModule");
            var resourceFilePath = Path.Combine(_outputFolder, $"{_addinName}.resources");
#endif

#if !NET8_0
            foreach (var xmlPushButton in _doc.Descendants("PushButton"))
            {
                string scriptFileName = xmlPushButton.Attribute("src")?.Value ?? xmlPushButton.Attribute("script")?.Value;
                if (scriptFileName == null)
                {
                    throw new ApplicationException("<PushButton/> tag missing a src attribute in addin manifest");
                }

                var scriptFile = GetRootedPath(_rootFolder, scriptFileName);
                var newScriptFile = Path.GetFileName(scriptFile);
                using (var scriptStream = File.OpenRead(scriptFile))
                {
                    moduleBuilder.DefineManifestResource(newScriptFile, scriptStream, ResourceAttributes.Public);
                }
                xmlPushButton.Attribute("src").Value = newScriptFile;
            }
#else
            using (var resWriter = new ResourceWriter(resourceFilePath))
            {
                foreach (var xmlPushButton in _doc.Descendants("PushButton"))
                {
                    string scriptFileName = xmlPushButton.Attribute("src")?.Value ?? xmlPushButton.Attribute("script")?.Value;
                    if (scriptFileName == null)
                    {
                        throw new ApplicationException("<PushButton/> tag missing a src attribute in addin manifest");
                    }

                    var scriptFile = GetRootedPath(_rootFolder, scriptFileName);
                    var newScriptFile = Path.GetFileName(scriptFile);
                    using (var scriptStream = File.OpenRead(scriptFile))
                    {
                        resWriter.AddResource(newScriptFile, scriptStream);
                    }
                    xmlPushButton.Attribute("src").Value = newScriptFile;
                }
                resWriter.Generate();
            }
#endif

            AddExternalApplicationToAssembly(_addinName, moduleBuilder);
        }



        /// <summary>
        /// Returns the possiblyRelativePath rooted in sourceFolder,
        /// if it is relative or unchanged if it is absolute already.
        /// if the input is null or an empty string, returns null.
        /// </summary>
        private static string GetRootedPath(string sourceFolder, string possiblyRelativePath)
        {
            if (string.IsNullOrEmpty(possiblyRelativePath))
            {
                return null;
            }

            if (!Path.IsPathRooted(possiblyRelativePath))
            {
                return Path.Combine(sourceFolder, possiblyRelativePath);
            }
            return possiblyRelativePath;
        }

        /// <summary>
        /// Adds a subclass of RpsExternalApplicationBase to make the assembly
        /// work as an external application.
        /// </summary>
        private void AddExternalApplicationToAssembly(string addinName, ModuleBuilder moduleBuilder)
        {
            var typeBuilder = moduleBuilder.DefineType(
                addinName,
                TypeAttributes.Class | TypeAttributes.Public,
                typeof(RpsExternalApplicationBase));
            AddRegenerationAttributeToType(typeBuilder);
            AddTransactionAttributeToType(typeBuilder);
            typeBuilder.CreateType();
        }

        /// <summary>
        /// Adds the [Transaction(TransactionMode.Manual)] attribute to the type.        
        /// </summary>
        private void AddTransactionAttributeToType(TypeBuilder typeBuilder)
        {
            var transactionConstructorInfo = typeof(TransactionAttribute).GetConstructor(new Type[] { typeof(TransactionMode) });
            var transactionAttributeBuilder = new CustomAttributeBuilder(transactionConstructorInfo, new object[] { TransactionMode.Manual });
            typeBuilder.SetCustomAttribute(transactionAttributeBuilder);
        }

        /// <summary>
        /// Adds the [Transaction(TransactionMode.Manual)] attribute to the type.
        /// </summary>
        /// <param name="typeBuilder"></param>
        private void AddRegenerationAttributeToType(TypeBuilder typeBuilder)
        {
            var regenerationConstrutorInfo = typeof(RegenerationAttribute).GetConstructor(new Type[] { typeof(RegenerationOption) });
            var regenerationAttributeBuilder = new CustomAttributeBuilder(regenerationConstrutorInfo, new object[] { RegenerationOption.Manual });
            typeBuilder.SetCustomAttribute(regenerationAttributeBuilder);
        }

        private void AddRpsAddinXmlToAssembly(string addinName, XDocument doc, ModuleBuilder moduleBuilder)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(doc.ToString());
            writer.Flush();
            stream.Position = 0;

#if !NET8_0
            moduleBuilder.DefineManifestResource(addinName + ".xml", stream, ResourceAttributes.Public);
#else
            var resourceFilePath = Path.Combine(_outputFolder, $"{_addinName}.resources");
            using (var resWriter = new ResourceWriter(resourceFilePath))
            {
                resWriter.AddResource(addinName + ".xml", stream);
                resWriter.Generate();
            }
#endif
        }

        /// <summary>
        /// Creates a subfolder in rootFolder with the basename of the
        /// RpsAddin xml file and returns the name of that folder.
        /// 
        /// deletes previous folders.
        /// 
        /// result: "Output_HelloWorld"
        /// </summary>
        private string CreateOutputFolder()
        {
            var folderName = $"Output_{_addinName}";
            var folderPath = Path.Combine(_rootFolder, folderName);
            if (Directory.Exists(folderPath))
            {
                Directory.Delete(folderPath, true);
            }
#if !NET8_0
            Directory.CreateDirectory(folderPath, Directory.GetAccessControl(_rootFolder));
#else
            Directory.CreateDirectory(folderPath);
#endif
            return folderPath;
        }
    }
}
