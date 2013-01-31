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
                }                
            }                        
        }

        /// <summary>
        /// Read in a PushButton tag and produce a PushButtonData object that can be used to
        /// add the PushButton to the UI (e.g. a RibbonPanel or SplitButton)
        /// </summary>
        private PushButtonData BuildPushButtonData(Assembly addinAssembly, XElement xmlPushButton)
        {
            var script = xmlPushButton.Attribute("script").Value;       // e.g. "helloworld.py"
            var scriptName = Path.GetFileNameWithoutExtension(script);  // e.g. "helloworld"
            var pbName = "pb_" + scriptName;                            // e.g. "pb_helloworld  ("pb" stands for "PushButton")
            var className = "ec_" + scriptName;                         // e.g. "ec_helloworld" ("ec" stands for "ExternalCommand")
            var text = xmlPushButton.Attribute("text").Value;           // the user visible text on the button

            var result = new PushButtonData(pbName, text, addinAssembly.Location, className);

            if (xmlPushButton.Attribute("largeImage") != null && !string.IsNullOrEmpty(xmlPushButton.Attribute("largeImage").Value))
            {
                var largeImagePath = Path.Combine(addinAssembly.Location, xmlPushButton.Attribute("largeImage").Value);
                result.LargeImage = BitmapDecoder.Create(File.OpenRead(largeImagePath), BitmapCreateOptions.None, BitmapCacheOption.None).Frames[0];
            }
            else
            {
                result.LargeImage = GetEmbeddedPng("RevitPythonShell.RpsRuntime.Resources.PythonScript32x32.png");
            }

            if (xmlPushButton.Attribute("smallImage") != null && !string.IsNullOrEmpty(xmlPushButton.Attribute("smallImage").Value))
            {
                var largeImagePath = Path.Combine(addinAssembly.Location, xmlPushButton.Attribute("smallImage").Value);
                result.LargeImage = BitmapDecoder.Create(File.OpenRead(largeImagePath), BitmapCreateOptions.None, BitmapCacheOption.None).Frames[0];
            }
            else
            {
                result.Image = GetEmbeddedPng("RevitPythonShell.RpsRuntime.Resources.PythonScript16x16.png");
            }

            return result;
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
    }
}
