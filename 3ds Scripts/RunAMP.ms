global modelPlugin
global mainObject
global mainMesh
global averagedNormals
global keys

-- Customize -> Preferences -> General -> Ui Display Group -> Uncheck "Display Topology-Depence Warning" checkbox
global meshSetVertFunc = meshOp.setVert
global meshGetVertFunc = meshOp.getVert
global meshGetMapVertFunc = meshop.getmapvert
global skinGetBoneNameFunc = skinOps.GetBoneName
global skinGetVertWeightCountFunc = skinOps.GetVertexWeightCount
global skinGetVertWeightFunc = skinops.getvertexweight
global skinGetVertWeightBoneIDFunc = skinOps.GetVertexWeightBoneID
global skinGetNumVertsFunc = skinOps.GetNumberVertices

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
		for f in vertexFaces do (append faceRNormals (meshop.getFaceRNormals mObject f))--(getFaceNormal mObject f))--(meshop.getFaceRNormals mObject f))
		for f in 1 to faces.count do
		(
			if faces[f][1] == v then
			(
				averageNormal += faceRNormals[f][1]
			)
			else if faces[f][2] == v then
			(
				averageNormal += faceRNormals[f][2]
			)
			else if faces[f][3] == v then
			(
				averageNormal += faceRNormals[f][3]
			)
		)
		
		averageNormal = normalize (averageNormal / faces.count)
		averagedNormals[v] = averageNormal
		--setNormal mObject v averageNormal
	)
)

function ConvertAnimKeys frm =
(
	if frm < animationRange.start or frm > animationRange.end then
	(
		-1.0
	)
	else
	(
		(frm - animationRange.start) as integer / 4800.0
	)
)

function GetNodeAnimKeyTimes theSubAnim =
(
	-- loop to the number of sub animation tracks (.numsubs)
	for subIndx = 1 to theSubAnim.numSubs do
	(
		local childSubAnim = getSubAnim theSubAnim subIndx
		
		if childSubAnim.controller != undefined then
		(
			local controllerKeys = childSubAnim.controller.keys
			local controllerKeysCount = controllerKeys.count
			for ki = 1 to controllerKeysCount do
			(
				appendIfUnique keys (ConvertAnimKeys controllerKeys[ki].time)
			)
		)
		
		GetNodeAnimKeyTimes childSubAnim
	)
)

-- BRG ====================================================================================================
global brgMeshes
global brgtotalNumVerts
global brgtotalNumFaces
global brgVertIndices
global brgVertTVIndices
global brgFaceArray

function ExportBrgVertNormals tim =
(
	local origNumMeshVerts = mainObject.numverts
	local numMeshVerts = brgVertIndices.count
	averagedNormals = #()
	averagedNormals.count = numMeshVerts
	
	-- create a temporary object around the mesh that has modifiers collapsed into it
	local tempObject = Editable_Mesh()
	tempObject.mesh = mainMesh
	addModifier tempObject (Edit_Normals()) ui:off
	modPanel.setCurrentObject tempObject.modifiers[#edit_normals] ui:true
	
	local convertVertSelFunc = tempObject.modifiers[#edit_normals].ConvertVertexSelection
	local getVertNormalFunc = tempObject.modifiers[#edit_normals].GetNormal
	
	local vertexNormalIds
	local averageNormal
	local vertexIndex
	for v = 1 to numMeshVerts do
	(
		vertexNormalIds = #{}
		averageNormal = [0,0,0]
		vertexIndex = brgVertIndices[v]
		
		at time tim (convertVertSelFunc #{vertexIndex} vertexNormalIds)
		at time tim (for n in vertexNormalIds do averageNormal += getVertNormalFunc n)
		
		averageNormal = normalize (averageNormal / vertexNormalIds.numberset)
		averagedNormals[v] = averageNormal
	)
	
	delete tempObject
)
function GetMeshSnapshotAtTime mObject tim =
(
	tim = ((tim * FrameRate) as time) -- convert time from seconds to frames
	local mMesh =  at time tim (snapshotAsMesh mObject)
	meshop.DeleteIsoVerts mMesh
	mMesh
)

function ExportPreservedTexCoordData mMesh =
(
	local currVertexIndex = 1
	brgVertIndices = #()
	local brgVertTVMap = #()
	brgVertTVIndices = #()
	brgFaceArray = #()
	
	local numFaces = mMesh.numfaces
	local meshGetMapFaceFunc = meshop.getMapFace
	for fi = 1 to numFaces do
	(
		local theFace = getFace mMesh fi
		local theMapFace = meshGetMapFaceFunc mMesh 1 fi
		
		for vi = 1 to 3 do
		(
			local vertexIndex = theFace[vi]
			local tvIndex = theMapFace[vi]
			if (findItem brgVertIndices vertexIndex) > 0 then
			(
				local tvMap = brgVertTVMap[vertexIndex]
				if tvMap[tvIndex] != undefined then
				(
					theFace[vi] = tvMap[tvIndex]
				)
				else
				(
					append brgVertIndices vertexIndex
					append brgVertTVIndices tvIndex
					
					tvMap[tvIndex] = currVertexIndex
					theFace[vi] = currVertexIndex
					
					currVertexIndex += 1
				)
			)
			else
			(
				append brgVertIndices vertexIndex
				append brgVertTVIndices tvIndex
				
				-- update tvmap
				local tvMap = #()
				tvMap[tvIndex] = currVertexIndex
				brgVertTVMap[vertexIndex] = tvMap
				-- update face
				theFace[vi] = currVertexIndex
				
				currVertexIndex += 1
			)
		)
		
		-- add face to array
		append brgFaceArray theFace
	)
	
	mMesh
)
function ExportBrgNode brgNode =
(
	local brgNodeClass = classof brgNode
	
	if brgNodeClass == Editable_Mesh then--or \
		--brgNodeClass == TriMeshGeometry then
	(
		append brgMeshes brgNode
		brgtotalNumVerts += brgNode.numverts
		brgtotalNumFaces += brgNode.numfaces
	)
	
	for child in brgNode.children do
	(
		if (not child.isHidden) then
		(
			ExportBrgNode child
		)
	)
)
function ExportBrgData =
(
	brgMeshes = #()
	brgtotalNumVerts = 0
	brgtotalNumFaces = 0
	keys = #()
	
	ExportBrgNode rootNode
	
	max modify mode
	for brgMesh in brgMeshes do
	(
		--GetBrgMeshKeys brgMesh
		GetNodeAnimKeyTimes brgMesh
		
		-- loop through animatable of brgMesh bones
		if (brgMesh.modifiers[#skin] != undefined) then
		(
			modPanel.setCurrentObject brgMesh.modifiers[#skin] ui:true
			local numBones = skinops.getnumberbones brgMesh.modifiers[#skin]
			for i = 1 to numBones do
			(
				local boneName = skinops.getbonename brgMesh.modifiers[#skin] i 0
				local boneNode = getNodeByName boneName
				
				GetNodeAnimKeyTimes boneNode
			)
		)
	)
	
	if keys.count <= 0 then
	(
		appendIfUnique keys (ConvertAnimKeys animationRange.start)
	)
	
	sort keys
	
	if keys[1] < 0.0 then
	(
		deleteItem keys 1
	)
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
global skinMod

function ExportSkinBBData =
(
	grnSkinBBIndices = #()
	grnSkinBBMaxs = #()
	grnSkinBBMins = #()
	--local boneArray = for o in objects where (refs.dependencyLoopTest skinMod o) collect o
	
	local bonesCount = skinops.getnumberbones skinMod
	for boneID = 1 to bonesCount do
	(
		modPanel.setCurrentObject skinMod ui:false
		local boneName = (skinGetBoneNameFunc skinMod boneID 1)
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
			--local boneBoxSize = grnBones[i].boxsize / 2
			local copiedBones = #();
			maxops.clonenodes #(grnBones[i]) newNodes:&copiedBones
			copiedBones[1].transform *= inverse(copiedBones[1].transform)
			append grnSkinBBIndices i
			append grnSkinBBMaxs copiedBones[1].max--boneBoxSize--copiedBone.max--grnBones[i].max
			append grnSkinBBMins copiedBones[1].min---boneBoxSize--copiedBone.min--grnBones[i].min
			for obj in copiedBones where isValidNode obj do (delete obj)
		)
	)
)
function ExportSkinData =
(
	-- get bone info
	grnSkinWeights = #()
	local vertexCount = skinGetNumVertsFunc skinMod--meshop.getnumverts mainObject

	-- loop through all mesh vertices in this mesh
	for i = 1 to vertexCount do
	(
		append grnSkinWeights #()
		-- loop through all bones affecting this vertex
		vertex_bone_count = skinGetVertWeightCountFunc skinMod i
		for bn = 1 to vertex_bone_count do
		(
			weight = skinGetVertWeightFunc skinMod i bn
			boneId = skinGetVertWeightBoneIDFunc skinMod i bn
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
	if grnNodeClass == Dummy or \
		grnNodeClass == BoneGeometry or \
		grnNodeClass == Biped_Object or \
		grnNodeClass == Editable_Mesh or \
		grnNodeClass == Editable_Poly or \
		grnNodeClass == Box then
	(
		append sceneDummies grnNode
		append grnBones grnNode
		append grnBoneParents parentIndex
		--format "%\n" grnNode.name
	)
	
	if grnNodeClass == Editable_Mesh and \
		grnNode.modifiers[#skin] != undefined then
	(
		append grnMeshes grnNode
	)
	
	--format "%, %, %" grnBones.count grnNode parentIndex
	
	local selfIndex = grnBones.count
	for child in grnNode.children do
	(
		if (not child.isHidden) then
		(
			ExportGrnNode child selfIndex--(parentIndex + 1)
		)
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

function GetBoneAnimKeys boneNode =
(
	keys = #()
	
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
		
		if (boneNode[#transform][#vertical] != undefined) then
		(
			numBoneKeys = boneNode[#transform][#vertical].keys.count
			for j = 1 to numBoneKeys do
			(
				appendIfUnique keys (ConvertAnimKeys boneNode[#transform][#vertical].keys[j].time)
			)
		)
		
		if (boneNode[#transform][#horizontal] != undefined) then
		(
			numBoneKeys = boneNode[#transform][#horizontal].keys.count
			for j = 1 to numBoneKeys do
			(
				appendIfUnique keys (ConvertAnimKeys boneNode[#transform][#horizontal].keys[j].time)
			)
		)
		
		if (boneNode[#transform][#turning] != undefined) then
		(
			numBoneKeys = boneNode[#transform][#turning].keys.count
			for j = 1 to numBoneKeys do
			(
				appendIfUnique keys (ConvertAnimKeys boneNode[#transform][#turning].keys[j].time)
			)
		)
	)
	
	if (classof boneNode == Editable_Mesh) then
	(
		numAnimKeys = boneNode.baseobject.mesh[1].keys.count
		for i = 1 to numAnimKeys do
		(
			appendIfUnique keys (ConvertAnimKeys boneNode.baseobject.mesh[1].keys[i].time)
		)
	)
	
	if keys.count <= 0 then
	(
		appendIfUnique keys (ConvertAnimKeys animationRange.start)
	)
	
	sort keys
	
	if keys[1] < 0.0 then
	(
		deleteItem keys 1
	)
)

(
	ael = (dotnetclass "System.Reflection.Assembly").Load ((dotnetclass "System.IO.File").ReadAllBytes "..\AoMEngineLibrary.dll")
	ampForm = ael.CreateInstance "AoMEngineLibrary.AMP.MaxPluginForm"
	--dotNet.setLifeTimeControl ampForm #dotNet
	--thePtr = DotNetObject "System.IntPtr" (windows.getMAXHWND())
	--ampForm.Show ((dotnetclass "System.Windows.Forms.Control").fromHandle thePtr)
	--ampForm.topMost = true
	--ampForm.Show()
	ampForm.ShowModeless()
)