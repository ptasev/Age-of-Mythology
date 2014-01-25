(
	ael = (dotnetclass "System.Reflection.Assembly").Load ((dotnetclass "System.IO.File").ReadAllBytes "..\AoMEngineLibrary.dll")
	brgForm = ael.CreateInstance "AoMEngineLibrary.MaxPluginForm"
	--brgForm.topMost = true
	brgForm.Show()
)