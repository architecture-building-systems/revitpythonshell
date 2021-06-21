[Files]
Source: "RevitPythonShell\bin\Release\2022\PythonConsoleControl.dll"; DestDir: "{app}"; Flags: replacesameversion
Source: "RevitPythonShell\bin\Release\2022\RevitPythonShell.dll"; DestDir: "{app}"; Flags: replacesameversion
Source: "RevitPythonShell\bin\Release\2022\RpsRuntime.dll"; DestDir: "{app}"; Flags: replacesameversion
Source: "RevitPythonShell\bin\Release\2022\RevitPythonShell.addin"; DestDir: "{userappdata}\Autodesk\Revit\Addins\2022"; Flags: replacesameversion
Source: "RevitPythonShell\bin\Release\2022\ICSharpCode.AvalonEdit.dll"; DestDir: "{app}"
Source: "RevitPythonShell\bin\Release\2022\IronPython.dll"; DestDir: "{app}"
Source: "RevitPythonShell\bin\Release\2022\IronPython.Modules.dll"; DestDir: "{app}"
Source: "RevitPythonShell\bin\Release\2022\Microsoft.Scripting.Metadata.dll"; DestDir: "{app}"
Source: "RevitPythonShell\bin\Release\2022\Microsoft.Dynamic.dll"; DestDir: "{app}"
Source: "RevitPythonShell\bin\Release\2022\Microsoft.Scripting.dll"; DestDir: "{app}"
Source: "RevitPythonShell\bin\Release\2022\DefaultConfig\RevitPythonShell.xml"; DestDir: "{userappdata}\RevitPythonShell\2022"; Flags: onlyifdoesntexist
Source: "RevitPythonShell\bin\Release\2022\DefaultConfig\init.py"; DestDir: {userappdata}\RevitPythonShell\2022; Flags: confirmoverwrite; 
Source: "RevitPythonShell\bin\Release\2022\DefaultConfig\startup.py"; DestDir: {userappdata}\RevitPythonShell\2022; Flags: confirmoverwrite; 


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

  AddinFilePath := ExpandConstant('{userappdata}\Autodesk\Revit\Addins\2022\RevitPythonShell.addin');
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
AppName=RevitPythonShell for Autodesk Revit 2022
AppVerName=RevitPythonShell for Autodesk Revit 2022
RestartIfNeededByRun=false
DefaultDirName={pf32}\RevitPythonShell\2022
OutputBaseFilename=Setup_RevitPythonShell_2022
ShowLanguageDialog=auto
FlatComponentsList=false
UninstallFilesDir={app}\Uninstall
UninstallDisplayName=RevitPythonShell for Autodesk Revit 2022
AppVersion=2022.0
VersionInfoVersion=2022.0
VersionInfoDescription=RevitPythonShell for Autodesk Revit 2022
VersionInfoTextVersion=RevitPythonShell for Autodesk Revit 2022
