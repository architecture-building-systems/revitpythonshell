[Files]
Source: "RevitPythonShell\bin\Debug 2016\PythonConsoleControl.dll"; DestDir: "{app}"; Flags: replacesameversion
Source: "RevitPythonShell\bin\Debug 2016\RevitPythonShell.dll"; DestDir: "{app}"; Flags: replacesameversion
Source: "RevitPythonShell\bin\Debug 2016\RpsRuntime.dll"; DestDir: "{app}"; Flags: replacesameversion
Source: "RequiredLibraries\ICSharpCode.AvalonEdit.dll"; DestDir: "{app}"
Source: "RequiredLibraries\IronPython.dll"; DestDir: "{app}"
Source: "RequiredLibraries\IronPython.Modules.dll"; DestDir: "{app}"
Source: "RequiredLibraries\Microsoft.Scripting.Metadata.dll"; DestDir: "{app}"
Source: "RequiredLibraries\Microsoft.Dynamic.dll"; DestDir: "{app}"
Source: "RequiredLibraries\Microsoft.Scripting.dll"; DestDir: "{app}"
Source: "RevitPythonShell\DefaultConfig\RevitPythonShell.xml"; DestDir: "{userappdata}\RevitPythonShell2016"; Flags: onlyifdoesntexist
Source: RevitPythonShell\DefaultConfig\init.py; DestDir: {userappdata}\RevitPythonShell2016; Flags: confirmoverwrite; 
Source: RevitPythonShell\DefaultConfig\startup.py; DestDir: {userappdata}\RevitPythonShell2016; Flags: confirmoverwrite; 

[code]
{ HANDLE INSTALL PROCESS STEPS }
procedure CurStepChanged(CurStep: TSetupStep);
var
  AddInFilePath: String;
  AddInFileContents: String;
begin

  if CurStep = ssPostInstall then
  begin

	{ GET LOCATION OF USER AppData (Roaming) }
	AddInFilePath := ExpandConstant('{userappdata}\Autodesk\Revit\Addins\2016\RevitPythonShell2016.addin');

	{ CREATE NEW ADDIN FILE }
	AddInFileContents := '<?xml version="1.0" encoding="utf-16" standalone="no"?>' + #13#10;
	AddInFileContents := AddInFileContents + '<RevitAddIns>' + #13#10;
	AddInFileContents := AddInFileContents + '  <AddIn Type="Application">' + #13#10;
  AddInFileContents := AddInFileContents + '    <Name>RevitPythonShell</Name>' + #13#10;
	AddInFileContents := AddInFileContents + '    <Assembly>'  + ExpandConstant('{app}') + '\RevitPythonShell.dll</Assembly>' + #13#10;
	AddInFileContents := AddInFileContents + '    <AddInId>3a7a1d24-51ed-462b-949f-1ddcca12008d</AddInId>' + #13#10;
	AddInFileContents := AddInFileContents + '    <FullClassName>RevitPythonShell.RevitPythonShellApplication</FullClassName>' + #13#10;
	AddInFileContents := AddInFileContents + '  <VendorId>RIPS</VendorId>' + #13#10;
	AddInFileContents := AddInFileContents + '  </AddIn>' + #13#10;
	AddInFileContents := AddInFileContents + '</RevitAddIns>' + #13#10;
	SaveStringToFile(AddInFilePath, AddInFileContents, False);

  end;
end;


[Setup]
AppName=RevitPythonShell for Autodesk Revit 2016
AppVerName=RevitPythonShell for Autodesk Revit 2016
RestartIfNeededByRun=false
DefaultDirName={pf32}\RevitPythonShell2016
OutputBaseFilename=Setup_RevitPythonShell_2016
ShowLanguageDialog=auto
FlatComponentsList=false
UninstallFilesDir={app}\Uninstall
UninstallDisplayName=RevitPythonShell for Autodesk Revit 2016
AppVersion=2016.0
VersionInfoVersion=2016.0
VersionInfoDescription=RevitPythonShell for Autodesk Revit 2016
VersionInfoTextVersion=RevitPythonShell for Autodesk Revit 2016
