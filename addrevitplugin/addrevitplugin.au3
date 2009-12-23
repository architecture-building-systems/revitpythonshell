; =============================================================================
; addrevitplugin
; 
; adds a plugin to Autodesk Revit Architecture 2010 by altering the Revit.ini 
; file.
; =============================================================================

; you might want to set this to another GUI, if you are not using
; Revit Architecture 2010 - see: http://thebuildingcoder.typepad.com/blog/2009/02/revit-install-path-and-product-guids.html
Const $guid = '{572FBF5D-3BAA-42ff-A468-A54C2C0A17C3}'


; retrieve the commandline arguments
If $CmdLine[0] == 2 Then
	
	$class = $CmdLine[1]
	$path = $CmdLine[2]
	AddOrUpdatePlugin($class, $path)
	
ElseIf $CmdLine[0] == 1 Then
	
	$class = $CmdLine[1]
	DeletePlugin($class)
	
Else

	; print usage and exit
	$message = 'usage: ' & @ScriptName & ' EAClassName [EAAssembly]'	
	MsgBox(0, "addrevitplugin", $message, 10)	

EndIf


; if a plugin is already defined for $class, then 
; just change the path, otherwise add a new plugin 
Func AddOrUpdatePlugin($class, $path)
	
	$inipath = RevitIni()
	$section = "ExternalApplications"
	
	$EACount = Int(IniRead($inipath, $section, "EACount", "0"))
	
	If $EACount > 0 Then
		
		; this fails if $EACount == 0, pity
		Dim $EAAssembly[$EACount]
		Dim $EAClassName[$EACount]
		
	EndIf
	
	; read in current values
	For $i = 0 To $EACount - 1
		
		$EAIndex = $i + 1 ; 1-based in ini-file, 0-based in array...
		$EAAssembly[$i] = IniRead($inipath, $section, "EAAssembly" & $EAIndex, "")
		$EAClassName[$i] = IniRead($inipath, $section, "EAClassName" & $EAIndex, "")
		
	Next
	
	$updated = False ; have we updated anything?
	; write back the values, updating the path if found
	For $i = 0 To $EACount - 1
		
		$EAIndex = $i + 1 ; 1-based in ini-file, 0-based in array...
		IniWrite($inipath, $section, "EAClassName" & $EAIndex, $EAClassName[$i])
		
		If $EAClassName[$i] == $class Then
		
			; update path to new assembly
			IniWrite($inipath, $section, "EAAssembly" & $EAIndex, $path)
			$updated = True

		Else
			
			; leave as is
			IniWrite($inipath, $section, "EAAssembly" & $EAIndex, $EAAssembly[$i])
			
		Endif
		
	Next
	
	; if we haven't updated anything yet, then we must add a new entry
	If Not $updated Then
		
		$EAIndex = $EACount + 1;
		IniWrite($inipath, $section, "EAClassName" & $EAIndex, $class)
		IniWrite($inipath, $section, "EAAssembly" & $EAIndex, $path)
		IniWrite($inipath, $section, "EACount", $EACount + 1)
		
	EndIf
	
EndFunc

; if a plugin is found with the class, delete it
Func DeletePlugin($class)
	
	$inipath = RevitIni()
	$section = "ExternalApplications"
	
	$EACount = Int(IniRead($inipath, $section, "EACount", "0"))
	If $EACount == 0 Then
		
		; no plugins to delete
		MsgBox(0, "addrevitplugin", "No external applications found in Revit.ini")
		Exit(0)
		
	EndIf
		
	Dim $EAAssembly[$EACount]
	Dim $EAClassName[$EACount]
	
	; read in current values
	For $i = 0 To $EACount - 1
		
		$EAIndex = $i + 1 ; 1-based in ini-file, 0-based in array...
		$EAAssembly[$i] = IniRead($inipath, $section, "EAAssembly" & $EAIndex, "")
		$EAClassName[$i] = IniRead($inipath, $section, "EAClassName" & $EAIndex, "")
		
	Next
		
	; delete current values
	IniWriteSection($inipath, $section, "")
	
	; write new values (skip deleted plugin)
	$EAIndex = 1
	For $i = 0 To $EACount - 1
		
		If $EAClassName[$i] <> $class && $EAClassName[$i] <> "" Then
			
			IniWrite($inipath, $section, "EAAssembly" & $EAIndex, $EAAssembly[$i])
			IniWrite($inipath, $section, "EAClassName" & $EAIndex, $EAClassName[$i])
			$EAIndex = $EAIndex + 1
			
		EndIf
		
	Next
	; new count of external assemblies
	IniWrite($inipath, $section, "EACount", $EAIndex - 1)
	
EndFunc

; finds the path to the Revit.ini file and returns it
Func RevitIni()
	
	; the location of the Autodesk Revit Architecture 2010 installation in is here
	$regpath = "HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\" & $guid
	$installpath = RegRead($regpath, "InstallLocation")
	If @error <> 0 Then
		
		$message = "maybe you are using a different version (e.g. not Autodesk Revit Architecture 2010 [32bit])?"
		MsgBox(48, "addrevitplugin", "Could not find registry key: " & $regpath & @CRLF & $message)
		Exit(1)
		
	Else
	
		return $installpath & "Program\Revit.ini"
		
	EndIf
	
EndFunc