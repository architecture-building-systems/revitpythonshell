[Files]
Source: "RevitPythonShell\bin\Release\2021\PythonConsoleControl.dll"; DestDir: "{app}"; Flags: replacesameversion
Source: "RevitPythonShell\bin\Release\2021\RevitPythonShell.dll"; DestDir: "{app}"; Flags: replacesameversion
Source: "RevitPythonShell\bin\Release\2021\RpsRuntime.dll"; DestDir: "{app}"; Flags: replacesameversion
Source: "RevitPythonShell\bin\Release\2021\RevitPythonShell.addin"; DestDir: "{userappdata}\Autodesk\Revit\Addins\2021"; Flags: replacesameversion
Source: "RevitPythonShell\bin\Release\2021\ICSharpCode.AvalonEdit.dll"; DestDir: "{app}"
Source: "RevitPythonShell\bin\Release\2021\IronPython.dll"; DestDir: "{app}"
Source: "RevitPythonShell\bin\Release\2021\IronPython.Modules.dll"; DestDir: "{app}"
Source: "RevitPythonShell\bin\Release\2021\Microsoft.Scripting.Metadata.dll"; DestDir: "{app}"
Source: "RevitPythonShell\bin\Release\2021\Microsoft.Dynamic.dll"; DestDir: "{app}"
Source: "RevitPythonShell\bin\Release\2021\Microsoft.Scripting.dll"; DestDir: "{app}"
Source: "RevitPythonShell\bin\Release\2021\DefaultConfig\RevitPythonShell.xml"; DestDir: "{userappdata}\RevitPythonShell\2021"; Flags: onlyifdoesntexist
Source: "RevitPythonShell\bin\Release\2021\DefaultConfig\init.py"; DestDir: {userappdata}\RevitPythonShell\2021; Flags: confirmoverwrite; 
Source: "RevitPythonShell\bin\Release\2021\DefaultConfig\startup.py"; DestDir: {userappdata}\RevitPythonShell\2021; Flags: confirmoverwrite; 


[code]
{ HANDLE INSTALL PROCESS STEPS }
procedure CurStepChanged(CurStep: TSetupStep);
var
  AddInFilePath: String;
  LoadedFile : TStrings;
  AddInFileContents: String;
  ReplaceString: String;
  SearchString: String;
begin

  if CurStep = ssPostInstall then
  begin

  AddinFilePath := ExpandConstant('{userappdata}\Autodesk\Revit\Addins\2021\RevitPythonShell.addin');
  LoadedFile := TStringList.Create;
  SearchString := 'Assembly>RevitPythonShell.dll<';
  ReplaceString := 'Assembly>' + ExpandConstant('{app}') + '\RevitPythonShell.dll<';

  try
      LoadedFile.LoadFromFile(AddInFilePath);
      AddInFileContents := LoadedFile.Text;

      { Only save if text has been changed. }
      if StringChangeEx(AddInFileContents, SearchString, ReplaceString, True) > 0 then
      begin;
        LoadedFile.Text := AddInFileContents;
        LoadedFile.SaveToFile(AddInFilePath);
      end;
    finally
      LoadedFile.Free;
    end;

  end;
end;

[Setup]
AppName=RevitPythonShell for Autodesk Revit 2021
AppVerName=RevitPythonShell for Autodesk Revit 2021
RestartIfNeededByRun=false
DefaultDirName={pf32}\RevitPythonShell\2021
OutputBaseFilename=Setup_RevitPythonShell_2021
ShowLanguageDialog=auto
FlatComponentsList=false
UninstallFilesDir={app}\Uninstall
UninstallDisplayName=RevitPythonShell for Autodesk Revit 2021
AppVersion=2021.0
VersionInfoVersion=2021.0
VersionInfoDescription=RevitPythonShell for Autodesk Revit 2021
VersionInfoTextVersion=RevitPythonShell for Autodesk Revit 2021
