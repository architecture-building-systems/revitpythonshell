[Files]
Source: "{OUTDIR}PythonConsoleControl.dll"; DestDir: "{app}"; Flags: replacesameversion
Source: "{OUTDIR}RevitPythonShell.dll"; DestDir: "{app}"; Flags: replacesameversion
Source: "{OUTDIR}RpsRuntime.dll"; DestDir: "{app}"; Flags: replacesameversion
Source: "{OUTDIR}RevitPythonShell.addin"; DestDir: "{userappdata}\Autodesk\Revit\Addins\{REVIT_VERSION}"; Flags: replacesameversion
Source: "{OUTDIR}ICSharpCode.AvalonEdit.dll"; DestDir: "{app}"
Source: "{OUTDIR}IronPython.dll"; DestDir: "{app}"
Source: "{OUTDIR}IronPython.Modules.dll"; DestDir: "{app}"
Source: "{OUTDIR}Microsoft.Scripting.Metadata.dll"; DestDir: "{app}"
Source: "{OUTDIR}Microsoft.Dynamic.dll"; DestDir: "{app}"
Source: "{OUTDIR}Microsoft.Scripting.dll"; DestDir: "{app}"
Source: "{OUTDIR}DefaultConfig\RevitPythonShell.xml"; DestDir: "{userappdata}\RevitPythonShell\{REVIT_VERSION}"; Flags: onlyifdoesntexist
Source: "{OUTDIR}DefaultConfig\init.py"; DestDir: {userappdata}\RevitPythonShell\{REVIT_VERSION}; Flags: confirmoverwrite; 
Source: "{OUTDIR}DefaultConfig\startup.py"; DestDir: {userappdata}\RevitPythonShell\{REVIT_VERSION}; Flags: confirmoverwrite; 


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

  AddinFilePath := ExpandConstant('{userappdata}\Autodesk\Revit\Addins\{REVIT_VERSION}\RevitPythonShell.addin');
  LoadedFile := TStringList.Create;
  SearchString := '{ASSEMBLY_PATH}';
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
AppName=RevitPythonShell for Autodesk Revit {REVIT_VERSION}
AppVerName=RevitPythonShell for Autodesk Revit {REVIT_VERSION}
RestartIfNeededByRun=false
DefaultDirName={pf32}\RevitPythonShell\{REVIT_VERSION}
OutputBaseFilename=Setup_RevitPythonShell_{REVIT_VERSION}
ShowLanguageDialog=auto
FlatComponentsList=false
UninstallFilesDir={app}\Uninstall
UninstallDisplayName=RevitPythonShell for Autodesk Revit {REVIT_VERSION}
AppVersion={REVIT_VERSION}.0
VersionInfoVersion={REVIT_VERSION}.0
VersionInfoDescription=RevitPythonShell for Autodesk Revit {REVIT_VERSION}
VersionInfoTextVersion=RevitPythonShell for Autodesk Revit {REVIT_VERSION}
