//---------------------------------------------------------------------------------
//Paths.cs - version 2.0.2.0b
// TextBox control based class designed to be used with Microsoft's IronPython.
// Maybe useful for testing Python scripts with IronPython. 
//
// WHAT'S NEW:
//      - Updated IronPython_Tutorial to point to IronPython 2.0B2 path
//      - Added Python25Dirs 
// TO DO:
//
//BY DOWNLOADING AND USING, YOU AGREE TO THE FOLLOWING TERMS:
//Copyright (c) 2006-2008 by Joseph P. Socoloski III
//LICENSE
//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:
//
//The above copyright notice and this permission notice shall be included in
//all copies or substantial portions of the Software.
//the MIT License, given here: <http://www.opensource.org/licenses/mit-license.php> 
//---------------------------------------------------------------------------------
using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using IronPython.Hosting;

namespace UIIronTextBox
{
    namespace Paths
    {
        /// <summary>
        /// Misc Paths. Customize here.
        /// </summary>
        public class MiscDirs
        {
            /// <summary>
            /// MIT's ConceptNet 2.1 directory.
            /// default: "...MyDocuments) + @"\Python Projects\conceptnet2.1"
            /// </summary>
            public static string ConceptNet = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Python Projects\conceptnet2.1";
            
            /// <summary>
            /// MIT's MontyLingua directory.
            /// default: "...MyDocuments) + @"\Python Projects\conceptnet2.1\montylingua"
            /// </summary>
            public static string montylingua = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Python Projects\conceptnet2.1\montylingua";

            /// <summary>
            /// Visual Studio\Projects Folder.
            /// default: "..MyDocuments) + @"\Visual Studio 2008\Projects"
            /// </summary>
            public static string vs_Projects = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Visual Studio 2008\Projects";

            /*
            /// <summary>
            /// StringCollection of all MiscDirs
            /// </summary>
            public static StringCollection scMiscDirs
            {
                get
                {
                    UIIronTextBox.IronTextBox.scMisc.Clear();
                    UIIronTextBox.IronTextBox.scMisc.Add(ConceptNet);
                    UIIronTextBox.IronTextBox.scMisc.Add(montylingua);
                    UIIronTextBox.IronTextBox.scMisc.Add(vs_Projects); 
                    return UIIronTextBox.IronTextBox.scMisc; }
                //set { UIIronTextBox.IronTextBox.scMisc = value; }
            }
            */

            /// <summary>
            /// Misc Paths.
            /// </summary>
            public MiscDirs()
            {
                
            }
        }

        /// <summary>
        /// Python 2.4 Paths. Customize here.
        /// </summary>
        public class Python24Dirs
        {
            /// <summary>
            /// Folder to Python library modules.
            /// default: "C:\Python24\Lib"
            /// </summary>
            public static string Python24_Lib = @"C:\Python24\Lib";

            /// <summary>
            /// Folder to Tkinter library modules.
            /// default: "C:\Python24\Lib\lib-tk"
            /// </summary>
            public static string Python24_Lib_lib_tk = @"C:\Python24\Lib\lib-tk";

            /// <summary>
            /// default: "C:\Python24\libs"
            /// </summary>
            public static string Python24_libs = @"C:\Python24\libs";

            /// <summary>
            /// default: "C:\Python24\DLLs"
            /// </summary>
            public static string Python24_DLLs = @"C:\Python24\DLLs";

            /// <summary>
            /// Some useful programs written in Python.
            /// default: "C:\Python24\Tools"
            /// </summary>
            public static string Python24_Tools = @"C:\Python24\Tools";

            /// <summary>
            /// Some useful programs written in Python.
            /// default: "C:\Python24\Tools\Scripts"
            /// </summary>
            public static string Python24_Tools_Scripts = @"C:\Python24\Tools\Scripts";

            /*
            /// <summary>
            /// StringCollection of all Python24Dirs
            /// </summary>
            public static StringCollection scPython24Dirs
            {
                get
                {
                    UIIronTextBox.IronTextBox.scPython24.Clear();
                    UIIronTextBox.IronTextBox.scPython24.Add(Python24_Lib);
                    UIIronTextBox.IronTextBox.scPython24.Add(Python24_Lib_lib_tk);
                    UIIronTextBox.IronTextBox.scPython24.Add(Python24_libs);
                    UIIronTextBox.IronTextBox.scPython24.Add(Python24_DLLs);
                    UIIronTextBox.IronTextBox.scPython24.Add(Python24_Tools);
                    UIIronTextBox.IronTextBox.scPython24.Add(Python24_Tools_Scripts);
                    return UIIronTextBox.IronTextBox.scPython24;
                }
            }
            */

            /// <summary>
            /// Python 2.4 Paths
            /// </summary>
            public Python24Dirs()
            {

            }
        }

        /// <summary>
        /// Python 2.5 Paths. Customize here.
        /// </summary>
        public class Python25Dirs
        {
            /// <summary>
            /// Folder to Python library modules.
            /// default: "C:\Python25\Lib"
            /// </summary>
            public static string Python25_Lib = @"C:\Python25\Lib";

            /// <summary>
            /// Folder to Tkinter library modules.
            /// default: "C:\Python25\Lib\lib-tk"
            /// </summary>
            public static string Python25_Lib_lib_tk = @"C:\Python25\Lib\lib-tk";

            /// <summary>
            /// default: "C:\Python25\libs"
            /// </summary>
            public static string Python25_libs = @"C:\Python25\libs";

            /// <summary>
            /// default: "C:\Python25\DLLs"
            /// </summary>
            public static string Python25_DLLs = @"C:\Python25\DLLs";

            /// <summary>
            /// Some useful programs written in Python.
            /// default: "C:\Python25\Tools"
            /// </summary>
            public static string Python25_Tools = @"C:\Python25\Tools";

            /// <summary>
            /// Some useful programs written in Python.
            /// default: "C:\Python25\Tools\Scripts"
            /// </summary>
            public static string Python25_Tools_Scripts = @"C:\Python25\Tools\Scripts";

            /*
            /// <summary>
            /// StringCollection of all Python25Dirs
            /// </summary>
            public static StringCollection scPython25Dirs
            {
                get
                {
                    UIIronTextBox.IronTextBox.scPython25.Clear();
                    UIIronTextBox.IronTextBox.scPython25.Add(Python25_Lib);
                    UIIronTextBox.IronTextBox.scPython25.Add(Python25_Lib_lib_tk);
                    UIIronTextBox.IronTextBox.scPython25.Add(Python25_libs);
                    UIIronTextBox.IronTextBox.scPython25.Add(Python25_DLLs);
                    UIIronTextBox.IronTextBox.scPython25.Add(Python25_Tools);
                    UIIronTextBox.IronTextBox.scPython25.Add(Python25_Tools_Scripts);
                    return UIIronTextBox.IronTextBox.scPython25;
                }
            }
            */

            /// <summary>
            /// Python 2.5 Paths
            /// </summary>
            public Python25Dirs()
            {

            }
        }
        
        /// <summary>
        /// IronPython Paths. Customize here.
        /// </summary>
        public class IronPythonDirs
        {
            /// <summary>
            /// Current Assembly.Location
            /// </summary>
            public static string Runtime  = Path.GetDirectoryName(typeof(IronPythonDirs).Assembly.Location);

            /// <summary>
            /// IronPython Tutorial scripts.
            /// default: "...MyDocuments) + @"\Visual Studio 2008\Projects\IronPython-2.0B2\Tutorial"
            /// </summary>
            public static string IronPython_Tutorial = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Visual Studio 2008\Projects\IronPython-2.0B2\Tutorial";

            /// <summary>
            /// IronPython Paths
            /// </summary>
            static IronPythonDirs()
            {

            }
        }

    }

}
