DotNet.LoadAssembly "C:\Program Files\Autodesk\3ds Max 2014\AoMEngineLibrary.dll"

--[This is the rollout which actually houses the .NET control itself, which is accessible because we loaded the assembly on the line above.]
rollout r_MaxScriptIntegration ".NET Integration" width:300 height:400
(
--[This is how you instantiate the .NET control using MaxScript. It is a fully qualified name, with “Namespace.Name”]
local dotnet_object = "AoMEngineLibrary.MaxPlugin"
local custom_control = DotNetObject dotnet_object
DotNetControl testObj "AoMEngineLibrary.MaxPlugin"

--[Now create a .NET panel on the rollout to contain the .NET control instantiated above. A panel is a control provided by the .NET framework.]
--DotNetControl dnc_dotNetPanel "System.Windows.Forms.Panel" width:290 height:360
--[Do some work to hook up events from our .NET control to the functions declared at the top of this code.]
on r_MaxScriptIntegration open do
(

--[This line adds our custom .NET control to the panel control. Every control has a .Controls property, which is a list of child controls, and we can add or remove other controls from this list.]
--dnc_dotNetPanel.Controls.Add custom_control
)
)

createDialog r_MaxScriptIntegration; 