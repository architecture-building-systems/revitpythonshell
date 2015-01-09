using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.Revit.UI;
using System.Reflection;
using System.IO;
using System.Xml.Linq;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace RevitPythonShell.RpsRuntime
{
    /// <summary>
    /// A base class for RpsAddins to inherit from.
    /// 
    /// The class is based on the idea that the RpsAddin is defined in an assembly with
    /// embedded resources:
    /// 
    ///   - the RpsAddin xml file that contains the configuration for the addin
    ///     - this file is called the same as the RpsAddin dll, (e.g. "HelloWorld.xml")
    ///   - the scripts referenced in the RpsAddin xml file
    ///   - icons used for the user interface
    ///   
    /// To keep the Reflection.Emit code in DeployRpsAddin as simple as possible, all
    /// that needs to be done is subclass from RpsExternalApplicationBase in a dll
    /// that has the scripts and a RpsAddin xml file embedded as resources.
    /// </summary>
    public abstract class RpsExternalApplicationBase: IExternalApplication
    {
        Result IExternalApplication.OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        /// <summary>
        /// Build the RibbonPanel based on the embedded RpsAddin xml file.
        /// </summary>
        Result IExternalApplication.OnStartup(UIControlledApplication application)
        {
            var addinName = this.GetType().Name; // subclass provides the name
            var addinAssembly = this.GetType().Assembly;

            var addinXml = XDocument.Load(addinAssembly.GetManifestResourceStream(addinName + ".xml"));

            BuildRibbonPanels(application, addinXml, addinAssembly);
            ExecuteStartupScript(application, addinXml, addinAssembly);

            return Result.Succeeded;
        }

        /// <summary>
        /// Reads in the parts of the RpsAddin xml file that specify how to 
        /// </summary>
        /// <param name="application"></param>
        /// <param name="addinXml"></param>
        private void BuildRibbonPanels(UIControlledApplication application, XDocument addinXml, Assembly addinAssembly)
        {
            foreach (var xmlRibbonPanel in addinXml.Root.Descendants("RibbonPanel"))
            {
                var ribbonPanel = application.CreateRibbonPanel(xmlRibbonPanel.Attribute("text").Value);
                foreach (var element in xmlRibbonPanel.Elements())
                {
                    if (element.Name == "PushButton")
                    {
                        var pushButtonData = BuildPushButtonData(addinAssembly, element);
                        ribbonPanel.AddItem(pushButtonData);
                    }
                    else if (element.Name == "SplitButton")
                    {
                        var splitButton = ribbonPanel.AddItem(
                            new SplitButtonData(
                                element.Attribute("text").Value, 
                                element.Attribute("text").Value)) as SplitButton;
                        foreach (var xmlPushButton in element.Descendants("PushButton"))
                        {
                            var pushButtonData = BuildPushButtonData(addinAssembly, xmlPushButton);
                            splitButton.AddPushButton(pushButtonData);
                        }
                    }
                }                
            }                        
        }

        /// <summary>
        /// Read in a PushButton tag and produce a PushButtonData object that can be used to
        /// add the PushButton to the UI (e.g. a RibbonPanel or SplitButton)
        /// </summary>
        private PushButtonData BuildPushButtonData(Assembly addinAssembly, XElement xmlPushButton)
        {
            var script = xmlPushButton.Attribute("src").Value;       // e.g. "helloworld.py"
            var scriptName = Path.GetFileNameWithoutExtension(script);  // e.g. "helloworld"
            var pbName = "pb_" + scriptName;                            // e.g. "pb_helloworld  ("pb" stands for "PushButton")
            var className = "ec_" + scriptName;                         // e.g. "ec_helloworld" ("ec" stands for "ExternalCommand")
            var text = xmlPushButton.Attribute("text").Value;           // the user visible text on the button

            var result = new PushButtonData(pbName, text, addinAssembly.Location, className);

            if (IsValidPath(xmlPushButton.Attribute("largeImage")))
            {
                var largeImagePath = GetAbsolutePath(xmlPushButton.Attribute("largeImage").Value);
                result.LargeImage = BitmapDecoder.Create(File.OpenRead(largeImagePath), BitmapCreateOptions.None, BitmapCacheOption.None).Frames[0];
            }
            else
            {
                result.LargeImage = GetEmbeddedPng("RevitPythonShell.RpsRuntime.Resources.PythonScript32x32.png");
            }

            if (IsValidPath(xmlPushButton.Attribute("smallImage")))
            {
                var smallImagePath = GetAbsolutePath(xmlPushButton.Attribute("smallImage").Value);
                result.Image = BitmapDecoder.Create(File.OpenRead(smallImagePath), BitmapCreateOptions.None, BitmapCacheOption.None).Frames[0];
            }
            else
            {
                result.Image = GetEmbeddedPng("RevitPythonShell.RpsRuntime.Resources.PythonScript16x16.png");
            }

            return result;
        }

        /// <summary>
        /// True, if the contents of the attribute is a valid absolute path (or relative path to the assembly) is
        /// an existing path.
        /// </summary>
        private bool IsValidPath(XAttribute pathAttribute)
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
        private string GetAbsolutePath(string path)
        {
            if (Path.IsPathRooted(path))
            {
                return path;
            }
            else
            {
                var assembly = this.GetType().Assembly;
                return Path.Combine(Path.GetDirectoryName(assembly.Location), path);
            }
        }

        /// <summary>
        /// Given a string representing an embedded png image, return it
        /// as an ImageSource, which can be used for PushButtons etc.
        /// 
        /// NOTE: any embedded resource in the "Resources" folder has the name
        /// "RpsRuntime.Resources.BASENAME.EXT"
        /// </summary>
        private ImageSource GetEmbeddedPng(string imageName)
        {
            var assembly = typeof(RpsExternalApplicationBase).Assembly;
            var file = assembly.GetManifestResourceStream(imageName);
            var source = PngBitmapDecoder.Create(file, BitmapCreateOptions.None, BitmapCacheOption.None);
            return source.Frames[0];
        }

        /// <summary>
        /// Execute the startup script (specified under /RpsAddin/StartupScript/@src)
        /// </summary>
        /// <param name="uiControlledApplication"></param>
        private void ExecuteStartupScript(UIControlledApplication uiControlledApplication, XDocument addinXml, Assembly addinAssembly)
        {
            // we need a UIApplication object to assign as `__revit__` in python...
            var fi = uiControlledApplication.GetType().GetField("m_application", BindingFlags.NonPublic | BindingFlags.Instance);
            var uiApplication = (UIApplication)fi.GetValue(uiControlledApplication);
            // execute StartupScript
            var startupScript = GetStartupScript(addinXml, addinAssembly);
            if (startupScript != null)
            {
                var executor = new ScriptExecutor(GetConfig(), uiApplication, uiControlledApplication);
                var result = executor.ExecuteScript(startupScript);
                if (result == (int)Result.Failed)
                {
                    // FIXME: make the TaskDialog show the addins name.
                    TaskDialog.Show("RevitPythonShell - StartupScript", executor.Message);
                }
            }
        }

        /// <summary>
        /// Returns a string to be executed, whenever the revit is started.
        /// If this is not specified as a path to an existing file in the XML file (under /RpsAddin/StartupScript/@src),
        /// then null is returned.
        /// </summary>
        private string GetStartupScript(XDocument addinXml, Assembly addinAssembly)
        {
            var startupScriptTags = addinXml.Root.Descendants("StartupScript") ?? new List<XElement>();
            if (startupScriptTags.Count() == 0)
            {
                return null;
            }
            var tag = startupScriptTags.First();            
            var scriptName = tag.Attribute("src").Value;
            var source = new StreamReader(addinAssembly.GetManifestResourceStream(scriptName)).ReadToEnd();
            return source;
        }

        /// <summary>
        /// Search for the config file first in the user preferences,
        /// then in the all users preferences.
        /// If not found, a new (empty) config file is created in the user preferences.
        /// 
        /// FIXME: for now, this is a copy of the RpsExternalCommandBase.GetConfig() method. I am
        /// duplicating this code, because I need "this" to reference a subclass in the RpsAddin assembly
        /// and don't know how to fix that right now...
        /// </summary>
        private RpsConfig GetConfig()
        {
            var addinName = Path.GetFileNameWithoutExtension(this.GetType().Assembly.Location);
            var fileName = addinName + ".xml";
            var userFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), addinName);

            var userFolderFile = Path.Combine(userFolder, fileName);
            if (File.Exists(userFolderFile))
            {
                return new RpsConfig(userFolderFile);
            }

            var allUserFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), addinName);
            var allUserFolderFile = Path.Combine(allUserFolder, addinName);
            if (File.Exists(allUserFolderFile))
            {
                return new RpsConfig(allUserFolderFile);
            }

            // create a new file in users appdata and return that
            var doc = new XDocument(
                new XElement("RevitPythonShell",
                    new XElement("SearchPaths"),
                    new XElement("Variables")));

            if (!Directory.Exists(userFolder))
            {
                Directory.CreateDirectory(userFolder);
            }

            doc.Save(userFolderFile);
            return new RpsConfig(userFolderFile);
        }
    }
}
