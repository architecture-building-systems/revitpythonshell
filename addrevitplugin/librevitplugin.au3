; =============================================================================
; librevitplugin
; 
; library functions for adding / removing plugins from Autodesk Revit versions
; based on a supplied GUID (see: http://thebuildingcoder.typepad.com/blog/2009/02/revit-install-path-and-product-guids.html)
;
; this is intended to be called by the scripts
;   - rac32plugin.au3
;   - rac64plugin.au3
;   - rme32plugin.au3
;   - rme64plugin.au3
;   - rst32plugin.au3
;   - rst64plugin.au3
; =============================================================================

; if a plugin is already defined for $class, then 
; just change the path, otherwise add a new plugin 
Func AddOrUpdatePlugin($class, $path, $guid)
	
	$inipath = RevitIni($guid)
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
Func DeletePlugin($class, $guid)
	
	$inipath = RevitIni($guid)
	$section = "ExternalApplications"
	
	$EACount = Int(IniRead($inipath, $section, "EACount", "0"))
	If $EACount == 0 Then
		
		; no plugins to delete
		MsgBox(0, "librevitplugin", "No external applications found in Revit.ini")
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
		
		If $EAClassName[$i] <> $class And $EAClassName[$i] <> "" Then
			
			IniWrite($inipath, $section, "EAAssembly" & $EAIndex, $EAAssembly[$i])
			IniWrite($inipath, $section, "EAClassName" & $EAIndex, $EAClassName[$i])
			$EAIndex = $EAIndex + 1
			
		EndIf
		
	Next
	; new count of external assemblies
	IniWrite($inipath, $section, "EACount", $EAIndex - 1)
	
EndFunc

; finds the path to the Revit.ini file and returns it
Func RevitIni($guid)
	
	; the location of the Autodesk Revit Architecture 2010 installation in is here
	$regpath = "HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\" & $guid
	$installpath = RegRead($regpath, "InstallLocation")
	If @error <> 0 Then
		
		$message = "maybe you are using a different version of Revit?"
		MsgBox(48, "librevitplugin", "Could not find registry key: " & $regpath & @CRLF & $message)
		Exit(1)
		
	Else
	
		return $installpath & "Program\Revit.ini"
		
	EndIf
	
EndFunc