[Files]
Source: "RevitPythonShell\bin\Release\2019\PythonConsoleControl.dll"; DestDir: "{app}"; Flags: replacesameversion
Source: "RevitPythonShell\bin\Release\2019\RevitPythonShell.dll"; DestDir: "{app}"; Flags: replacesameversion
Source: "RevitPythonShell\bin\Release\2019\RpsRuntime.dll"; DestDir: "{app}"; Flags: replacesameversion
Source: "RevitPythonShell\bin\Release\2019\RevitPythonShell.addin"; DestDir: "{userappdata}\Autodesk\Revit\Addins\2019"; Flags: replacesameversion
Source: "RevitPythonShell\bin\Release\2019\DefaultConfig\RevitPythonShell.xml"; DestDir: "{userappdata}\RevitPythonShell\2019"; Flags: onlyifdoesntexist
Source: "RevitPythonShell\bin\Release\2019\DefaultConfig\init.py"; DestDir: {userappdata}\RevitPythonShell\2019; Flags: confirmoverwrite; 
Source: "RevitPythonShell\bin\Release\2019\DefaultConfig\startup.py"; DestDir: {userappdata}\RevitPythonShell\2019; Flags: confirmoverwrite; 


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

  AddinFilePath := ExpandConstant('{userappdata}\Autodesk\Revit\Addins\2019\RevitPythonShell.addin');
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
AppName=RevitPythonShell for Autodesk Revit 2019
AppVerName=RevitPythonShell for Autodesk Revit 2019
RestartIfNeededByRun=false
DefaultDirName={pf32}\RevitPythonShell\2019
OutputBaseFilename=Setup_RevitPythonShell_2019
ShowLanguageDialog=auto
FlatComponentsList=false
UninstallFilesDir={app}\Uninstall
UninstallDisplayName=RevitPythonShell for Autodesk Revit 2019
AppVersion=2019.0
VersionInfoVersion=2019.0
VersionInfoDescription=RevitPythonShell for Autodesk Revit 2019
VersionInfoTextVersion=RevitPythonShell for Autodesk Revit 2019
