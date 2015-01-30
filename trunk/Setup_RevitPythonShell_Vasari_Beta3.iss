[Files]
Source: RevitPythonShell\bin\Debug 2014\PythonConsoleControl.dll; DestDir: {app};
Source: RevitPythonShell\bin\Debug 2014\RevitPythonShell.dll; DestDir: {app};
Source: RevitPythonShell\bin\Debug 2014\RpsRuntime.dll; DestDir: {app};
Source: RequiredLibraries\ICSharpCode.AvalonEdit.dll; DestDir: {app};
Source: RequiredLibraries\IronPython.dll; DestDir: {app};
Source: RequiredLibraries\IronPython.Modules.dll; DestDir: {app};
Source: RequiredLibraries\Microsoft.Scripting.Metadata.dll; DestDir: {app};
Source: RequiredLibraries\Microsoft.Dynamic.dll; DestDir: {app};
Source: RequiredLibraries\Microsoft.Scripting.dll; DestDir: {app};
Source: RevitPythonShell\RevitPythonShell.xml; DestDir: {userappdata}\RevitPythonShell_Vasari; Flags: onlyifdoesntexist; 
Source: "RevitPythonShell\init.py"; DestDir: "{userappdata}\RevitPythonShell_Vasari"; Flags: onlyifdoesntexist
Source: "RevitPythonShell\startup.py"; DestDir: "{userappdata}\RevitPythonShell_Vasari"; Flags: onlyifdoesntexist


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
	AddInFilePath := ExpandConstant('{userappdata}\Autodesk\Vasari\Addins\2014\RevitPythonShell_Vasari.addin');

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
AppName=RevitPythonShell for Vasari Beta3
AppVerName=RevitPythonShell for Vasari Beta3
RestartIfNeededByRun=false
DefaultDirName={pf32}\RevitPythonShell_for_Vasari_Beta3
OutputBaseFilename=Setup_RevitPythonShell_Vasari_Beta3
ShowLanguageDialog=auto
FlatComponentsList=false
UninstallFilesDir={app}\Uninstall
UninstallDisplayName=RevitPythonShell for Vasari Beta3
AppVersion=2014.1
VersionInfoVersion=2014.1
VersionInfoDescription=RevitPythonShell for Vasari Beta3
VersionInfoTextVersion=RevitPythonShell for Vasari Beta3

