-- BRG IMPORT/EXPORT SCRIPT  (version 1.0)
-- for Age of Mythology models
--  by Petar Tasev
--
-- 
(
fileName

-- ### Mesh Flags ###
NONE1        = 0x8000
TRANSPCOLOR  = 0x4000
NONE2        = 0x2000
NONE3        = 0x1000
MOVINGTEX    = 0x0800
NOTFIRSTMESH = 0x0400
NONE4        = 0x0200
ATTACHPOINTS = 0x0100
NONE5        = 0x0080
MATERIALS    = 0x0040
CHANGINGCOL  = 0x0020
NONE7        = 0x0010
NONE8        = 0x0008
NONE9        = 0x0004
TEXTURE      = 0x0002
VERTCOLOR    = 0x0001

-- ### Mat Flags ###
SFX				= 0x1C000000
MGLOW			= 0x00200000
MATNONE1		= 0x00800000
PLAYERCOLOR	= 0x00040000
SOLIDCOLOR	= 0x00020000
MATTEXTURE	= 0x00000030

importButton
exportButton
mainForm
	
fn round val =
(
	val2 = (val as Integer)
	if ((val - val2) >= 0.5) then
	(
		return (ceil val)
	)
	else
	(
		return (floor val)
	)
)
fn checkFlag flags flag =
(
	if ((bit.and flags flag) == flag) do
	(
		return true;
	)
	
	return false;
)
	
fn import =
(
	resetMaxFile #noprompt -- remove everything
	
	f = FOpen fileName "rb"
	ReadLong f -- "BRG3"
	numMeshes = ReadLong f
	numMaterials = ReadLong f
	animTime = 1
	meshes = #()
	texVerts = #()
	faces = #()
	faceMats = #()
	
	if (numMeshes > 1) do
	(
		animTime = ReadFloat f
		frameRate = (round (numMeshes/animTime))
		animationRange = interval 0 (numMeshes)
	)
	
	for i = 1 to numMeshes do
	(
		numVertices = ReadShort f
		numFaces = ReadShort f
		flags = ReadShort f
		
		vertices = #()
		normals = #()
		
		for v = 1 to numVertices do
		(
			x = ReadFloat f
			y = ReadFloat f
			z = ReadFloat f
			append vertices [x, y, z]
		)
		
		for n = 1 to numVertices do
		(
			x = ReadFloat f
			y = ReadFloat f
			z = ReadFloat f
			append normals [-x, -y, -z]
			-- normals get reset after render, so this is useless
		)
		
		if ((not (checkFlag flags NOTFIRSTMESH)) or (checkFlag flags MOVINGTEX)) do
		(
			if ((checkFlag flags TEXTURE)) do
			(
				for tv = 1 to numVertices do
				(
					u = ReadFloat f
					v = ReadFloat f
					append texVerts [u, v, 0]
				)
			)
		)
		
		if ((not (checkFlag flags NOTFIRSTMESH))) do
		(
			if ((checkFlag flags MATERIALS)) do
			(
				for fm = 1 to numFaces do
				(
					append faceMats (ReadLong f)
				)
			)
			
			for fa = 1 to numFaces do
			(
				x = ReadShort f
				y = ReadShort f
				z = ReadShort f
				append faces [x, y, z]
			)
		)
		
		if ((checkFlag flags ATTACHPOINTS)) do
		(
			numAttachpoints = ReadLong f

			for att = 1 to numAttachpoints do
            (
				aName = "f" + i as String + "a" + att as String + "." + ReadString f
				
				X = ReadFloat f
				Y = ReadFloat f
				Z = ReadFloat f
				xVector = [X, Y, Z]
				
				X = ReadFloat f
				Y = ReadFloat f
				Z = ReadFloat f
				yVector = [X, Y, Z]
				
				X = ReadFloat f
				Y = ReadFloat f
				Z = ReadFloat f
				zVector = [X, Y, Z]
				
				X = ReadFloat f
				Y = ReadFloat f
				Z = ReadFloat f
				positionVector = [X, Y, Z]
				
				dummy name:aName transform:(matrix3 xVector yVector zVector positionVector) boxsize:[0.5, 0.5, 0.5]
			)
		)
		
		append meshes (mesh vertices:vertices normals:normals faces:faces materialIDS:faceMats tverts:texVerts)
		
		--print (getNormal $Object001.mesh 1)
		for fid = 1 to meshes[i].numfaces do
		(
			--setFaceMatID meshes[i] fid faceMats[fid]
		)
		
		-- Workaround for TVerts 3ds Max bug
		buildTVFaces meshes[i]
		for tvFix = 1 to meshes[i].numfaces do
		(
			setTVFace meshes[i] tvFix (getFace meshes[i] tvFix)
		)
		
		--update meshes[i] normals:false
	)
	
	if (numMaterials > 0) do
	(
		matGroup = multimaterial numsubs:numMaterials
		
		for mid = 1 to numMaterials do
		(
			rmid = ReadLong f
			flags = ReadLong f
			matName = ReadString f
		
			mat = StandardMaterial()
		
			tex = BitmapTexture()
			tex.name = matName
			tex.filename = matName + ".tga" --"m" + (rmid as string) + ".bmp"
			tex.alphasource = 2
		
			--mat.id = rmid
			mat.name = matName
			if ((checkFlag flags MATTEXTURE)) do
			(
				mat.diffusemap = tex
			)
			if ((checkFlag flags MGLOW)) do
			(
				mat.selfillummap = tex
			)
			
			matGroup[mid] = mat
			matGroup.materialIDList[mid] = rmid
		)
		
		meshes[1].material = matGroup
		for mId = 1 to numMaterials do
		(
			showTextureMap meshes[1].material.materialList[mId] true
		)
	)
	
	FClose f
	
	new_mesh = meshes[1]
	firstAttach = execute("$f1a*") as array
	with animate on
	(
		frame0 = copy meshes[1]
		attach0 = #()
		for fAIndex = 1 to firstAttach.count do
		(
			append attach0 firstAttach[fAIndex].transform
		)
		
		for a = 2 to numMeshes do 
		(
			attachpointsFrame = execute("$f" + a as String + "a*") as array
			at time ((a-1))
			(
			    for i = 1 to new_mesh.mesh.numverts do
				(
					move new_mesh.verts[i] (meshes[a].verts[i].pos-new_mesh.verts[i].pos)
				)
				
				for att = 1 to attachpointsFrame.count do
				(
					firstAttach[att].transform = attachpointsFrame[att].transform
				)
			)
			delete meshes[a]
			delete attachpointsFrame
		)
		
		if (numMeshes > 1) do
		(
			at time (numMeshes)
			(
			    for i = 1 to new_mesh.mesh.numverts do
				(
					move new_mesh.verts[i] (frame0.verts[i].pos-new_mesh.verts[i].pos)
				)
				
				for att = 1 to firstAttach.count do
				(
					firstAttach[att].transform = attach0[att]
				)
			)
			delete frame0
			--delete attach0
		)
	)
	
	update meshes[1]
)

fn setFlag flags flag =
(
	return bit.or flags flag
)

fn createFlags mainObj =
(
	attachpointsFrame = $helpers/f?a?* as array
	flags = 0
	
	if (attachpointsFrame.count > 0) do
	(
		flags = setFlag flags ATTACHPOINTS
	)
	
	if (mainObj.material.count > 0) do
	(
		flags = setFlag flags MATERIALS
		
		for m = 1 to mainObj.material.materialList.count do
		(
			mId = mainObj.material.materialIDList[m]
			if (classof mainObj.material[mId].diffusemap == BitmapTexture) do
			(
				flags = setFlag flags TEXTURE
			)
		)
	)
	
	return flags
)
fn createMatFlags mat =
(
	flags = MATNONE1
	
	if ((classof mat.diffusemap) == BitmapTexture) do
	(
		flags = setFlag flags MATTEXTURE
	)
	if ((classof mat.selfillummap) == BitmapTexture) do
	(
		flags = setFlag flags MGLOW
	)
	
	return flags
)
fn getMatName mat flags =
(
	matName = ""
	
	if ((checkFlag flags MATTEXTURE)) do
	(
		matName = getFilenameFile(mat.diffusemap.filename)
	)
	if ((checkFlag flags MGLOW)) do
	(
		matName = getFilenameFile(mat.selfillummap.filename)
	)
	
	return matName
)

fn export =
(
	f = FOpen fileName "wb"
	mainObj
	at time 0 (mainObj = selection[1])
	mainMesh = mainObj.mesh
	numMeshes = animationRange.end
	numMaterials = mainObj.material.materialList.count
	animTime = numMeshes / (frameRate + 0.0)
	flags = createFlags mainObj
	
	WriteLong f 860312130 -- "BRG3"
	WriteLong f numMeshes
	WriteLong f numMaterials -- numMaterials
	
	if (numMeshes > 1) do
	(
		WriteFloat f animTime
	)
	
	for i = 1 to numMeshes do
	(
		at time ((i-1))
		(
			mainMesh = selection[1].mesh
			
			WriteShort f mainMesh.numverts
			WriteShort f mainMesh.numfaces
			if (i > 1) do
			(
				flags = setFlag flags NOTFIRSTMESH
			)
			WriteShort f flags
			
			for v = 1 to mainMesh.numverts do
			(
				vert = getVert mainMesh v
				WriteFloat f vert.x
				WriteFloat f vert.y
				WriteFloat f vert.z
			)
			
			for n = 1 to mainMesh.numverts do
			(
				normal = getNormal mainMesh n
				WriteFloat f -normal.x
				WriteFloat f -normal.y
				WriteFloat f -normal.z
			)
			
			if ((not (checkFlag flags NOTFIRSTMESH)) or (checkFlag flags MOVINGTEX)) do
			(
				if ((checkFlag flags TEXTURE)) do
				(
					for tV = 1 to mainMesh.numverts do
					(
						tVert = getTVert mainMesh tV
						WriteFloat f tVert[1]
						WriteFloat f tVert[2]
					)
				)
			)
		
			if ((not (checkFlag flags NOTFIRSTMESH))) do
			(
				if ((checkFlag flags MATERIALS)) do
				(
					for fM = 1 to mainMesh.numfaces do
					(
						faceMat = getFaceMatID mainMesh fM
						WriteLong f faceMat
					)
				)
			
				for fa = 1 to mainMesh.numfaces do
				(
					face = getFace mainMesh fa
					WriteShort f face[1]
					WriteShort f face[2]
					WriteShort f face[3]
				)
			)
			
			attachpointsFrame = $helpers/f?a?* as array
			if ((checkFlag flags ATTACHPOINTS)) do
			(
				numAttachpoints = attachpointsFrame.count
				WriteLong f numAttachpoints

				for att = 1 to numAttachpoints do
				(
					WriteString f (substring attachpointsFrame[att].name 6 (attachpointsFrame[att].name.count - 5))
					
					WriteFloat f attachpointsFrame[att].transform[1][1]
					WriteFloat f attachpointsFrame[att].transform[1][2]
					WriteFloat f attachpointsFrame[att].transform[1][3]
						
					WriteFloat f attachpointsFrame[att].transform[2][1]
					WriteFloat f attachpointsFrame[att].transform[2][2]
					WriteFloat f attachpointsFrame[att].transform[2][3]
						
					WriteFloat f attachpointsFrame[att].transform[3][1]
					WriteFloat f attachpointsFrame[att].transform[3][2]
					WriteFloat f attachpointsFrame[att].transform[3][3]
						
					WriteFloat f attachpointsFrame[att].transform[4][1]
					WriteFloat f attachpointsFrame[att].transform[4][2]
					WriteFloat f attachpointsFrame[att].transform[4][3]
				)
			)
		)
	)
	
	if (numMaterials > 0) do
	(
		for m = 1 to numMaterials do
		(
			mId = mainObj.material.materialIDList[m]
			mat = mainObj.material[mId]
			flags = createMatFlags mat
			WriteLong f mId
			WriteLong f flags
			WriteString f (getMatName mat flags)
		)
	)
	
	FClose f
)

fn importButton_Click =
(
	theDialog = dotNetObject "System.Windows.Forms.OpenFileDialog"
	theDialog.title = "Select a file to import"
	theDialog.Multiselect = false
	theDialog.Filter = "Br3 files (*.br3)|*.br3|All Files (*.*)|*.*"
	theDialog.FilterIndex = 1
	result = theDialog.showDialog()
	result.ToString()
	if result.Equals result.OK then (
		filename = theDialog.fileName
		import()
	)
)

fn exportButton_Click =
(
	theDialog = dotNetObject "System.Windows.Forms.SaveFileDialog"
	theDialog.title = "Select a filename to export"
	theDialog.Filter = "Br3 files (*.br3)|*.br3|All Files (*.*)|*.*"
	theDialog.FilterIndex = 1
	result = theDialog.showDialog()
	result.ToString()
	if result.Equals result.OK then (
		filename = theDialog.fileName
		export()
	)
)

-- ### Create Import Button ###
importButton = dotNetObject "System.Windows.Forms.Button"
importButton.text = "Import"
--importButton.size = dotNetObject "System.Drawing.Size" 60 60
importButton.location = dotNetObject "System.Drawing.Point" 0 0
dotNet.addEventHandler importButton "click" importButton_Click

-- ### Create Export Button ###
exportButton = dotNetObject "System.Windows.Forms.Button"
exportButton.text = "Export"
--importButton.size = dotNetObject "System.Drawing.Size" 60 60
exportButton.location = dotNetObject "System.Drawing.Point" 100 0
dotNet.addEventHandler exportButton "click" exportButton_Click

-- ### Create Form ###
mainForm = dotNetObject "System.Windows.Forms.Form"
mainForm.controls.add importButton
mainForm.controls.add exportButton
mainForm.topmost = true
mainForm.show()

)