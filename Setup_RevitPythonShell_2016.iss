[Files]
Source: "RevitPythonShell\bin\Release\2016\PythonConsoleControl.dll"; DestDir: "{app}"; Flags: replacesameversion
Source: "RevitPythonShell\bin\Release\2016\RevitPythonShell.dll"; DestDir: "{app}"; Flags: replacesameversion
Source: "RevitPythonShell\bin\Release\2016\RpsRuntime.dll"; DestDir: "{app}"; Flags: replacesameversion
Source: "RevitPythonShell\bin\Release\2016\RevitPythonShell.addin"; DestDir: "{userappdata}\Autodesk\Revit\Addins\2016"; Flags: replacesameversion
Source: "RevitPythonShell\bin\Release\2016\DefaultConfig\RevitPythonShell.xml"; DestDir: "{userappdata}\RevitPythonShell\2016"; Flags: onlyifdoesntexist
Source: "RevitPythonShell\bin\Release\2016\DefaultConfig\init.py"; DestDir: {userappdata}\RevitPythonShell\2016; Flags: confirmoverwrite; 
Source: "RevitPythonShell\bin\Release\2016\DefaultConfig\startup.py"; DestDir: {userappdata}\RevitPythonShell\2016; Flags: confirmoverwrite; 


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

  AddinFilePath := ExpandConstant('{userappdata}\Autodesk\Revit\Addins\2016\RevitPythonShell.addin');
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
AppName=RevitPythonShell for Autodesk Revit 2016
AppVerName=RevitPythonShell for Autodesk Revit 2016
RestartIfNeededByRun=false
DefaultDirName={pf32}\RevitPythonShell\2016
OutputBaseFilename=Setup_RevitPythonShell_2016
ShowLanguageDialog=auto
FlatComponentsList=false
UninstallFilesDir={app}\Uninstall
UninstallDisplayName=RevitPythonShell for Autodesk Revit 2016
AppVersion=2016.0
VersionInfoVersion=2016.0
VersionInfoDescription=RevitPythonShell for Autodesk Revit 2016
VersionInfoTextVersion=RevitPythonShell for Autodesk Revit 2016
