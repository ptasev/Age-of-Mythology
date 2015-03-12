namespace AoMEngineLibrary.Graphics.Brg
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Wintellect.PowerCollections;

    public class BrgMesh
    {
        public BrgFile ParentFile;

        public BrgMeshHeader header;
        public Vector3<float>[] vertices;
        public Vector3<float>[] normals;

        public Vector2<float>[] texVertices;
        public Int16[] faceMaterials;
        public Vector3<Int16>[] faceVertices;
        private Int16[] vertMaterials;

        public BrgMeshExtendedHeader extendedHeader;
        public BrgUserDataEntry[] UserDataEntries;
        float[] particleData;

        public VertexColor[] vertexColors; // unknown0a

        //public Int16 numMatrix;
        //Int16 numIndex;
        //public Int16 unknown10;
        public BrgAttachpointCollection Attachpoint;
        public float[] animTimeAdjust;

        public BrgMesh(BrgBinaryReader reader, BrgFile file)
        {
            ParentFile = file;

            reader.ReadMeshHeader(ref header);

            vertices = new Vector3<float>[0];
            normals = new Vector3<float>[0];
            texVertices = new Vector2<float>[0];
            faceMaterials = new Int16[0];
            faceVertices = new Vector3<short>[0];
            vertMaterials = new Int16[0];
            if (!header.flags.HasFlag(BrgMeshFlag.PARTICLEPOINTS))
            {
                vertices = new Vector3<float>[header.numVertices];
                for (int i = 0; i < header.numVertices; i++)
                {
                    reader.ReadVector3(out vertices[i], true, header.version == 22);
                }
                normals = new Vector3<float>[header.numVertices];
                for (int i = 0; i < header.numVertices; i++)
                {
                    if (header.version >= 13 && header.version <= 17)
                    {
                        reader.ReadInt16(); // No idea what this is
                    }
                    else // v == 18, 19 or 22
                    {
                        reader.ReadVector3(out normals[i], true, header.version == 22);
                    }
                }

                if (!header.flags.HasFlag(BrgMeshFlag.SECONDARYMESH) || header.flags.HasFlag(BrgMeshFlag.ANIMTEXCOORDS) || header.flags.HasFlag(BrgMeshFlag.PARTICLESYSTEM))
                {
                    if (header.flags.HasFlag(BrgMeshFlag.TEXCOORDSA))
                    {
                        texVertices = new Vector2<float>[header.numVertices];
                        for (int i = 0; i < header.numVertices; i++)
                        {
                            reader.ReadVector2(out texVertices[i], header.version == 22);
                        }
                    }
                }

                if (!header.flags.HasFlag(BrgMeshFlag.SECONDARYMESH) || header.flags.HasFlag(BrgMeshFlag.PARTICLESYSTEM))
                {
                    if (header.flags.HasFlag(BrgMeshFlag.MATERIAL))
                    {
                        faceMaterials = new Int16[header.numFaces];
                        for (int i = 0; i < header.numFaces; i++)
                        {
                            faceMaterials[i] = reader.ReadInt16();
                        }
                    }

                    faceVertices = new Vector3<Int16>[header.numFaces];
                    for (int i = 0; i < header.numFaces; i++)
                    {
                        reader.ReadVector3(out faceVertices[i]);
                    }

                    if (header.flags.HasFlag(BrgMeshFlag.MATERIAL))
                    {
                        vertMaterials = new Int16[header.numVertices];
                        for (int i = 0; i < header.numVertices; i++)
                        {
                            vertMaterials[i] = reader.ReadInt16();
                        }
                    }
                }
            }

            UserDataEntries = new BrgUserDataEntry[header.userDataEntryCount];
            if (!header.flags.HasFlag(BrgMeshFlag.PARTICLEPOINTS))
            {
                for (int i = 0; i < header.userDataEntryCount; i++)
                {
                    UserDataEntries[i] = reader.ReadUserDataEntry(false);
                }
            }

            reader.ReadMeshExtendedHeader(ref extendedHeader, header.extendedHeaderSize);

            particleData = new float[0];
            if (header.flags.HasFlag(BrgMeshFlag.PARTICLEPOINTS))
            {
                particleData = new float[4 * header.numVertices];
                for (int i = 0; i < particleData.Length; i++)
                {
                    particleData[i] = reader.ReadSingle();
                }
                for (int i = 0; i < header.userDataEntryCount; i++)
                {
                    UserDataEntries[i] = reader.ReadUserDataEntry(true);
                }
            }

            if (header.version == 13)
            {
                reader.ReadBytes(extendedHeader.nameLength);
            }

            if (header.version >= 13 && header.version <= 18)
            {
                header.flags |= BrgMeshFlag.ATTACHPOINTS;
            }
            if (header.flags.HasFlag(BrgMeshFlag.ATTACHPOINTS))
            {
                Int16 numMatrix = extendedHeader.numMatrix;
                Int16 numIndex = extendedHeader.numIndex;
                if (header.version == 19 || header.version == 22)
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
                    reader.ReadVector3(out attpts[i].x, true, header.version == 22);
                }
                for (int i = 0; i < numMatrix; i++)
                {
                    reader.ReadVector3(out attpts[i].y, true, header.version == 22);
                }
                for (int i = 0; i < numMatrix; i++)
                {
                    reader.ReadVector3(out attpts[i].z, true, header.version == 22);
                }
                if (header.version == 19 || header.version == 22)
                {
                    for (int i = 0; i < numMatrix; i++)
                    {
                        reader.ReadVector3(out attpts[i].position, true, header.version == 22);
                    }
                }
                for (int i = 0; i < numMatrix; i++)
                {
                    reader.ReadVector3(out attpts[i].unknown11a, true, header.version == 22);
                }
                for (int i = 0; i < numMatrix; i++)
                {
                    reader.ReadVector3(out attpts[i].unknown11b, true, header.version == 22);
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

                Attachpoint = new BrgAttachpointCollection();
                for (int i = 0; i < nameId.Count; i++)
                {
                    Attachpoint.Add(new BrgAttachpoint(attpts[reader.ReadByte()]));
                    Attachpoint[i].NameId = nameId[i];
                    //attpts[reader.ReadByte()].NameId = nameId[i];
                }
                //attachpoints = new List<BrgAttachpoint>(attpts);
            }
            else
            {
                Attachpoint = new BrgAttachpointCollection();
            }

            vertexColors = new VertexColor[0];
            if (((header.flags.HasFlag(BrgMeshFlag.COLORALPHACHANNEL) || header.flags.HasFlag(BrgMeshFlag.COLORCHANNEL)) && !header.flags.HasFlag(BrgMeshFlag.SECONDARYMESH))
                || header.flags.HasFlag(BrgMeshFlag.ANIMVERTCOLORALPHA))
            {
                vertexColors = new VertexColor[header.numVertices];
                for (int i = 0; i < header.numVertices; i++)
                {
                    reader.ReadVertexColor(out vertexColors[i]);
                }
            }

            // Only seen on first mesh
            animTimeAdjust = new float[0];
            if (header.properties.HasFlag(BrgMeshAnimType.NONUNIFORM))
            {
                animTimeAdjust = new float[extendedHeader.nonUniformKeyCount];
                for (int i = 0; i < extendedHeader.nonUniformKeyCount; i++)
                {
                    animTimeAdjust[i] = reader.ReadSingle();
                }
            }

            if (header.version >= 14 && header.version <= 19)
            {
                // Face Normals??
                Vector3<float>[] legacy = new Vector3<float>[header.numFaces];
                for (int i = 0; i < header.numFaces; i++)
                {
                    reader.ReadVector3(out legacy[i]);
                }
            }

            if (!header.flags.HasFlag(BrgMeshFlag.SECONDARYMESH) && header.version != 22)
            {
                reader.ReadBytes(extendedHeader.shadowNameLength0 + extendedHeader.shadowNameLength1);
            }
        }
        public BrgMesh(BrgFile file)
        {
            ParentFile = file;
            header.version = 22;
            header.extendedHeaderSize = 40;

            vertices = new Vector3<float>[0];
            normals = new Vector3<float>[0];

            texVertices = new Vector2<float>[0];
            faceMaterials = new Int16[0];
            faceVertices = new Vector3<Int16>[0];
            vertMaterials = new Int16[0];

            extendedHeader.materialLibraryTimestamp = 191738312;
            extendedHeader.exportedScaleFactor = 1f;

            UserDataEntries = new BrgUserDataEntry[0];
            particleData = new float[0];

            Vector2<float>[] unknown0a = new Vector2<float>[0];
            Attachpoint = new BrgAttachpointCollection();
            animTimeAdjust = new float[0];
        }

        public void Write(BrgBinaryWriter writer)
        {
            header.version = 22;
            if (header.flags.HasFlag(BrgMeshFlag.PARTICLEPOINTS))
            {
                header.numVertices = (Int16)(particleData.Length / 4);
            }
            else
            {
                header.numVertices = (Int16)vertices.Length;
            }
            header.numFaces = (Int16)faceVertices.Length;
            header.userDataEntryCount = (Int16)UserDataEntries.Length;
            header.centerRadius = header.boundingBoxMax.LongestAxisLength();
            header.extendedHeaderSize = 40;
            writer.WriteMeshHeader(ref header);

            if (!header.flags.HasFlag(BrgMeshFlag.PARTICLEPOINTS))
            {
                for (int i = 0; i < vertices.Length; i++)
                {
                    writer.WriteVector3(ref vertices[i], true, true);
                }
                for (int i = 0; i < vertices.Length; i++)
                {
                    writer.WriteVector3(ref normals[i], true, true);
                }

                if (!header.flags.HasFlag(BrgMeshFlag.SECONDARYMESH) || header.flags.HasFlag(BrgMeshFlag.ANIMTEXCOORDS) || header.flags.HasFlag(BrgMeshFlag.PARTICLESYSTEM))
                {
                    if (header.flags.HasFlag(BrgMeshFlag.TEXCOORDSA))
                    {
                        for (int i = 0; i < texVertices.Length; i++)
                        {
                            writer.WriteVector2(ref texVertices[i], true);
                        }
                    }
                }

                if (!header.flags.HasFlag(BrgMeshFlag.SECONDARYMESH) || header.flags.HasFlag(BrgMeshFlag.PARTICLESYSTEM))
                {
                    if (header.flags.HasFlag(BrgMeshFlag.MATERIAL))
                    {
                        for (int i = 0; i < faceMaterials.Length; i++)
                        {
                            writer.Write(faceMaterials[i]);
                        }
                    }

                    for (int i = 0; i < faceVertices.Length; i++)
                    {
                        writer.WriteVector3(ref faceVertices[i]);
                    }

                    if (header.flags.HasFlag(BrgMeshFlag.MATERIAL))
                    {
                        for (int i = 0; i < vertMaterials.Length; i++)
                        {
                            writer.Write(vertMaterials[i]);
                        }
                    }
                }
            }

            if (!header.flags.HasFlag(BrgMeshFlag.PARTICLEPOINTS))
            {
                for (int i = 0; i < UserDataEntries.Length; i++)
                {
                    writer.WriteUserDataEntry(ref UserDataEntries[i], false);
                }
            }

            extendedHeader.nonUniformKeyCount = animTimeAdjust.Length;
            writer.WriteMeshExtendedHeader(ref extendedHeader);

            if (header.flags.HasFlag(BrgMeshFlag.PARTICLEPOINTS))
            {
                for (int i = 0; i < particleData.Length; i++)
                {
                    writer.Write(particleData[i]);
                }
                for (int i = 0; i < UserDataEntries.Length; i++)
                {
                    writer.WriteUserDataEntry(ref UserDataEntries[i], true);
                }
            }

            if (header.flags.HasFlag(BrgMeshFlag.ATTACHPOINTS))
            {
                writer.Write((Int16)Attachpoint.Count);

                List<int> nameId = new List<int>();
                int maxNameId = -1;
                foreach (BrgAttachpoint att in Attachpoint)
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


                foreach (BrgAttachpoint att in Attachpoint)
                {
                    writer.WriteVector3(ref att.x, true, true);
                }
                foreach (BrgAttachpoint att in Attachpoint)
                {
                    writer.WriteVector3(ref att.y, true, true);
                }
                foreach (BrgAttachpoint att in Attachpoint)
                {
                    writer.WriteVector3(ref att.z, true, true);
                }
                foreach (BrgAttachpoint att in Attachpoint)
                {
                    writer.WriteVector3(ref att.position, true, true);
                }
                foreach (BrgAttachpoint att in Attachpoint)
                {
                    writer.WriteVector3(ref att.unknown11a, true, true);
                }
                foreach (BrgAttachpoint att in Attachpoint)
                {
                    writer.WriteVector3(ref att.unknown11b, true, true);
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
                for (int i = 0; i < Attachpoint.Count; i++)
                {
                    for (int j = 0; j < Attachpoint.Count; j++)
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

            if (((header.flags.HasFlag(BrgMeshFlag.COLORALPHACHANNEL) || header.flags.HasFlag(BrgMeshFlag.COLORCHANNEL)) && !header.flags.HasFlag(BrgMeshFlag.SECONDARYMESH))
                || header.flags.HasFlag(BrgMeshFlag.ANIMVERTCOLORALPHA))
            {
                for (int i = 0; i < vertexColors.Length; i++)
                {
                    writer.WriteVertexColor(ref vertexColors[i]);
                }
            }

            if (header.properties.HasFlag(BrgMeshAnimType.NONUNIFORM))
            {
                for (int i = 0; i < animTimeAdjust.Length; i++)
                {
                    writer.Write(animTimeAdjust[i]);
                }
            }
        }

        // Only used for first key frame/mesh
        public string ExportToMax()
        {
            string vertArray = Maxscript.NewArray("vertArray");
            string normArray = Maxscript.NewArray("normArray");
            string texVerts = Maxscript.NewArray("texVerts");

            string faceMats = Maxscript.NewArray("faceMats");
            string faceArray = Maxscript.NewArray("faceArray");

            Maxscript.CommentTitle("Load Vertices");
            foreach (Vector3<float> vert in vertices)
            {
                Maxscript.Append(vertArray, Maxscript.NewPoint3<float>("v", -vert.X, -vert.Z, vert.Y));
            }
            Maxscript.CommentTitle("Load Normals");
            foreach (Vector3<float> norm in normals)
            {
                Maxscript.Append(normArray, Maxscript.NewPoint3<float>("n", -norm.X, -norm.Z, norm.Y));
            }
            if (!header.flags.HasFlag(BrgMeshFlag.SECONDARYMESH) || header.flags.HasFlag(BrgMeshFlag.ANIMTEXCOORDS))
            {
                if (header.flags.HasFlag(BrgMeshFlag.TEXCOORDSA))
                {
                    Maxscript.CommentTitle("Load UVW");
                    foreach (Vector2<float> texVert in texVertices)
                    {
                        Maxscript.Append(texVerts, Maxscript.NewPoint3<float>("tV", texVert.X, texVert.Y, 0));
                    }
                }
            }

            if (!header.flags.HasFlag(BrgMeshFlag.SECONDARYMESH))
            {
                if (header.flags.HasFlag(BrgMeshFlag.MATERIAL))
                {
                    Maxscript.CommentTitle("Load Face Materials");
                    foreach (Int16 fMat in faceMaterials)
                    {
                        Maxscript.Append(faceMats, fMat.ToString());
                    }
                }

                Maxscript.CommentTitle("Load Faces");
                foreach (Vector3<Int16> face in faceVertices)
                {
                    Maxscript.Append(faceArray, Maxscript.NewPoint3<Int32>("fV", face.X + 1, face.Z + 1, face.Y + 1));
                }
            }

            if (header.flags.HasFlag(BrgMeshFlag.ATTACHPOINTS))
            {
                Maxscript.CommentTitle("Load Attachpoints");
                foreach (BrgAttachpoint att in Attachpoint)
                {
                    string attachDummy = Maxscript.NewDummy("attachDummy", att.GetMaxName(), att.GetMaxTransform(), att.GetMaxPosition(), att.GetMaxBoxSize(), att.GetMaxScale());
                }
            }

            string mainObject = "mainObj";
            Maxscript.AnimateAtTime(ParentFile.GetFrameTime(0), Maxscript.NewMeshLiteral(mainObject, vertArray, normArray, faceArray, faceMats, texVerts));
            Maxscript.Command("{0} = getNodeByName \"{0}\"", mainObject);

            if (((header.flags.HasFlag(BrgMeshFlag.COLORALPHACHANNEL) || header.flags.HasFlag(BrgMeshFlag.COLORCHANNEL)) && !header.flags.HasFlag(BrgMeshFlag.SECONDARYMESH))
                || header.flags.HasFlag(BrgMeshFlag.ANIMVERTCOLORALPHA))
            {
                //Maxscript.Command("{0}.showVertexColors = true", mainObject);
                for (int i = 0; i < vertexColors.Length; i++)
                {
                    if (header.flags.HasFlag(BrgMeshFlag.COLORALPHACHANNEL))
                    {
                        //Maxscript.Command("meshop.supportVAlphas {0}", mainObject);
                        Maxscript.Command("meshop.setVertAlpha {0} -2 {1} {2}", mainObject, i + 1, vertexColors[i].A);
                    }
                    else
                    {
                        Maxscript.Command("meshop.setVertColor {0} 0 {1} (color {2} {3} {4})", mainObject, i + 1, vertexColors[i].R, vertexColors[i].G, vertexColors[i].B);
                    }
                }
            }

            string groundPlanePos = Maxscript.NewPoint3<float>("groundPlanePos", -header.groundPos.X, -header.groundPos.Z, header.groundPos.Y);
            Maxscript.Command("plane name:\"ground\" pos:{0} length:10 width:10", groundPlanePos);

            Maxscript.CommentTitle("TVert Hack");
            Maxscript.Command("buildTVFaces {0}", mainObject);
            for (int i = 1; i <= faceVertices.Length; i++)
            {
                Maxscript.Command("setTVFace {0} {1} (getFace {0} {1})", mainObject, i);
            }

            if (header.flags.HasFlag(BrgMeshFlag.ANIMTEXCOORDS))
            {
                Maxscript.Command("select {0}", mainObject);
                Maxscript.Command("addModifier {0} (Unwrap_UVW())", mainObject);

                Maxscript.Command("select {0}.verts", mainObject);
                Maxscript.Animate("{0}.Unwrap_UVW.moveSelected [0,0,0]", mainObject);
            }

            return mainObject;
        }
        public void ImportFromMax(string mainObject, float time, int meshIndex)
        {
            UpdateSettings(meshIndex);

            header.centerPos.X = -(float)Maxscript.Query("{0}.center.x", Maxscript.QueryType.Float, mainObject);
            header.centerPos.Z = -(float)Maxscript.Query("{0}.center.y", Maxscript.QueryType.Float, mainObject);
            header.centerPos.Y = (float)Maxscript.Query("{0}.center.z", Maxscript.QueryType.Float, mainObject);

            Maxscript.Command("grnd = getNodeByName \"ground\"");
            if (!Maxscript.QueryBoolean("grnd == undefined"))
            {
                header.groundPos.X = -(float)Maxscript.Query("grnd.position.x", Maxscript.QueryType.Float);
                header.groundPos.Z = -(float)Maxscript.Query("grnd.position.y", Maxscript.QueryType.Float);
                header.groundPos.Y = (float)Maxscript.Query("grnd.position.z", Maxscript.QueryType.Float);
            }

            Maxscript.Command("{0}BBMax = {0}.max", mainObject);
            Maxscript.Command("{0}BBMin = {0}.min", mainObject);
            Vector3<float> bBoxMax = new Vector3<float>(Maxscript.QueryFloat("{0}BBMax.X", mainObject), Maxscript.QueryFloat("{0}BBMax.Y", mainObject), Maxscript.QueryFloat("{0}BBMax.Z", mainObject));
            Vector3<float> bBoxMin = new Vector3<float>(Maxscript.QueryFloat("{0}BBMin.X", mainObject), Maxscript.QueryFloat("{0}BBMin.Y", mainObject), Maxscript.QueryFloat("{0}BBMin.Z", mainObject));
            Vector3<float> bBox = (bBoxMax - bBoxMin) / 2;
            header.boundingBoxMin.X = -bBox.X;
            header.boundingBoxMin.Z = -bBox.Y;
            header.boundingBoxMin.Y = -bBox.Z;
            header.boundingBoxMax.X = bBox.X;
            header.boundingBoxMax.Z = bBox.Y;
            header.boundingBoxMax.Y = bBox.Z;

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
            vertices = verticesList.ToArray();
            normals = normalsList.ToArray();

            //System.Windows.Forms.MessageBox.Show("2");
            if (!header.flags.HasFlag(BrgMeshFlag.SECONDARYMESH) || header.flags.HasFlag(BrgMeshFlag.ANIMTEXCOORDS))
            {
                if (header.flags.HasFlag(BrgMeshFlag.TEXCOORDSA))
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
                    texVertices = texVerticesList.ToArray();
                }
            }

            //System.Windows.Forms.MessageBox.Show("3");
            HashSet<int> diffFaceMats = new HashSet<int>();
            if (!header.flags.HasFlag(BrgMeshFlag.SECONDARYMESH))
            {
                if (header.flags.HasFlag(BrgMeshFlag.MATERIAL))
                {
                    faceMaterials = new Int16[numFaces];
                    for (int i = 0; i < numFaces; i++)
                    {
                        faceMaterials[i] = (Int16)(Int32)Maxscript.Query("getFaceMatID {0} {1}", Maxscript.QueryType.Integer, mainMesh, i + 1);
                        diffFaceMats.Add(faceMaterials[i]);
                    }
                }

                //System.Windows.Forms.MessageBox.Show("3.1");
                faceVertices = new Vector3<Int16>[numFaces];
                for (int i = 0; i < faceVertices.Length; i++)
                {
                    Maxscript.Command("face = getFace {0} {1}", mainMesh, i + 1);
                    faceVertices[i].X = newVertMap[((Int32)Maxscript.Query("face.x", Maxscript.QueryType.Integer) - 1)];
                    faceVertices[i].Y = newVertMap[((Int32)Maxscript.Query("face.z", Maxscript.QueryType.Integer) - 1)];
                    faceVertices[i].Z = newVertMap[((Int32)Maxscript.Query("face.y", Maxscript.QueryType.Integer) - 1)];
                }

                //System.Windows.Forms.MessageBox.Show("3.2");
                if (header.flags.HasFlag(BrgMeshFlag.MATERIAL))
                {
                    vertMaterials = new Int16[vertices.Length];
                    for (int i = 0; i < faceVertices.Length; i++)
                    {
                        vertMaterials[faceVertices[i].X] = faceMaterials[i];
                        vertMaterials[faceVertices[i].Y] = faceMaterials[i];
                        vertMaterials[faceVertices[i].Z] = faceMaterials[i];
                    }
                }
            }

            //System.Windows.Forms.MessageBox.Show("4");
            if (meshIndex == 0 && diffFaceMats.Count > 0)
            {
                extendedHeader.materialCount = (byte)(diffFaceMats.Count - 1);
                extendedHeader.uniqueMaterialCount = diffFaceMats.Count;
            }
            extendedHeader.animTime = (float)Maxscript.Query("animationRange.end.ticks / 4800 as float", Maxscript.QueryType.Float);

            //System.Windows.Forms.MessageBox.Show("5 " + numAttachpoints);
            if (header.flags.HasFlag(BrgMeshFlag.ATTACHPOINTS))
            {
                Attachpoint = new BrgAttachpointCollection();
                for (int i = 0; i < numAttachpoints; i++)
                {
                    int index = Convert.ToInt32((Maxscript.QueryString("{0}[{1}].name", attachDummy, i + 1)).Substring(4, 2));
                    index = Attachpoint.Add(index);
                    //System.Windows.Forms.MessageBox.Show("5.1");
                    Attachpoint[index].NameId = BrgAttachpoint.GetIdByName(((string)Maxscript.Query("{0}[{1}].name", Maxscript.QueryType.String, attachDummy, i + 1)).Substring(7));
                    Maxscript.Command("{0}[{1}].name = \"{2}\"", attachDummy, i + 1, Attachpoint[index].GetMaxName());
                    //System.Windows.Forms.MessageBox.Show("5.2");
                    Maxscript.SetVarAtTime(time, "{0}Transform", "{0}[{1}].rotation as matrix3", attachDummy, i + 1);
                    Maxscript.SetVarAtTime(time, "{0}Position", "{0}[{1}].position", attachDummy, i + 1);
                    Maxscript.SetVarAtTime(time, "{0}Scale", "{0}[{1}].scale", attachDummy, i + 1);
                    //System.Windows.Forms.MessageBox.Show("5.3");
                    Vector3<float> scale = new Vector3<float>(Maxscript.QueryFloat("{0}Scale.X", attachDummy), Maxscript.QueryFloat("{0}Scale.Y", attachDummy), Maxscript.QueryFloat("{0}Scale.Z", attachDummy));
                    bBox = scale / 2;
                    //System.Windows.Forms.MessageBox.Show("5.4");

                    Attachpoint[index].x.X = -(float)Maxscript.Query("{0}Transform[1].z", Maxscript.QueryType.Float, attachDummy);
                    Attachpoint[index].x.Y = (float)Maxscript.Query("{0}Transform[3].z", Maxscript.QueryType.Float, attachDummy);
                    Attachpoint[index].x.Z = -(float)Maxscript.Query("{0}Transform[2].z", Maxscript.QueryType.Float, attachDummy);

                    Attachpoint[index].y.X = -(float)Maxscript.Query("{0}Transform[1].y", Maxscript.QueryType.Float, attachDummy);
                    Attachpoint[index].y.Y = (float)Maxscript.Query("{0}Transform[3].y", Maxscript.QueryType.Float, attachDummy);
                    Attachpoint[index].y.Z = -(float)Maxscript.Query("{0}Transform[2].y", Maxscript.QueryType.Float, attachDummy);

                    Attachpoint[index].z.X = -(float)Maxscript.Query("{0}Transform[1].x", Maxscript.QueryType.Float, attachDummy);
                    Attachpoint[index].z.Y = (float)Maxscript.Query("{0}Transform[3].x", Maxscript.QueryType.Float, attachDummy);
                    Attachpoint[index].z.Z = -(float)Maxscript.Query("{0}Transform[2].x", Maxscript.QueryType.Float, attachDummy);

                    Attachpoint[index].position.X = -(float)Maxscript.Query("{0}Position.x", Maxscript.QueryType.Float, attachDummy);
                    Attachpoint[index].position.Z = -(float)Maxscript.Query("{0}Position.y", Maxscript.QueryType.Float, attachDummy);
                    Attachpoint[index].position.Y = (float)Maxscript.Query("{0}Position.z", Maxscript.QueryType.Float, attachDummy);
                    //System.Windows.Forms.MessageBox.Show("5.5");

                    Attachpoint[index].unknown11a.X = -bBox.X;
                    Attachpoint[index].unknown11a.Z = -bBox.Y;
                    Attachpoint[index].unknown11a.Y = -bBox.Z;
                    Attachpoint[index].unknown11b.X = bBox.X;
                    Attachpoint[index].unknown11b.Z = bBox.Y;
                    Attachpoint[index].unknown11b.Y = bBox.Z;
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

        public void UpdateSettings(int meshIndex)
        {
            header.flags = ParentFile.ParentForm.Flags;
            header.format = ParentFile.ParentForm.Format;
            header.properties = ParentFile.ParentForm.Properties;
            header.interpolationType = ParentFile.ParentForm.InterpolationType;
            extendedHeader.exportedScaleFactor = ParentFile.ParentForm.TimeMult;
            if (meshIndex > 0)
            {
                header.flags |= BrgMeshFlag.SECONDARYMESH;
                header.properties &= ~BrgMeshAnimType.NONUNIFORM;
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
