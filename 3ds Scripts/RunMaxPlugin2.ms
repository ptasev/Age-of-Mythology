--DotNet.LoadAssembly "C:\Program Files\Autodesk\3ds Max 2014\AoMEngineLibrary.dll"
(
	ael = (dotnetclass "System.Reflection.Assembly").Load ((dotnetclass "System.IO.File").ReadAllBytes "..\AoMEngineLibrary.dll")
	brgForm = ael.CreateInstance "AoMEngineLibrary.MaxPluginForm"
	--brgForm.topMost = true
	brgForm.Show()
	
	--maxPlugin = ael.CreateInstance "AoMEngineLibrary.MaxPlugin"
	--maxPlugin.dock 
	
	-- ### Create Form ###
	--mainForm = dotNetObject "MaxCustomControls.MaxForm"
	--mainForm.controls.add maxPlugin
	--mainForm.topmost = true
	--mainForm.show()
	
	--hApp = dotNetClass "System.Windows.Forms.Application"
	--hApp.Run mainForm
)