; =============================================================================
; addrevitplugin
; 
; adds a plugin to Autodesk Revit Architecture 2010 64 bit by altering the Revit.ini 
; file.
; =============================================================================

#include "librevitplugin.au3"

; Revit Architecture 2010 64 bit - see: http://thebuildingcoder.typepad.com/blog/2009/02/revit-install-path-and-product-guids.html
Const $guid = '{2A8EEE2F-4A9E-43d8-AA07-EC8A316B2DEB}'

; retrieve the commandline arguments
If $CmdLine[0] == 2 Then
	
	$class = $CmdLine[1]
	$path = $CmdLine[2]
	AddOrUpdatePlugin($class, $path, $guid)
	
ElseIf $CmdLine[0] == 1 Then
	
	$class = $CmdLine[1]
	DeletePlugin($class, $guid)
	
Else

	; print usage and exit
	$message = 'usage: ' & @ScriptName & ' EAClassName [EAAssembly]'	
	MsgBox(0, "librevitplugin", $message, 10)	

EndIf