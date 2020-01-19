[Files]
Source: "RevitPythonShell\bin\Release\2014\PythonConsoleControl.dll"; DestDir: "{app}"; Flags: replacesameversion
Source: "RevitPythonShell\bin\Release\2014\RevitPythonShell.dll"; DestDir: "{app}"; Flags: replacesameversion
Source: "RevitPythonShell\bin\Release\2014\RpsRuntime.dll"; DestDir: "{app}"; Flags: replacesameversion
Source: "RevitPythonShell\bin\Release\2014\RevitPythonShell.addin"; DestDir: "{userappdata}\Autodesk\Revit\Addins\2014"; Flags: replacesameversion
Source: "RevitPythonShell\bin\Release\2014\ICSharpCode.AvalonEdit.dll"; DestDir: "{app}"
Source: "RevitPythonShell\bin\Release\2014\IronPython.dll"; DestDir: "{app}"
Source: "RevitPythonShell\bin\Release\2014\IronPython.Modules.dll"; DestDir: "{app}"
Source: "RevitPythonShell\bin\Release\2014\Microsoft.Scripting.Metadata.dll"; DestDir: "{app}"
Source: "RevitPythonShell\bin\Release\2014\Microsoft.Dynamic.dll"; DestDir: "{app}"
Source: "RevitPythonShell\bin\Release\2014\Microsoft.Scripting.dll"; DestDir: "{app}"
Source: "RevitPythonShell\bin\Release\2014\DefaultConfig\RevitPythonShell.xml"; DestDir: "{userappdata}\RevitPythonShell\2014"; Flags: onlyifdoesntexist
Source: "RevitPythonShell\bin\Release\2014\DefaultConfig\init.py"; DestDir: {userappdata}\RevitPythonShell\2014; Flags: confirmoverwrite; 
Source: "RevitPythonShell\bin\Release\2014\DefaultConfig\startup.py"; DestDir: {userappdata}\RevitPythonShell\2014; Flags: confirmoverwrite; 


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

  AddinFilePath := ExpandConstant('{userappdata}\Autodesk\Revit\Addins\2014\RevitPythonShell.addin');
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
AppName=RevitPythonShell for Autodesk Revit 2014
AppVerName=RevitPythonShell for Autodesk Revit 2014
RestartIfNeededByRun=false
DefaultDirName={pf32}\RevitPythonShell\2014
OutputBaseFilename=Setup_RevitPythonShell_2014
ShowLanguageDialog=auto
FlatComponentsList=false
UninstallFilesDir={app}\Uninstall
UninstallDisplayName=RevitPythonShell for Autodesk Revit 2014
AppVersion=2014.0
VersionInfoVersion=2014.0
VersionInfoDescription=RevitPythonShell for Autodesk Revit 2014
VersionInfoTextVersion=RevitPythonShell for Autodesk Revit 2014
