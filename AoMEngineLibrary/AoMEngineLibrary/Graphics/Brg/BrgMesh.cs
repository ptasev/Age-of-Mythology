namespace AoMEngineLibrary.Graphics.Brg
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Wintellect.PowerCollections;

    public class BrgMesh
    {
        public BrgFile ParentFile { get; set; }

        public BrgMeshHeader Header { get; set; }
        public Vector3<Single>[] Vertices { get; set; }
        public Vector3<Single>[] Normals { get; set; }

        public Vector2<Single>[] TextureCoordinates { get; set; }
        public Int16[] FaceMaterials { get; set; }
        public Vector3<Int16>[] FaceVertices { get; set; }
        private Int16[] VertexMaterials { get; set; }

        public BrgMeshExtendedHeader ExtendedHeader { get; set; }
        public BrgUserDataEntry[] UserDataEntries { get; set; }
        private float[] particleData;

        public VertexColor[] VertexColors { get; set; }
        public BrgAttachpointCollection Attachpoints { get; set; }
        public float[] NonUniformKeys { get; set; }

        public BrgMesh(BrgBinaryReader reader, BrgFile file)
        {
            this.ParentFile = file;

            this.Header = new BrgMeshHeader(reader);

            this.Vertices = new Vector3<float>[0];
            this.Normals = new Vector3<float>[0];
            this.TextureCoordinates = new Vector2<float>[0];
            this.FaceMaterials = new Int16[0];
            this.FaceVertices = new Vector3<short>[0];
            this.VertexMaterials = new Int16[0];
            if (!this.Header.Flags.HasFlag(BrgMeshFlag.PARTICLEPOINTS))
            {
                this.Vertices = new Vector3<float>[this.Header.NumVertices];
                for (int i = 0; i < this.Header.NumVertices; i++)
                {
                    this.Vertices[i] = reader.ReadVector3Single(true, this.Header.Version == 22);
                }
                this.Normals = new Vector3<float>[this.Header.NumVertices];
                for (int i = 0; i < this.Header.NumVertices; i++)
                {
                    if (this.Header.Version >= 13 && this.Header.Version <= 17)
                    {
                        reader.ReadInt16(); // No idea what this is
                    }
                    else // v == 18, 19 or 22
                    {
                        this.Normals[i] = reader.ReadVector3Single(true, this.Header.Version == 22);
                    }
                }

                if ((!this.Header.Flags.HasFlag(BrgMeshFlag.SECONDARYMESH) ||
                    this.Header.Flags.HasFlag(BrgMeshFlag.ANIMTEXCOORDS) ||
                    this.Header.Flags.HasFlag(BrgMeshFlag.PARTICLESYSTEM)) &&
                    this.Header.Flags.HasFlag(BrgMeshFlag.TEXCOORDSA))
                {
                    this.TextureCoordinates = new Vector2<float>[this.Header.NumVertices];
                    for (int i = 0; i < this.Header.NumVertices; i++)
                    {
                        reader.ReadVector2(out this.TextureCoordinates[i], this.Header.Version == 22);
                    }
                }

                if (!this.Header.Flags.HasFlag(BrgMeshFlag.SECONDARYMESH) ||
                    this.Header.Flags.HasFlag(BrgMeshFlag.PARTICLESYSTEM))
                {
                    if (this.Header.Flags.HasFlag(BrgMeshFlag.MATERIAL))
                    {
                        this.FaceMaterials = new Int16[this.Header.NumFaces];
                        for (int i = 0; i < this.Header.NumFaces; i++)
                        {
                            this.FaceMaterials[i] = reader.ReadInt16();
                        }
                    }

                    this.FaceVertices = new Vector3<Int16>[this.Header.NumFaces];
                    for (int i = 0; i < this.Header.NumFaces; i++)
                    {
                        this.FaceVertices[i] = reader.ReadVector3Int16();
                    }

                    if (this.Header.Flags.HasFlag(BrgMeshFlag.MATERIAL))
                    {
                        this.VertexMaterials = new Int16[this.Header.NumVertices];
                        for (int i = 0; i < this.Header.NumVertices; i++)
                        {
                            this.VertexMaterials[i] = reader.ReadInt16();
                        }
                    }
                }
            }

            this.UserDataEntries = new BrgUserDataEntry[this.Header.UserDataEntryCount];
            if (!this.Header.Flags.HasFlag(BrgMeshFlag.PARTICLEPOINTS))
            {
                for (int i = 0; i < this.Header.UserDataEntryCount; i++)
                {
                    this.UserDataEntries[i] = reader.ReadUserDataEntry(false);
                }
            }

            this.ExtendedHeader = new BrgMeshExtendedHeader(reader, this.Header.ExtendedHeaderSize);

            this.particleData = new float[0];
            if (this.Header.Flags.HasFlag(BrgMeshFlag.PARTICLEPOINTS))
            {
                this.particleData = new float[4 * this.Header.NumVertices];
                for (int i = 0; i < this.particleData.Length; i++)
                {
                    this.particleData[i] = reader.ReadSingle();
                }
                for (int i = 0; i < this.Header.UserDataEntryCount; i++)
                {
                    this.UserDataEntries[i] = reader.ReadUserDataEntry(true);
                }
            }

            if (this.Header.Version == 13)
            {
                reader.ReadBytes(this.ExtendedHeader.NameLength);
            }

            if (this.Header.Version >= 13 && this.Header.Version <= 18)
            {
                this.Header.Flags |= BrgMeshFlag.ATTACHPOINTS;
            }
            if (this.Header.Flags.HasFlag(BrgMeshFlag.ATTACHPOINTS))
            {
                Int16 numMatrix = this.ExtendedHeader.NumDummies;
                Int16 numIndex = this.ExtendedHeader.NumNameIndexes;
                if (this.Header.Version == 19 || this.Header.Version == 22)
                {
                    numMatrix = reader.ReadInt16();
                    numIndex = reader.ReadInt16();
                    reader.ReadInt16();
                }

                BrgAttachpoint[] attpts = new BrgAttachpoint[numMatrix];
                for (int i = 0; i < numMatrix; i++)
                {
                    attpts[i] = new BrgAttachpoint();
                }
                for (int i = 0; i < numMatrix; i++)
                {
                    attpts[i].XVector = reader.ReadVector3Single(true, this.Header.Version == 22);
                }
                for (int i = 0; i < numMatrix; i++)
                {
                    attpts[i].YVector = reader.ReadVector3Single(true, this.Header.Version == 22);
                }
                for (int i = 0; i < numMatrix; i++)
                {
                    attpts[i].ZVector = reader.ReadVector3Single(true, this.Header.Version == 22);
                }
                if (this.Header.Version == 19 || this.Header.Version == 22)
                {
                    for (int i = 0; i < numMatrix; i++)
                    {
                        attpts[i].Position = reader.ReadVector3Single(true, this.Header.Version == 22);
                    }
                }
                for (int i = 0; i < numMatrix; i++)
                {
                    attpts[i].BoundingBoxMin = reader.ReadVector3Single(true, this.Header.Version == 22);
                }
                for (int i = 0; i < numMatrix; i++)
                {
                    attpts[i].BoundingBoxMax = reader.ReadVector3Single(true, this.Header.Version == 22);
                }

                List<int> nameId = new List<int>();
                for (int i = 0; i < numIndex; i++)
                {
                    int duplicate = reader.ReadInt32(); // have yet to find a model with duplicates
                    reader.ReadInt32(); // Skip the id (at least I think its an ID)
                    for (int j = 0; j < duplicate; j++)
                    {
                        nameId.Add(i);
                    }
                }

                this.Attachpoints = new BrgAttachpointCollection();
                for (int i = 0; i < nameId.Count; i++)
                {
                    this.Attachpoints.Add(new BrgAttachpoint(attpts[reader.ReadByte()]));
                    this.Attachpoints[i].NameId = nameId[i];
                    //attpts[reader.ReadByte()].NameId = nameId[i];
                }
                //attachpoints = new List<BrgAttachpoint>(attpts);
            }
            else
            {
                this.Attachpoints = new BrgAttachpointCollection();
            }

            this.VertexColors = new VertexColor[0];
            if (((this.Header.Flags.HasFlag(BrgMeshFlag.COLORALPHACHANNEL) ||
                this.Header.Flags.HasFlag(BrgMeshFlag.COLORCHANNEL)) &&
                !this.Header.Flags.HasFlag(BrgMeshFlag.SECONDARYMESH)) ||
                this.Header.Flags.HasFlag(BrgMeshFlag.ANIMVERTCOLORALPHA))
            {
                this.VertexColors = new VertexColor[this.Header.NumVertices];
                for (int i = 0; i < this.Header.NumVertices; i++)
                {
                    reader.ReadVertexColor(out this.VertexColors[i]);
                }
            }

            // Only seen on first mesh
            this.NonUniformKeys = new float[0];
            if (this.Header.AnimationType.HasFlag(BrgMeshAnimType.NONUNIFORM))
            {
                this.NonUniformKeys = new float[this.ExtendedHeader.NumNonUniformKeys];
                for (int i = 0; i < this.ExtendedHeader.NumNonUniformKeys; i++)
                {
                    this.NonUniformKeys[i] = reader.ReadSingle();
                }
            }

            if (this.Header.Version >= 14 && this.Header.Version <= 19)
            {
                // Face Normals??
                Vector3<float>[] legacy = new Vector3<float>[this.Header.NumFaces];
                for (int i = 0; i < this.Header.NumFaces; i++)
                {
                    legacy[i] = reader.ReadVector3Single();
                }
            }

            if (!this.Header.Flags.HasFlag(BrgMeshFlag.SECONDARYMESH) && this.Header.Version != 22)
            {
                reader.ReadBytes(ExtendedHeader.ShadowNameLength0 + ExtendedHeader.ShadowNameLength1);
            }
        }
        public BrgMesh(BrgFile file)
        {
            this.ParentFile = file;
            this.Header = new BrgMeshHeader();
            this.Header.Version = 22;
            this.Header.ExtendedHeaderSize = 40;

            this.Vertices = new Vector3<float>[0];
            this.Normals = new Vector3<float>[0];

            this.TextureCoordinates = new Vector2<float>[0];
            this.FaceMaterials = new Int16[0];
            this.FaceVertices = new Vector3<Int16>[0];
            this.VertexMaterials = new Int16[0];

            this.ExtendedHeader = new BrgMeshExtendedHeader();
            this.ExtendedHeader.MaterialLibraryTimestamp = 191738312;
            this.ExtendedHeader.ExportedScaleFactor = 1f;

            this.UserDataEntries = new BrgUserDataEntry[0];
            this.particleData = new float[0];

            this.VertexColors = new VertexColor[0];
            this.Attachpoints = new BrgAttachpointCollection();
            this.NonUniformKeys = new float[0];
        }

        public void Write(BrgBinaryWriter writer)
        {
            this.Header.Version = 22;
            if (this.Header.Flags.HasFlag(BrgMeshFlag.PARTICLEPOINTS))
            {
                this.Header.NumVertices = (Int16)(this.particleData.Length / 4);
            }
            else
            {
                this.Header.NumVertices = (Int16)this.Vertices.Length;
            }
            this.Header.NumFaces = (Int16)this.FaceVertices.Length;
            this.Header.UserDataEntryCount = (Int16)this.UserDataEntries.Length;
            this.Header.CenterRadius = this.Header.MaximumExtent.LongestAxisLength();
            this.Header.ExtendedHeaderSize = 40;
            this.Header.Write(writer);

            if (!this.Header.Flags.HasFlag(BrgMeshFlag.PARTICLEPOINTS))
            {
                for (int i = 0; i < this.Vertices.Length; i++)
                {
                    writer.WriteVector3(this.Vertices[i], true, true);
                }
                for (int i = 0; i < this.Vertices.Length; i++)
                {
                    writer.WriteVector3(this.Normals[i], true, true);
                }

                if ((!this.Header.Flags.HasFlag(BrgMeshFlag.SECONDARYMESH) ||
                    this.Header.Flags.HasFlag(BrgMeshFlag.ANIMTEXCOORDS) ||
                    this.Header.Flags.HasFlag(BrgMeshFlag.PARTICLESYSTEM)) &&
                    this.Header.Flags.HasFlag(BrgMeshFlag.TEXCOORDSA))
                {
                    for (int i = 0; i < this.TextureCoordinates.Length; i++)
                    {
                        writer.WriteVector2(ref this.TextureCoordinates[i], true);
                    }
                }

                if (!this.Header.Flags.HasFlag(BrgMeshFlag.SECONDARYMESH) ||
                    this.Header.Flags.HasFlag(BrgMeshFlag.PARTICLESYSTEM))
                {
                    if (this.Header.Flags.HasFlag(BrgMeshFlag.MATERIAL))
                    {
                        for (int i = 0; i < this.FaceMaterials.Length; i++)
                        {
                            writer.Write(this.FaceMaterials[i]);
                        }
                    }

                    for (int i = 0; i < this.FaceVertices.Length; i++)
                    {
                        writer.WriteVector3(ref this.FaceVertices[i]);
                    }

                    if (this.Header.Flags.HasFlag(BrgMeshFlag.MATERIAL))
                    {
                        for (int i = 0; i < this.VertexMaterials.Length; i++)
                        {
                            writer.Write(this.VertexMaterials[i]);
                        }
                    }
                }
            }

            if (!this.Header.Flags.HasFlag(BrgMeshFlag.PARTICLEPOINTS))
            {
                for (int i = 0; i < this.UserDataEntries.Length; i++)
                {
                    writer.WriteUserDataEntry(ref this.UserDataEntries[i], false);
                }
            }

            this.ExtendedHeader.NumNonUniformKeys = NonUniformKeys.Length;
            this.ExtendedHeader.Write(writer);

            if (this.Header.Flags.HasFlag(BrgMeshFlag.PARTICLEPOINTS))
            {
                for (int i = 0; i < this.particleData.Length; i++)
                {
                    writer.Write(this.particleData[i]);
                }
                for (int i = 0; i < this.UserDataEntries.Length; i++)
                {
                    writer.WriteUserDataEntry(ref this.UserDataEntries[i], true);
                }
            }

            if (this.Header.Flags.HasFlag(BrgMeshFlag.ATTACHPOINTS))
            {
                writer.Write((Int16)this.Attachpoints.Count);

                List<int> nameId = new List<int>();
                int maxNameId = -1;
                foreach (BrgAttachpoint att in this.Attachpoints)
                {
                    nameId.Add(att.NameId);
                    if (att.NameId > maxNameId)
                    {
                        maxNameId = att.NameId;
                    }
                }
                Int16 numIndex = (Int16)(maxNameId + 1);//(Int16)(55 - maxNameId);
                writer.Write((Int16)numIndex);
                writer.Write((Int16)1);

                foreach (BrgAttachpoint att in this.Attachpoints)
                {
                    writer.WriteVector3(att.XVector, true, true);
                }
                foreach (BrgAttachpoint att in this.Attachpoints)
                {
                    writer.WriteVector3(att.YVector, true, true);
                }
                foreach (BrgAttachpoint att in this.Attachpoints)
                {
                    writer.WriteVector3(att.ZVector, true, true);
                }
                foreach (BrgAttachpoint att in this.Attachpoints)
                {
                    writer.WriteVector3(att.Position, true, true);
                }
                foreach (BrgAttachpoint att in this.Attachpoints)
                {
                    writer.WriteVector3(att.BoundingBoxMin, true, true);
                }
                foreach (BrgAttachpoint att in this.Attachpoints)
                {
                    writer.WriteVector3(att.BoundingBoxMax, true, true);
                }

                int[] dup = new int[numIndex];
                for (int i = 0; i < nameId.Count; i++)
                {
                    dup[nameId[i]] += 1;
                }
                int countId = 0;
                for (int i = 0; i < numIndex; i++)
                {
                    writer.Write(dup[i]);
                    if (dup[i] == 0)
                    {
                        writer.Write(0);
                    }
                    else
                    {
                        writer.Write(countId);
                    }
                    countId += dup[i];
                }

                List<int> nameId2 = new List<int>(nameId);
                nameId.Sort();
                for (int i = 0; i < this.Attachpoints.Count; i++)
                {
                    for (int j = 0; j < this.Attachpoints.Count; j++)
                    {
                        if (nameId[i] == nameId2[j])
                        {
                            nameId2[j] = -1;
                            writer.Write((byte)j);
                            break;
                        }
                    }
                }
            }

            if (((this.Header.Flags.HasFlag(BrgMeshFlag.COLORALPHACHANNEL) ||
                this.Header.Flags.HasFlag(BrgMeshFlag.COLORCHANNEL)) &&
                !this.Header.Flags.HasFlag(BrgMeshFlag.SECONDARYMESH)) ||
                this.Header.Flags.HasFlag(BrgMeshFlag.ANIMVERTCOLORALPHA))
            {
                for (int i = 0; i < this.VertexColors.Length; i++)
                {
                    writer.WriteVertexColor(ref this.VertexColors[i]);
                }
            }

            if (this.Header.AnimationType.HasFlag(BrgMeshAnimType.NONUNIFORM))
            {
                for (int i = 0; i < this.NonUniformKeys.Length; i++)
                {
                    writer.Write(this.NonUniformKeys[i]);
                }
            }
        }

        // Only used for first key frame/mesh
        public void ExportToMax(string mainObject, float time)
        {
            string vertArray = "";
            string normArray = "";
            string texVerts = "";
            string faceMats = "";
            string faceArray = "";
            if (!this.Header.Flags.HasFlag(BrgMeshFlag.SECONDARYMESH))
            {
                vertArray = Maxscript.NewArray("vertArray");
                normArray = Maxscript.NewArray("normArray");
                texVerts = Maxscript.NewArray("texVerts");

                faceMats = Maxscript.NewArray("faceMats");
                faceArray = Maxscript.NewArray("faceArray");
            }

            Maxscript.CommentTitle("Load Vertices/Normals/UVWs");
            for (int i = 0; i < this.Vertices.Length; i++)
            {
                if (this.Header.Flags.HasFlag(BrgMeshFlag.SECONDARYMESH))
                {
                    Maxscript.AnimateAtTime(time, "setNormal {0} {1} {2}", mainObject, i + 1,
                        Maxscript.NewPoint3Literal<float>(-this.Normals[i].X, -this.Normals[i].Z, this.Normals[i].Y));

                    if (this.Header.Flags.HasFlag(BrgMeshFlag.ANIMTEXCOORDS) &&
                        this.Header.Flags.HasFlag(BrgMeshFlag.TEXCOORDSA))
                    {
                        Maxscript.Animate("{0}.Unwrap_UVW.SetVertexPosition {1}s {2} {3}", mainObject, time, i + 1,
                            Maxscript.NewPoint3Literal<float>(this.TextureCoordinates[i].X, this.TextureCoordinates[i].Y, 0));
                    }

                    if (this.Header.Flags.HasFlag(BrgMeshFlag.ANIMVERTCOLORALPHA))
                    {
                        if (this.Header.Flags.HasFlag(BrgMeshFlag.COLORALPHACHANNEL))
                        {
                            Maxscript.AnimateAtTime(time, "meshop.setVertAlpha {0} -2 {1} {2}",
                                mainObject, i + 1, this.VertexColors[i].A);
                        }
                        else if (this.Header.Flags.HasFlag(BrgMeshFlag.COLORCHANNEL))
                        {
                            Maxscript.AnimateAtTime(time, "meshop.setVertColor {0} 0 {1} (color {2} {3} {4})", mainObject, i + 1,
                                this.VertexColors[i].R, this.VertexColors[i].G, this.VertexColors[i].B);
                        }
                    }
                }
                else
                {
                    Maxscript.Append(normArray, Maxscript.NewPoint3<float>("n",
                        -this.Normals[i].X, -this.Normals[i].Z, this.Normals[i].Y));

                    if (Header.Flags.HasFlag(BrgMeshFlag.TEXCOORDSA))
                    {
                        Maxscript.Append(texVerts, Maxscript.NewPoint3<float>("tV",
                            this.TextureCoordinates[i].X, this.TextureCoordinates[i].Y, 0));
                    }

                    if (Header.Flags.HasFlag(BrgMeshFlag.COLORALPHACHANNEL))
                    {
                        //Maxscript.Command("meshop.supportVAlphas {0}", mainObject);
                        Maxscript.Command("meshop.setVertAlpha {0} -2 {1} {2}",
                            mainObject, i + 1, this.VertexColors[i].A);
                    }
                    else if(this.Header.Flags.HasFlag(BrgMeshFlag.COLORCHANNEL))
                    {
                        Maxscript.Command("meshop.setVertColor {0} 0 {1} (color {2} {3} {4})", mainObject, i + 1,
                            this.VertexColors[i].R, this.VertexColors[i].G, this.VertexColors[i].B);
                    }
                }
            }

            Maxscript.CommentTitle("Load Attachpoints");
            foreach (BrgAttachpoint att in Attachpoints)
            {
                if (this.Header.Flags.HasFlag(BrgMeshFlag.SECONDARYMESH))
                {
                    Maxscript.Command("attachpoint = getNodeByName \"{0}\"", att.GetMaxName());
                    Maxscript.AnimateAtTime(time, "attachpoint.rotation = {0}", att.GetMaxTransform());
                    Maxscript.AnimateAtTime(time, "attachpoint.position = {0}", att.GetMaxPosition());
                    Maxscript.AnimateAtTime(time, "attachpoint.scale = {0}", att.GetMaxScale());
                }
                else
                {
                    string attachDummy = Maxscript.NewDummy("attachDummy", att.GetMaxName(), att.GetMaxTransform(), att.GetMaxPosition(), att.GetMaxBoxSize(), att.GetMaxScale());
                }
            }

            if (!this.Header.Flags.HasFlag(BrgMeshFlag.SECONDARYMESH))
            {
                if (Header.Flags.HasFlag(BrgMeshFlag.MATERIAL))
                {
                    Maxscript.CommentTitle("Load Face Materials");
                    foreach (Int16 fMat in FaceMaterials)
                    {
                        Maxscript.Append(faceMats, fMat.ToString());
                    }
                }

                Maxscript.CommentTitle("Load Faces");
                foreach (Vector3<Int16> face in FaceVertices)
                {
                    Maxscript.Append(faceArray, Maxscript.NewPoint3<Int32>("fV", face.X + 1, face.Z + 1, face.Y + 1));
                }

                Maxscript.AnimateAtTime(ParentFile.GetFrameTime(0), Maxscript.NewMeshLiteral(mainObject, vertArray, normArray, faceArray, faceMats, texVerts));
                Maxscript.Command("{0} = getNodeByName \"{0}\"", mainObject);

                string groundPlanePos = Maxscript.NewPoint3<float>("groundPlanePos", -Header.HotspotPosition.X, -Header.HotspotPosition.Z, Header.HotspotPosition.Y);
                Maxscript.Command("plane name:\"ground\" pos:{0} length:10 width:10", groundPlanePos);

                Maxscript.CommentTitle("TVert Hack");
                Maxscript.Command("buildTVFaces {0}", mainObject);
                for (int i = 1; i <= FaceVertices.Length; i++)
                {
                    Maxscript.Command("setTVFace {0} {1} (getFace {0} {1})", mainObject, i);
                }

                if (Header.Flags.HasFlag(BrgMeshFlag.ANIMTEXCOORDS))
                {
                    Maxscript.Command("select {0}", mainObject);
                    Maxscript.Command("addModifier {0} (Unwrap_UVW())", mainObject);

                    Maxscript.Command("select {0}.verts", mainObject);
                    Maxscript.Animate("{0}.Unwrap_UVW.moveSelected [0,0,0]", mainObject);
                }
            }
        }
        public void ImportFromMax(string mainObject, float time)
        {
            this.Header.CenterPosition = new Vector3<float>
            (
                -Maxscript.QueryFloat("{0}.center.x", mainObject),
                Maxscript.QueryFloat("{0}.center.z", mainObject),
                -Maxscript.QueryFloat("{0}.center.y", mainObject)
            );

            Maxscript.Command("grnd = getNodeByName \"ground\"");
            if (!Maxscript.QueryBoolean("grnd == undefined"))
            {
                this.Header.HotspotPosition = new Vector3<float>
                (
                    -Maxscript.QueryFloat("grnd.position.x"),
                    Maxscript.QueryFloat("grnd.position.z"),
                    -Maxscript.QueryFloat("grnd.position.y")
                );
            }

            Maxscript.Command("{0}BBMax = {0}.max", mainObject);
            Maxscript.Command("{0}BBMin = {0}.min", mainObject);
            Vector3<float> bBoxMax = new Vector3<float>(Maxscript.QueryFloat("{0}BBMax.X", mainObject), Maxscript.QueryFloat("{0}BBMax.Y", mainObject), Maxscript.QueryFloat("{0}BBMax.Z", mainObject));
            Vector3<float> bBoxMin = new Vector3<float>(Maxscript.QueryFloat("{0}BBMin.X", mainObject), Maxscript.QueryFloat("{0}BBMin.Y", mainObject), Maxscript.QueryFloat("{0}BBMin.Z", mainObject));
            Vector3<float> bBox = (bBoxMax - bBoxMin) / 2;
            this.Header.MinimumExtent = new Vector3<float>(-bBox.X, -bBox.Z, -bBox.Y);
            this.Header.MaximumExtent = new Vector3<float>(bBox.X, bBox.Z, bBox.Y);

            string mainMesh = Maxscript.SnapshotAsMesh("mainMesh", mainObject, time);
            int numVertices = (int)Maxscript.Query("{0}.numverts", Maxscript.QueryType.Integer, mainMesh);
            int numFaces = (int)Maxscript.Query("{0}.numfaces", Maxscript.QueryType.Integer, mainMesh);
            string attachDummy = Maxscript.NewArray("attachDummy");
            Maxscript.SetVarAtTime(time, attachDummy, "$helpers/atpt??* as array");
            int numAttachpoints = (int)Maxscript.Query("{0}.count", Maxscript.QueryType.Integer, attachDummy);

            // Figure out the used vertices, and corresponding tex verts
            setUpTVerts(mainMesh, numVertices, numFaces);
            numVertices = Maxscript.QueryInteger("{0}.numverts", mainMesh);
            numFaces = Maxscript.QueryInteger("{0}.numfaces", mainMesh);
            //System.Windows.Forms.MessageBox.Show(numVertices + " " + numFaces);
            List<int> vertexMask = new List<int>(numVertices);
            for (int i = 0; i < numVertices; i++)
            {
                vertexMask.Add(0);
            }
            for (int i = 0; i < numFaces; i++)
            {
                Maxscript.Command("face = getFace {0} {1}", mainMesh, i + 1);
                Maxscript.Command("tFace = getTVFace {0} {1}", mainMesh, i + 1);
                int vert1 = Maxscript.QueryInteger("face[1]") - 1;
                int vert2 = Maxscript.QueryInteger("face[2]") - 1;
                int vert3 = Maxscript.QueryInteger("face[3]") - 1;
                int tVert1 = Maxscript.QueryInteger("tFace[1]");
                int tVert2 = Maxscript.QueryInteger("tFace[2]");
                int tVert3 = Maxscript.QueryInteger("tFace[3]");

                vertexMask[vert1] = tVert1;
                vertexMask[vert2] = tVert2;
                vertexMask[vert3] = tVert3;
            }

            //System.Windows.Forms.MessageBox.Show("1 " + numVertices);
            List<Vector3<float>> verticesList = new List<Vector3<float>>(numVertices);
            List<Vector3<float>> normalsList = new List<Vector3<float>>(numVertices);
            Dictionary<int, Int16> newVertMap = new Dictionary<int, Int16>(numVertices);
            for (int i = 0; i < numVertices; i++)
            {
                //System.Windows.Forms.MessageBox.Show("1.1");
                //System.Windows.Forms.MessageBox.Show("1.2");
                if (vertexMask[i] > 0)
                {
                    newVertMap.Add(i, (Int16)verticesList.Count);
                    //System.Windows.Forms.MessageBox.Show("1.3");
                    Maxscript.Command("vertex = getVert {0} {1}", mainMesh, i + 1);
                    //System.Windows.Forms.MessageBox.Show("1.4");
                    verticesList.Add(new Vector3<float>(-Maxscript.QueryFloat("vertex.x"), Maxscript.QueryFloat("vertex.z"), -Maxscript.QueryFloat("vertex.y")));

                    //System.Windows.Forms.MessageBox.Show("1.5");
                    // Snapshot of Mesh recalculates normals, so do it based on time from the real object
                    Maxscript.SetVarAtTime(time, "normal", "normalize (getNormal {0} {1})", mainObject, i + 1);
                    //System.Windows.Forms.MessageBox.Show("1.6");
                    normalsList.Add(new Vector3<float>(-Maxscript.QueryFloat("normal.x"), Maxscript.QueryFloat("normal.z"), -Maxscript.QueryFloat("normal.y")));
                    //System.Windows.Forms.MessageBox.Show("1.7");
                }
            }
            Vertices = verticesList.ToArray();
            Normals = normalsList.ToArray();

            //System.Windows.Forms.MessageBox.Show("2");
            if (!Header.Flags.HasFlag(BrgMeshFlag.SECONDARYMESH) || Header.Flags.HasFlag(BrgMeshFlag.ANIMTEXCOORDS))
            {
                if (Header.Flags.HasFlag(BrgMeshFlag.TEXCOORDSA))
                {
                    List<Vector2<float>> texVerticesList = new List<Vector2<float>>(numVertices);
                    for (int i = 0; i < numVertices; i++)
                    {
                        if (vertexMask[i] > 0)
                        {
                            Maxscript.Command("tVert = getTVert {0} {1}", mainMesh, vertexMask[i]);
                            texVerticesList.Add(new Vector2<float>(Maxscript.QueryFloat("tVert.x"), Maxscript.QueryFloat("tVert.y")));
                        }
                    }
                    TextureCoordinates = texVerticesList.ToArray();
                }
            }

            //System.Windows.Forms.MessageBox.Show("3");
            HashSet<int> diffFaceMats = new HashSet<int>();
            if (!Header.Flags.HasFlag(BrgMeshFlag.SECONDARYMESH))
            {
                if (Header.Flags.HasFlag(BrgMeshFlag.MATERIAL))
                {
                    FaceMaterials = new Int16[numFaces];
                    for (int i = 0; i < numFaces; i++)
                    {
                        FaceMaterials[i] = (Int16)Maxscript.QueryInteger("getFaceMatID {0} {1}", mainMesh, i + 1);
                        diffFaceMats.Add(FaceMaterials[i]);
                    }
                }

                //System.Windows.Forms.MessageBox.Show("3.1");
                FaceVertices = new Vector3<Int16>[numFaces];
                for (int i = 0; i < FaceVertices.Length; i++)
                {
                    Maxscript.Command("face = getFace {0} {1}", mainMesh, i + 1);
                    FaceVertices[i].X = newVertMap[(Maxscript.QueryInteger("face.x") - 1)];
                    FaceVertices[i].Y = newVertMap[(Maxscript.QueryInteger("face.z") - 1)];
                    FaceVertices[i].Z = newVertMap[(Maxscript.QueryInteger("face.y") - 1)];
                }

                //System.Windows.Forms.MessageBox.Show("3.2");
                if (Header.Flags.HasFlag(BrgMeshFlag.MATERIAL))
                {
                    VertexMaterials = new Int16[Vertices.Length];
                    for (int i = 0; i < FaceVertices.Length; i++)
                    {
                        VertexMaterials[FaceVertices[i].X] = FaceMaterials[i];
                        VertexMaterials[FaceVertices[i].Y] = FaceMaterials[i];
                        VertexMaterials[FaceVertices[i].Z] = FaceMaterials[i];
                    }
                }
            }

            //System.Windows.Forms.MessageBox.Show("4");
            if (this.Header.Flags.HasFlag(BrgMeshFlag.SECONDARYMESH) && diffFaceMats.Count > 0)
            {
                ExtendedHeader.NumMaterials = (byte)(diffFaceMats.Count - 1);
                ExtendedHeader.NumUniqueMaterials = diffFaceMats.Count;
            }
            ExtendedHeader.AnimationLength = Maxscript.QueryFloat("animationRange.end.ticks / 4800 as float");

            //System.Windows.Forms.MessageBox.Show("5 " + numAttachpoints);
            if (Header.Flags.HasFlag(BrgMeshFlag.ATTACHPOINTS))
            {
                Attachpoints = new BrgAttachpointCollection();
                for (int i = 0; i < numAttachpoints; i++)
                {
                    int index = Convert.ToInt32((Maxscript.QueryString("{0}[{1}].name", attachDummy, i + 1)).Substring(4, 2));
                    index = Attachpoints.Add(index);
                    //System.Windows.Forms.MessageBox.Show("5.1");
                    Attachpoints[index].NameId = BrgAttachpoint.GetIdByName((Maxscript.QueryString("{0}[{1}].name", attachDummy, i + 1)).Substring(7));
                    Maxscript.Command("{0}[{1}].name = \"{2}\"", attachDummy, i + 1, Attachpoints[index].GetMaxName());
                    //System.Windows.Forms.MessageBox.Show("5.2");
                    Maxscript.SetVarAtTime(time, "{0}Transform", "{0}[{1}].rotation as matrix3", attachDummy, i + 1);
                    Maxscript.SetVarAtTime(time, "{0}Position", "{0}[{1}].position", attachDummy, i + 1);
                    Maxscript.SetVarAtTime(time, "{0}Scale", "{0}[{1}].scale", attachDummy, i + 1);
                    //System.Windows.Forms.MessageBox.Show("5.3");
                    Vector3<float> scale = new Vector3<float>(Maxscript.QueryFloat("{0}Scale.X", attachDummy), Maxscript.QueryFloat("{0}Scale.Y", attachDummy), Maxscript.QueryFloat("{0}Scale.Z", attachDummy));
                    bBox = scale / 2;
                    //System.Windows.Forms.MessageBox.Show("5.4");

                    Attachpoints[index].XVector.X = -Maxscript.QueryFloat("{0}Transform[1].z", attachDummy);
                    Attachpoints[index].XVector.Y = Maxscript.QueryFloat("{0}Transform[3].z", attachDummy);
                    Attachpoints[index].XVector.Z = -Maxscript.QueryFloat("{0}Transform[2].z", attachDummy);

                    Attachpoints[index].YVector.X = -Maxscript.QueryFloat("{0}Transform[1].y", attachDummy);
                    Attachpoints[index].YVector.Y = Maxscript.QueryFloat("{0}Transform[3].y", attachDummy);
                    Attachpoints[index].YVector.Z = -Maxscript.QueryFloat("{0}Transform[2].y", attachDummy);

                    Attachpoints[index].ZVector.X = -Maxscript.QueryFloat("{0}Transform[1].x", attachDummy);
                    Attachpoints[index].ZVector.Y = Maxscript.QueryFloat("{0}Transform[3].x", attachDummy);
                    Attachpoints[index].ZVector.Z = -Maxscript.QueryFloat("{0}Transform[2].x", attachDummy);

                    Attachpoints[index].Position.X = -Maxscript.QueryFloat("{0}Position.x", attachDummy);
                    Attachpoints[index].Position.Z = -Maxscript.QueryFloat("{0}Position.y", attachDummy);
                    Attachpoints[index].Position.Y = Maxscript.QueryFloat("{0}Position.z", attachDummy);
                    //System.Windows.Forms.MessageBox.Show("5.5");

                    Attachpoints[index].BoundingBoxMin.X = -bBox.X;
                    Attachpoints[index].BoundingBoxMin.Z = -bBox.Y;
                    Attachpoints[index].BoundingBoxMin.Y = -bBox.Z;
                    Attachpoints[index].BoundingBoxMax.X = bBox.X;
                    Attachpoints[index].BoundingBoxMax.Z = bBox.Y;
                    Attachpoints[index].BoundingBoxMax.Y = bBox.Z;
                }
                //System.Windows.Forms.MessageBox.Show("# Atpts: " + Attachpoint.Count);
            }
        }
        private void setUpTVerts(string mainMesh, int numVertices, int numFaces)
        {
            // Reduce Duplicate TVerts
            //string uniqueTVerts = Maxscript.NewArray("uTVerts");
            //string uTVIndex = Maxscript.NewBitArray("uTVIndex");
            //Maxscript.Command("{0}.count = meshop.getNumTverts {1}", uTVIndex, mainMesh);
            //Maxscript.Command("for t = 1 to (meshop.getNumTverts {0}) do ( if(appendIfUnique {1} (getTVert {0} t)) do (append {2} t))", mainMesh, uniqueTVerts, uTVIndex);
            //Maxscript.Command("for t = 1 to (meshop.getNumTverts {0}) do (appendIfUnique {1} (getTVert {0} t))", mainMesh, uniqueTVerts);
            /*Maxscript.Command("meshop.setNumTverts {0} {1}.count", mainMesh, uniqueTVerts);
            Maxscript.Command("for t = 1 to (meshop.getNumTverts {0}) do (setTVert {0} t {1}[t]", mainMesh, uniqueTVerts);
            Maxscript.Command("buildTVFaces {0}", mainMesh);
            for (int i = 1; i <= faceVertices.Length; i++)
            {
                Maxscript.Command("setTVFace {0} {1} (getFace {0} {1})", mainMesh, i);
            }*/
            // Figure out the used vertices, and corresponding tex verts
            OrderedSet<int>[] vertUsage = new OrderedSet<int>[numVertices];
            OrderedSet<int>[] faceUsage = new OrderedSet<int>[numVertices];
            for (int i = 0; i < vertUsage.Length; i++)
            {
                vertUsage[i] = new OrderedSet<int>();
            }
            for (int i = 0; i < faceUsage.Length; i++)
            {
                faceUsage[i] = new OrderedSet<int>();
            }

            for (int i = 0; i < numFaces; i++)
            {
                Maxscript.Command("face = getFace {0} {1}", mainMesh, i + 1);
                Maxscript.Command("tFace = meshop.getMapFace {0} 1 {1}", mainMesh, i + 1);
                int vert1 = Maxscript.QueryInteger("face[1]") - 1;
                int vert2 = Maxscript.QueryInteger("face[2]") - 1;
                int vert3 = Maxscript.QueryInteger("face[3]") - 1;
                int tVert1 = Maxscript.QueryInteger("tFace[1]");
                int tVert2 = Maxscript.QueryInteger("tFace[2]");
                int tVert3 = Maxscript.QueryInteger("tFace[3]");
                try
                {
                    vertUsage[vert1].Add(tVert1);
                    vertUsage[vert2].Add(tVert2);
                    vertUsage[vert3].Add(tVert3);
                    faceUsage[vert1].Add(i + 1);
                    faceUsage[vert2].Add(i + 1);
                    faceUsage[vert3].Add(i + 1);
                }
                catch { throw new Exception(vertUsage.Length + " " + faceUsage.Length + " " + i + " " + vert1 + " " + vert2 + " " + vert3); }
            }

            for (int i = 0; i < vertUsage.Length; i++)
            {
                if (vertUsage[i].Count > 1)
                {
                    Maxscript.Command("vertPos = getVert {0} {1}", mainMesh, i + 1);
                    for (int mapV = 1; mapV < vertUsage[i].Count; mapV++)
                    {
                        int mapVert = vertUsage[i][mapV];
                        //if (!Maxscript.QueryBoolean("{0}[{1}] == true", uTVIndex, mapVert))
                        //  continue;
                        int newIndex = Maxscript.QueryInteger("{0}.numverts", mainMesh) + 1;
                        Maxscript.Command("setNumVerts {0} {1} true", mainMesh, newIndex);
                        Maxscript.Command("setVert {0} {1} {2}", mainMesh, newIndex, "vertPos");
                        for (int f = 0; f < faceUsage[i].Count; f++)
                        {
                            int face = faceUsage[i][f];
                            Maxscript.Command("theFaceDef = getFace {0} {1}", mainMesh, face);
                            Maxscript.Command("theMapFaceDef = meshOp.getMapFace {0} 1 {1}", mainMesh, face);
                            if (Maxscript.QueryInteger("theMapFaceDef.x") == mapVert && Maxscript.QueryInteger("theFaceDef.x") == i + 1)
                            {
                                Maxscript.Command("theFaceDef.x = {0}", newIndex);
                                Maxscript.Command("setFace {0} {1} theFaceDef", mainMesh, face);
                            }
                            if (Maxscript.QueryInteger("theMapFaceDef.y") == mapVert && Maxscript.QueryInteger("theFaceDef.y") == i + 1)
                            {
                                Maxscript.Command("theFaceDef.y = {0}", newIndex);
                                Maxscript.Command("setFace {0} {1} theFaceDef", mainMesh, face);
                            }
                            if (Maxscript.QueryInteger("theMapFaceDef.z") == mapVert && Maxscript.QueryInteger("theFaceDef.z") == i + 1)
                            {
                                Maxscript.Command("theFaceDef.z = {0}", newIndex);
                                Maxscript.Command("setFace {0} {1} theFaceDef", mainMesh, face);
                            }
                        }
                    }
                }
            }
        }

        public class BrgAttachpointCollection : IEnumerable
        {
            private BrgAttachpoint[] attachpoint;
            public readonly static int Capacity = 100;
            public int Count;

            internal BrgAttachpointCollection()
            {
                attachpoint = new BrgAttachpoint[Capacity];
            }

            public void Add()
            {
                Add(Count, new BrgAttachpoint());
            }
            public int Add(int index)
            {
                return Add(index, new BrgAttachpoint());
            }
            public void Add(BrgAttachpoint att)
            {
                Add(Count, att);
            }
            public int Add(int index, BrgAttachpoint att)
            {
                if (Count > 100)
                {
                    throw new Exception("Reached max attachpoint capacity!");
                }
                att.Index = index;
                if (attachpoint[att.Index] != null)
                {
                    att.Index = 0;
                    while (attachpoint[att.Index] != null)
                    {
                        att.Index++;
                    }
                }
                attachpoint[att.Index] = att;
                Count++;
                return att.Index;
            }
            public void Remove(int index)
            {
                attachpoint[index] = null;
                Count--;
            }
            public BrgAttachpoint this[int index]
            {
                get
                {
                    return attachpoint[index];
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
            public IEnumerator GetEnumerator()
            {
                foreach (BrgAttachpoint att in attachpoint)
                {
                    if (att != null)
                        yield return att;
                }
                //return attachpoint.GetEnumerator();
            }
        }
    }
}
