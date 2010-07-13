using System;
using System.Configuration.Install;
using Autodesk.RevitAddIns;
using System.IO;
using System.ComponentModel;
using System.Collections;
using System.Diagnostics;


namespace RegisterRevit2011Addin
{
    [RunInstaller(true)]
    public partial class RegisterAddinCustomAction : System.Configuration.Install.Installer
    {
        public RegisterAddinCustomAction()
        {
        }

        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
        public override void Install(IDictionary stateSaver)
        {
            Debugger.Break();

            // make sure addin manifest is written
            var assemblyFolder = Path.GetDirectoryName(Context.Parameters["assemblypath"]);
            var assemblyPath = Path.Combine(assemblyFolder, "RevitPythonShell.dll");

            var manifest = new RevitAddInManifest();
            var application = new RevitAddInApplication("RevitPythonShell", assemblyPath, Guid.NewGuid(), 
                "RevitPythonShell.RevitPythonShellApplication");
            manifest.AddInApplications.Add(application);
            var revitProduct = RevitProductUtility.GetAllInstalledRevitProducts();

            base.Install(stateSaver);
        }

        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
        public override void Commit(IDictionary savedState)
        {
            base.Commit(savedState);
            System.Diagnostics.Process.Start("http://www.microsoft.com");
        }

        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
        public override void Rollback(IDictionary savedState)
        {
            base.Rollback(savedState);
        }

        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
        public override void Uninstall(IDictionary savedState)
        {
            base.Uninstall(savedState);
        }
    }
}
