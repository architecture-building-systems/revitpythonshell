using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.Revit.UI;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Xml.Linq;
using Autodesk.Revit.Attributes;
using System.Windows.Forms;
using RevitPythonShell.RpsRuntime;

namespace RevitPythonShell
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
        Result IExternalCommand.Execute(ExternalCommandData commandData, ref string message, Autodesk.Revit.DB.ElementSet elements)
        {
            // read in rpsaddin.xml
            var rpsAddinXmlPath = GetAddinXmlPath(); // FIXME: do some argument checking here            

            var addinName = Path.GetFileNameWithoutExtension(rpsAddinXmlPath);
            var rootFolder = Path.GetDirectoryName(rpsAddinXmlPath);

            var doc = XDocument.Load(rpsAddinXmlPath);

            // create subfolder
            var outputFolder = CreateOutputFolder(rootFolder, addinName);

            // copy static stuff (rpsaddin runtime, ironpython dlls etc., addin installation utilities)

            // copy files mentioned (they must all be unique)

            // create addin assembly
            CreateAssembly(outputFolder, addinName, doc, rootFolder);

            // create innosetup script

            return Result.Succeeded;
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
        private void CreateAssembly(string outputFolder, string addinName, XDocument doc, string sourceFolder)
        {
            var assemblyName = new AssemblyName { Name = addinName + ".dll", Version = new Version(1, 0, 0, 0) }; // FIXME: read version from doc
            var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave, outputFolder);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule("RpsAddinModule", addinName + ".dll");

            foreach (var xmlPushButton in doc.Descendants("PushButton"))
            {
                var scriptFile = xmlPushButton.Attribute("script").Value;                // e.g. "C:\projects\helloworld\helloworld.py" or "..\helloworld.py"
                if (!Path.IsPathRooted(scriptFile))
                {
                    scriptFile = Path.Combine(sourceFolder, scriptFile);
                }
                var newScriptFile = Path.GetFileName(scriptFile);                        // e.g. "helloworld.py" - strip path for embedded resource
                var className = "ec_" + Path.GetFileNameWithoutExtension(newScriptFile); // e.g. "ec_helloworld", "ec" stands for ExternalCommand

                var scriptStream = File.OpenRead(scriptFile);
                moduleBuilder.DefineManifestResource(newScriptFile, scriptStream, ResourceAttributes.Public);

                // script has new path inside assembly, rename it for the RpsAddin xml file we intend to save as a resource
                xmlPushButton.Attribute("script").Value = newScriptFile;

                var typeBuilder = moduleBuilder.DefineType(
                    className,
                    TypeAttributes.Class | TypeAttributes.Public,
                    typeof(RpsExternalCommandBase));

                AddRegenerationAttributeToType(typeBuilder);
                AddTransactionAttributeToType(typeBuilder);

                typeBuilder.CreateType();            
            }

            AddRpsAddinXmlToAssembly(addinName, doc, moduleBuilder);
            AddExternalApplicationToAssembly(addinName, moduleBuilder);
            assemblyBuilder.Save(addinName + ".dll");
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
            moduleBuilder.DefineManifestResource(addinName + ".xml", stream, ResourceAttributes.Public);
        }

        /// <summary>
        /// Creates a subfolder in rootFolder with a timestamp and the basename of the
        /// RpsAddin xml file and returns the name of that folder.
        /// 
        /// Example result: "2013.01.28.16.40.06_HelloWorld"
        /// </summary>
        private string CreateOutputFolder(string rootFolder, string basename)
        {
            var folderName = string.Format("{0}_{1}", DateTime.Now.ToString("yyyy.MM.dd.HH.mm.ss"), basename);
            var folderPath = Path.Combine(rootFolder, folderName);
            Directory.CreateDirectory(folderPath);
            return folderPath;
        }
    }
}
