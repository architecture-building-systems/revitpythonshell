using System;
using System.Configuration.Install;
using Autodesk.RevitAddIns;
using System.IO;
using System.ComponentModel;
using System.Collections;
using System.Diagnostics;
using System.Linq;

namespace RegisterRevit2013Addin
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
            // Debugger.Launch();
            // make sure addin manifest is written
            var assemblyFolder = Path.GetDirectoryName(Context.Parameters["assemblypath"]);
            var assemblyPath = Path.Combine(assemblyFolder, "RevitPythonShell.dll");            

            var manifest = new RevitAddInManifest();
            var application = new RevitAddInApplication("RevitPythonShell", assemblyPath, Guid.Parse("3a7a1d24-51ed-462b-949f-1ddcca12008d"),
                "RevitPythonShell.RevitPythonShellApplication", "RIPS");
            manifest.AddInApplications.Add(application);
            var revitProducts = RevitProductUtility.GetAllInstalledRevitProducts();

            if (revitProducts.Count < 1)
            {
                throw new InvalidOperationException("No Autodesk Revit products found");
            }

            foreach (var product in revitProducts.Where(p => p.Version == RevitVersion.Revit2013))
            {
                var manifestFolder = Context.Parameters["allusers"] == "1"
                    ? product.AllUsersAddInFolder
                    : product.CurrentUserAddInFolder;
                var manifestPath = Path.Combine(manifestFolder, "RevitPythonShell2013.addin");


                manifest.SaveAs(manifestPath);
            }

            base.Install(stateSaver);
        }

        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
        public override void Commit(IDictionary savedState)
        {
            base.Commit(savedState);
        }

        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
        public override void Rollback(IDictionary savedState)
        {
            base.Rollback(savedState);
        }

        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
        public override void Uninstall(IDictionary savedState)
        {
            // Debugger.Launch();
            // remove addin manifest
            foreach (var product in RevitProductUtility.GetAllInstalledRevitProducts().Where(p => p.Version == RevitVersion.Revit2013))
            {
                var manifestFolder = Context.Parameters["allusers"] == "1"
                    ? product.AllUsersAddInFolder
                    : product.CurrentUserAddInFolder;
                var manifestPath = Path.Combine(manifestFolder, "RevitPythonShell2013.addin");

                if (File.Exists(manifestPath))
                {
                    File.Delete(manifestPath);
                }
            }

            base.Uninstall(savedState);
        }
    }
}
