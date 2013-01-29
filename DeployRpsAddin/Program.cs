using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;

namespace DeployRpsAddin
{
    class Program
    {
        /// <summary>
        /// Expect a RpsAddin xml file as first argument. Create a subfolder
        /// with timestamp containing the deployable version of the RPS scripts.
        /// 
        /// This includes the RpsRuntime.dll (see separate project) that recreates some
        /// of the RPS experience for canned commands.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            // read in rpsaddin.xml
            var rpsAddinXmlPath = args[0]; // FIXME: do some argument checking here            
            
            var basename = Path.GetFileNameWithoutExtension(rpsAddinXmlPath);
            var rootFolder = Path.GetDirectoryName(rpsAddinXmlPath);

            var doc = XDocument.Load(rpsAddinXmlPath);

            // create subfolder
            var outputFolder = CreateOutputFolder(rootFolder, basename);

            // copy static stuff (rpsaddin runtime, ironpython dlls etc., addin installation utilities)

            // copy files mentioned (they must all be unique)

            // create addin assembly

            // create innosetup script
        }

        /// <summary>
        /// Creates a subfolder in rootFolder with a timestamp and the basename of the
        /// RpsAddin xml file and returns the name of that folder.
        /// 
        /// Example result: "2013.01.28.16.40.06_HelloWorld"
        /// </summary>
        private static string CreateOutputFolder(string rootFolder, string basename)
        {
            var folderName = string.Format("{0}_{1}", DateTime.Now.ToString("yyyy.MM.dd.HH.mm.ss"), basename);
            var folderPath = Path.Combine(rootFolder, folderName);
            Directory.CreateDirectory(folderPath);
            return folderPath;
        }
    }
}
