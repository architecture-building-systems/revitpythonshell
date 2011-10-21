[Files]
Source: C:\projects\revitpythonshell\RevitPythonShell\bin\Debug\ICSharpCode.AvalonEdit.dll; DestDir: {app};
Source: C:\projects\revitpythonshell\RevitPythonShell\bin\Debug\IronPython.dll; DestDir: {app};
Source: C:\projects\revitpythonshell\RevitPythonShell\bin\Debug\IronPython.Modules.dll; DestDir: {app};
Source: C:\projects\revitpythonshell\RevitPythonShell\bin\Debug\PythonConsoleControl.dll; DestDir: {app};
Source: C:\projects\revitpythonshell\RevitPythonShell\bin\Debug\RevitPythonShell.dll; DestDir: {app};
Source: "C:\Program Files (x86)\IronPython 2.7\Microsoft.Scripting.Metadata.dll"; DestDir: {app};
Source: "C:\Program Files (x86)\IronPython 2.7\Microsoft.Dynamic.dll"; DestDir: {app};
Source: "C:\Program Files (x86)\IronPython 2.7\Microsoft.Scripting.dll"; DestDir: {app};
Source: C:\projects\revitpythonshell\RevitPythonShell\RevitPythonShell.xml; DestDir: {userappdata}\RevitPythonShell; Flags: onlyifdoesntexist;

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
	AddInFilePath := ExpandConstant('{userappdata}\Autodesk\Vasari\Addins\TP2.1\RevitPythonShell2012.addin');

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
AppName=RevitPythonShell2012
AppVerName=2012
RestartIfNeededByRun=false
DefaultDirName={pf32}\RevitPythonShell2012
OutputBaseFilename=Setup_RevitPythonShell_Vasari_TP2.1
ShowLanguageDialog=auto
FlatComponentsList=false
UninstallFilesDir={app}\Uninstall
UninstallDisplayName=RevitPythonShell2012
AppVersion=2012
VersionInfoVersion=2012.0
VersionInfoDescription=RevitPythonShell2012
VersionInfoTextVersion=RevitPythonShell2012

