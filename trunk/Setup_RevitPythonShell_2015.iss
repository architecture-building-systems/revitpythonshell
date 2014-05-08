[Files]
Source: RevitPythonShell\bin\Debug\PythonConsoleControl.dll; DestDir: {app};
Source: RevitPythonShell\bin\Debug\RevitPythonShell.dll; DestDir: {app};
Source: RevitPythonShell\bin\Debug\RpsRuntime.dll; DestDir: {app};
Source: RequiredLibraries\ICSharpCode.AvalonEdit.dll; DestDir: {app};
Source: RequiredLibraries\IronPython.dll; DestDir: {app};
Source: RequiredLibraries\IronPython.Modules.dll; DestDir: {app};
Source: RequiredLibraries\Microsoft.Scripting.Metadata.dll; DestDir: {app};
Source: RequiredLibraries\Microsoft.Dynamic.dll; DestDir: {app};
Source: RequiredLibraries\Microsoft.Scripting.dll; DestDir: {app};
Source: RevitPythonShell\RevitPythonShell.xml; DestDir: {userappdata}\RevitPythonShell2015; Flags: onlyifdoesntexist; 

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
	AddInFilePath := ExpandConstant('{userappdata}\Autodesk\Revit\Addins\2015\RevitPythonShell2015.addin');

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
AppName=RevitPythonShell for Autodesk Revit 2015
AppVerName=RevitPythonShell for Autodesk Revit 2015
RestartIfNeededByRun=false
DefaultDirName={pf32}\RevitPythonShell2015
OutputBaseFilename=Setup_RevitPythonShell_2015
ShowLanguageDialog=auto
FlatComponentsList=false
UninstallFilesDir={app}\Uninstall
UninstallDisplayName=RevitPythonShell for Autodesk Revit 2015
AppVersion=2015.0
VersionInfoVersion=2015.0
VersionInfoDescription=RevitPythonShell for Autodesk Revit 2015
VersionInfoTextVersion=RevitPythonShell for Autodesk Revit 2015

