global modelPlugin
global mainObject
global mainMesh
global averagedNormals
global keys

-- Customize -> Preferences -> General -> Ui Display Group -> Uncheck "Display Topology-Depence Warning" checkbox

-- Normals, this is used for testing currently
function CalcAveNorms mObject =
(
	averagedNormals = #()
	for v in 1 to meshop.getnumverts mObject do
	(
		local averageNormal = [0,0,0]
		local faces = #()
		local faceRNormals = #()
		
		local vertexFaces = (meshop.getFacesUsingVert mObject v as array)
		--for f in 1 to $.numfaces do (meshop.unifyNormals mObject f)
		for f in vertexFaces do (append faces (getFace mObject f))
		for f in vertexFaces do (append faceRNormals (getFaceNormal mObject f))--(meshop.getFaceRNormals mObject f))
		for f in 1 to faces.count do
		(
			if faces[f][1] == v then
			(
				averageNormal += faceRNormals[f]--[1]
			)
			else if faces[f][2] == v then
			(
				averageNormal += faceRNormals[f]--[2]
			)
			else if faces[f][3] == v then
			(
				averageNormal += faceRNormals[f]--[3]
			)
		)
		
		averageNormal = normalize (averageNormal / faces.count)
		averagedNormals[v] = averageNormal
		--setNormal mObject v averageNormal
	)
)

-- BRG ====================================================================================================
function ConvertAnimKeys frm =
(
	if frm < animationRange.start or frm > animationRange.end then
	(
		0.0
	)
	else
	(
		(frm - animationRange.start) as integer / 4800.0
	)
)
function GetModelAnimKeys =
(
	max modify mode
	keys = #()
	if (mainObject.modifiers[#Unwrap_UVW] != undefined and \
		mainObject.Unwrap_UVW[1] != undefined and \
		mainObject.Unwrap_UVW[1].keys != undefined) then
	(
		modPanel.setCurrentObject mainObject.modifiers[#Unwrap_UVW] ui:true
		numTexKeys = mainObject.Unwrap_UVW[1].keys.count
		for i = 1 to numTexKeys do
		(
			appendIfUnique keys (ConvertAnimKeys mainObject.Unwrap_UVW[1].keys[i].time)
		)
	)
	
	if (mainObject.modifiers[#skin] != undefined) then
	(
		modPanel.setCurrentObject mainObject.modifiers[#skin] ui:true
		numBones = skinops.getnumberbones mainObject.modifiers[#skin]
		for i = 1 to numBones do
		(
			boneName = skinops.getbonename mainObject.modifiers[#skin] i 0
			boneNode = getNodeByName boneName
			
			if (boneNode[#transform] != undefined) then
			(
				numBoneKeys = boneNode[#transform].controller.keys.count
				for j = 1 to numBoneKeys do
				(
					appendIfUnique keys (ConvertAnimKeys boneNode[#transform].controller.keys[j].time)
				)
				
				if (boneNode[#transform][#position] != undefined) then
				(
					numBoneKeys = boneNode[#transform][#position].keys.count
					for j = 1 to numBoneKeys do
					(
						appendIfUnique keys (ConvertAnimKeys boneNode[#transform][#position].keys[j].time)
					)
				)
				
				if (boneNode[#transform][#rotation] != undefined) then
				(
					numBoneKeys = boneNode[#transform][#rotation].keys.count
					local rotKeys
					if (numBoneKeys == -1) then
					( 
						numBoneKeys = boneNode[#transform][#rotation][1].keys.count
						rotKeys = boneNode[#transform][#rotation][1].keys
					)
					else rotKeys = boneNode[#transform][#rotation].keys
					for j = 1 to numBoneKeys do
					(
						appendIfUnique keys (ConvertAnimKeys rotKeys[j].time)
					)
				)
				
				if (boneNode[#transform][#scale] != undefined) then
				(
					numBoneKeys = boneNode[#transform][#scale].keys.count
					for j = 1 to numBoneKeys do
					(
						appendIfUnique keys (ConvertAnimKeys boneNode[#transform][#scale].keys[j].time)
					)
				)
			)
			
-- 			numBoneKeys = boneNode.controller.keys.count
-- 			for j = 1 to numBoneKeys do
-- 			(
-- 				appendIfUnique keys ((boneNode.controller.keys[j].time as float) / 4800.0)
-- 			)
		)
	)
	
	numAnimKeys = mainObject.baseobject.mesh[1].keys.count
	for i = 1 to numAnimKeys do
	(
		appendIfUnique keys (ConvertAnimKeys mainObject.baseobject.mesh[1].keys[i].time)
	)
	
	if keys.count <= 0 then
	(
		appendIfUnique keys (ConvertAnimKeys animationRange.start)
	)
	
	--newKeys = #()
	--for i = 1 to keys.count do (if keys[i] >= 0 then append newKeys keys[i])
	--keys = newKeys;
	sort keys
)

function SplitVertsByMapVerts theMesh tim =
(
	averagedNormals = #()
	--first collect two arrays of bitarrays - one containing the map vertices for each mesh vertex,
	--the other containing the faces using each mesh vertex:
	--convertToMesh theMesh 
	lastVert = theMesh.numverts
	lastFace = theMesh.numfaces
	VertsUsage = for v = 1 to lastVert collect #{}
	FaceUsage = for v = 1 to lastVert collect #{}
	for f = 1 to lastFace do
	(
		theFace = getFace theMesh f
		theMapFace = meshop.getMapFace theMesh 1 f
		append VertsUsage[theFace.x] theMapFace.x
		append VertsUsage[theFace.y] theMapFace.y
		append VertsUsage[theFace.z] theMapFace.z
		append FaceUsage [theFace.x] f
		append FaceUsage [theFace.y] f
		append FaceUsage [theFace.z] f
	)
	--Now we can go though the mesh vertex array and see if we have 2 or more map vertices.
	--If yes, we count from 2 to the number of map verts and create new mesh vertices with the
	--same position as the current mesh vertex. Then we go through the list of collected faces
	--using that vertex and see which of the 3 components matches the current vertex and its
	--corresponding map vertex matches the current map vertex. We repoint that component of the face
	--to the new mesh vertex. Thus, for each previously existing map vertex, we get a new mesh vertex
	--at the end of the vertex list. 

	for v = 1 to VertsUsage.count do
	(
		-- first calculate the averageNormal, and set in array
		vertexNormalIds = #{}
		at time tim (mainObject.modifiers[#edit_normals].ConvertVertexSelection #{v} vertexNormalIds)
        averageNormal = [0,0,0]
		at time tim (for n in vertexNormalIds do averageNormal += mainObject.modifiers[#edit_normals].GetNormal n)
		averageNormal = normalize (averageNormal / vertexNormalIds.numberset)
		averagedNormals[v] = averageNormal
		
		theMapVertsArray = VertsUsage[v] as array
		if theMapVertsArray.count > 1 do
		(
			theFacesArray = FaceUsage[v] as array
			theVertPos = getVert theMesh v
			for mapv = 2 to theMapVertsArray.count do
			(
				theMapVert = theMapVertsArray[mapv]
				newIndex = theMesh.numverts+1
				setNumVerts theMesh newIndex true
				setVert theMesh newIndex theVertPos
                averagedNormals[newIndex] = averageNormal -- set the same normal to the copied vertex
				for f in theFacesArray do
				(
					theFaceDef = getFace theMesh f
					theMapFaceDef = meshOp.getMapFace theMesh 1 f
					if theMapFaceDef.x == theMapVert and theFaceDef.x == v do 
					(
						theFaceDef.x = newIndex 
						setFace theMesh f theFaceDef
					)				
					if theMapFaceDef.y == theMapVert and theFaceDef.y == v do 
					(
						theFaceDef.y = newIndex 
						setFace theMesh f theFaceDef
					)				
					if theMapFaceDef.z == theMapVert and theFaceDef.z == v do 
					(
						theFaceDef.z = newIndex 
						setFace theMesh f theFaceDef
					)				
				)--end f loop
			)--end v loop
		)
	)
	--update theMesh
)

function GetExportData tim =
(
	tim = ((tim * FrameRate) as time) -- convert time from seconds to frames
	mainMesh = at time tim (snapshotAsMesh mainObject)
	
	meshop.DeleteIsoVerts mainMesh
	SplitVertsByMapVerts mainMesh tim
)

-- GRN ====================================================================================================
global sceneDummies
global grnMeshes
global grnBones
global grnBoneParents
global grnSkinWeights
global grnSkinBBIndices
global grnSkinBBMaxs
global grnSkinBBMins
function ExportSkinBBData =
(
	grnSkinBBIndices = #()
	grnSkinBBMaxs = #()
	grnSkinBBMins = #()
	--local boneArray = for o in objects where (refs.dependencyLoopTest skinMod o) collect o
	
	local bonesCount = skinops.getnumberbones skinMod
	for boneID = 1 to bonesCount do
	(
		local boneName = (skinOps.GetBoneName skinMod boneID 1)
		local notFound = true
		local i = 0
		while notFound and i < grnBones.count do
		(
			i += 1
			if grnBones[i].name == boneName then notFound=false
		)
		--if notFound then undefined else boneArray[i]
		if not notFound then
		(
			local boneBoxSize = grnBones[i].boxsize / 2
			--local copiedBone = copy grnBones[i]
			--copiedBone.transform *= inverse(copiedBone.parent.transform)
			append grnSkinBBIndices i
			append grnSkinBBMaxs boneBoxSize--copiedBone.max--grnBones[i].max
			append grnSkinBBMins -boneBoxSize--copiedBone.min--grnBones[i].min
			--delete copiedBone
		)
	)
)
function ExportSkinData =
(
	-- get bone info
	grnSkinWeights = #()
	vertexCount = meshop.getnumverts mainObject

	-- loop through all mesh vertices in this mesh
	for i = 1 to vertexCount do
	(
		append grnSkinWeights #()
		-- loop through all bones affecting this vertex
		vertex_bone_count = skinOps.GetVertexWeightCount skinMod i
		for bn = 1 to vertex_bone_count do
		(
			weight = skinops.getvertexweight skinMod i bn
			boneId = skinOps.GetVertexWeightBoneID skinMod i bn
			if weight > 0 then
			(
				append grnSkinWeights[i] [boneId - 1, weight]
			)
		)
	)
	
	ExportSkinBBData()
)
function ExportGrnNode grnNode parentIndex =
(
	local grnNodeClass = classof grnNode
	if grnNodeClass == Dummy or grnNodeClass == BoneGeometry then
	(
		append sceneDummies grnNode
		append grnBones grnNode
		append grnBoneParents parentIndex
	)
	else if grnNodeClass == Editable_Mesh then
	(
		append grnMeshes grnNode
		append grnBones grnNode
		append grnBoneParents parentIndex
	)
	
	--format "%, %, %" grnBones.count grnNode parentIndex
	
	local selfIndex = grnBones.count
	for child in grnNode.children do
	(
		ExportGrnNode child selfIndex--(parentIndex + 1)
	)
)
function ExportGrnData =
(
	sceneDummies = #()
	grnMeshes = #()
	grnBones = #()
	grnBoneParents = #()
	--rootNode = getNodeByName "__Root"
	ExportGrnNode rootNode -1
)
function CalculateAveragedNormals mObject =
(
	averagedNormals = #()
	for v in 1 to (meshop.getnumverts mObject) do
	(
		vertexNormalIds = #{}
		mObject.modifiers[#edit_normals].ConvertVertexSelection #{v} vertexNormalIds
		averageNormal = [0,0,0]
		for n in vertexNormalIds do averageNormal += mObject.modifiers[#edit_normals].GetNormal n
		averageNormal = normalize (averageNormal / vertexNormalIds.numberset)
		averagedNormals[v] = averageNormal
	)
)

(
	ael = (dotnetclass "System.Reflection.Assembly").Load ((dotnetclass "System.IO.File").ReadAllBytes "..\AoMEngineLibrary.dll")
	ampForm = ael.CreateInstance "AoMEngineLibrary.MaxPluginForm"
	--dotNet.setLifeTimeControl ampForm #dotNet
	--thePtr = DotNetObject "System.IntPtr" (windows.getMAXHWND())
	--ampForm.Show ((dotnetclass "System.Windows.Forms.Control").fromHandle thePtr)
	--ampForm.topMost = true
	--ampForm.Show()
	ampForm.ShowModeless()
)