[Files]
Source: "RevitPythonShell\bin\Release\2020\PythonConsoleControl.dll"; DestDir: "{app}"; Flags: replacesameversion
Source: "RevitPythonShell\bin\Release\2020\RevitPythonShell.dll"; DestDir: "{app}"; Flags: replacesameversion
Source: "RevitPythonShell\bin\Release\2020\RpsRuntime.dll"; DestDir: "{app}"; Flags: replacesameversion
Source: "RevitPythonShell\bin\Release\2020\RevitPythonShell.addin"; DestDir: "{userappdata}\Autodesk\Revit\Addins\2020"; Flags: replacesameversion
Source: "RevitPythonShell\bin\Release\2020\ICSharpCode.AvalonEdit.dll"; DestDir: "{app}"
Source: "RevitPythonShell\bin\Release\2020\IronPython.dll"; DestDir: "{app}"
Source: "RevitPythonShell\bin\Release\2020\IronPython.Modules.dll"; DestDir: "{app}"
Source: "RevitPythonShell\bin\Release\2020\Microsoft.Scripting.Metadata.dll"; DestDir: "{app}"
Source: "RevitPythonShell\bin\Release\2020\Microsoft.Dynamic.dll"; DestDir: "{app}"
Source: "RevitPythonShell\bin\Release\2020\Microsoft.Scripting.dll"; DestDir: "{app}"
Source: "RevitPythonShell\bin\Release\2020\DefaultConfig\RevitPythonShell.xml"; DestDir: "{userappdata}\RevitPythonShell\2020"; Flags: onlyifdoesntexist
Source: "RevitPythonShell\bin\Release\2020\DefaultConfig\init.py"; DestDir: {userappdata}\RevitPythonShell\2020; Flags: confirmoverwrite; 
Source: "RevitPythonShell\bin\Release\2020\DefaultConfig\startup.py"; DestDir: {userappdata}\RevitPythonShell\2020; Flags: confirmoverwrite; 


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

  AddinFilePath := ExpandConstant('{userappdata}\Autodesk\Revit\Addins\2020\RevitPythonShell.addin');
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
AppName=RevitPythonShell for Autodesk Revit 2020
AppVerName=RevitPythonShell for Autodesk Revit 2020
RestartIfNeededByRun=false
DefaultDirName={pf32}\RevitPythonShell\2020
OutputBaseFilename=Setup_RevitPythonShell_2020
ShowLanguageDialog=auto
FlatComponentsList=false
UninstallFilesDir={app}\Uninstall
UninstallDisplayName=RevitPythonShell for Autodesk Revit 2020
AppVersion=2020.0
VersionInfoVersion=2020.0
VersionInfoDescription=RevitPythonShell for Autodesk Revit 2020
VersionInfoTextVersion=RevitPythonShell for Autodesk Revit 2020
