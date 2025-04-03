using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using WixSharp;
using WixSharp.CommonTasks;
using WixSharp.Controls;

const string installationDir = @"%AppDataFolder%\Autodesk\Revit\Addins\";
const string projectName = "RevitPythonShell";
const string outputName = "RevitPythonShell";
const string outputDir = "output";
const string version = "2.2.0";

var fileName = new StringBuilder().Append(outputName).Append("-").Append(version);
var project = new Project
{
    Name = projectName,
    OutDir = outputDir,
    Platform = Platform.x64,
    Description = "The RevitPythonShell adds an IronPython interpreter to Autodesk Revit and Vasari.",
    UI = WUI.WixUI_InstallDir,
    Version = new Version(version),
    OutFileName = fileName.ToString(),
    InstallScope = InstallScope.perUser,
    MajorUpgrade = MajorUpgrade.Default,
    GUID = new Guid("8A43E94C-B89C-4135-8D7C-B8E51DCE70D5"),
    BackgroundImage = @"Installer\Resources\Icons\BackgroundImage.png",
    BannerImage = @"Installer\Resources\Icons\BannerImage.png",
    ControlPanelInfo =
    {
        Manufacturer = "architecture-building-systems",
        HelpLink = "https://github.com/architecture-building-systems/revitpythonshell",
        Comments = "The RevitPythonShell adds an IronPython interpreter to Autodesk Revit and Vasari.",
        ProductIcon = @"Installer\Resources\Icons\ShellIcon.ico",
    },
    Dirs = new Dir[]
    {
        new InstallDir(installationDir, GenerateWixEntities())
    }
};

MajorUpgrade.Default.AllowSameVersionUpgrades = true;
project.RemoveDialogsBetween(NativeDialogs.WelcomeDlg, NativeDialogs.InstallDirDlg);
project.BuildMsi();

WixEntity[] GenerateWixEntities()
{
    var versionRegex = new Regex(@"\d+");
    var versionStorages = new Dictionary<string, List<WixEntity>>();

    foreach (var directory in args)
    {
        var directoryInfo = new DirectoryInfo(directory);
        var fileVersion = versionRegex.Match(directoryInfo.Name).Value;
        var files = new Files($@"{directory}\*.*");
        if (versionStorages.ContainsKey(fileVersion))
            versionStorages[fileVersion].Add(files);
        else
            versionStorages.Add(fileVersion, new List<WixEntity> { files });

        var assemblies = Directory.GetFiles(directory, "*", SearchOption.AllDirectories);
        Console.WriteLine($"Added '{fileVersion}' version files: ");
        foreach (var assembly in assemblies) Console.WriteLine($"'{assembly}'");
    }

    return versionStorages.Select(storage => new Dir(storage.Key, storage.Value.ToArray())).Cast<WixEntity>().ToArray();
}
