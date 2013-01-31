using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.Revit.UI;
using System.Reflection;
using System.IO;
using System.Xml.Linq;

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

            // FIXME: add icons here!

            return result;
        }
    }
}
