namespace AoMEngineLibrary.Graphics.Brg
{
    using MiscUtil.Conversion;
    using System;
    using System.Collections.Generic;

    public class BrgFile
    {
        public string FileName;
        public MaxPluginForm ParentForm;

        public BrgHeader Header;
        public BrgAsetHeader AsetHeader;
        public List<BrgMesh> Mesh;
        public List<BrgMaterial> Material;

        public BrgFile(System.IO.FileStream fileStream, MaxPluginForm form)
            : this(fileStream)
        {
            ParentForm = form;
        }
        public BrgFile(System.IO.FileStream fileStream)
        {
            using (BrgBinaryReader reader = new BrgBinaryReader(new LittleEndianBitConverter(), fileStream))
            {
                FileName = fileStream.Name;
                string magic = reader.ReadString(4);
                if (magic != "BANG")
                {
                    throw new Exception("This is not a BRG file!");
                }
                reader.ReadHeader(ref Header);

                int asetCount = 0;
                Mesh = new List<BrgMesh>(Header.numMeshes);
                Material = new List<BrgMaterial>();
                while (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    magic = reader.ReadString(4);
                    if (magic == "ASET")
                    {
                        reader.ReadAsetHeader(ref AsetHeader);
                        ++asetCount;
                    }
                    else if (magic == "MESI")
                    {
                        Mesh.Add(new BrgMesh(reader, this));
                    }
                    else if (magic == "MTRL")
                    {
                        BrgMaterial mat = new BrgMaterial(reader, this);
                        if (!ContainsMaterialID(mat.id))
                        {
                            Material.Add(mat);
                        }
                        else
                        {
                            //throw new Exception("Duplicate material ids!");
                        }
                    }
                    else
                    {
                        throw new Exception("The type tag " +/* magic +*/ " is not recognized!");
                    }
                }

                if (asetCount > 1)
                {
                    throw new Exception("Multiple ASETs!");
                }

                if (Header.numMeshes < Mesh.Count)
                {
                    throw new Exception("Inconsistent mesh count!");
                }

                if (Header.numMaterials < Material.Count)
                {
                    throw new Exception("Inconsistent material count!");
                }

                if (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    throw new Exception("The end of stream was not reached!");
                }
            }
        }
        public BrgFile(MaxPluginForm form)
        {
            FileName = string.Empty;
            ParentForm = form;
            Header.unknown03 = 1999922179;

            Mesh = new List<BrgMesh>();
            Material = new List<BrgMaterial>();
        }

        public void Write(System.IO.FileStream fileStream)
        {
            using (BrgBinaryWriter writer = new BrgBinaryWriter(new LittleEndianBitConverter(), fileStream))
            {
                FileName = fileStream.Name;
                writer.Write(1196310850); // magic "BANG"

                Header.numMeshes = Mesh.Count;
                Header.numMaterials = Material.Count;
                Header.unknown03 = 1999922179;
                writer.WriteHeader(ref Header);

                if (Header.numMeshes > 1)
                {
                    updateAsetHeader();
                    writer.Write(1413829441); // magic "ASET"
                    writer.WriteAsetHeader(ref AsetHeader);
                }

                for (int i = 0; i < Mesh.Count; i++)
                {
                    writer.Write(1230193997); // magic "MESI"
                    Mesh[i].Write(writer);
                }

                for (int i = 0; i < Material.Count; i++)
                {
                    writer.Write(1280463949); // magic "MTRL"
                    Material[i].Write(writer);
                }
            }
        }

        public void ExportToMax()
        {
            if (Mesh.Count > 1)
            {
                Maxscript.Command("frameRate = {0}", Math.Round(Mesh.Count / Mesh[0].extendedHeader.animTime));
                Maxscript.Interval(0, Mesh[0].extendedHeader.animTime);
            }
            else
            {
                Maxscript.Command("frameRate = 1");
                Maxscript.Interval(0, 1);
            }

            if (Mesh.Count > 0)
            {
                string mainObject = Mesh[0].ExportToMax();
                //string firstFrameObject = MaxHelper.SnapshotAsMesh("firstFrameObject", mainObject, 0);

                for (int i = 1; i < Mesh.Count; i++)
                {
                    Maxscript.CommentTitle("ANIMATE FRAME " + i);
                    float time = GetFrameTime(i);

                    for (int j = 0; j < Mesh[i].vertices.Length; j++)
                    {
                        Maxscript.AnimateAtTime(time, "meshOp.setVert {0} {1} {2}", mainObject, j + 1, Maxscript.NewPoint3Literal<float>(-Mesh[i].vertices[j].X, -Mesh[i].vertices[j].Z, Mesh[i].vertices[j].Y));
                        Maxscript.AnimateAtTime(time, "setNormal {0} {1} {2}", mainObject, j + 1, Maxscript.NewPoint3Literal<float>(-Mesh[i].normals[j].X, -Mesh[i].normals[j].Z, Mesh[i].normals[j].Y));

                        // When NOTFIRSTMESH (i aleady starts from 1)
                        if (Mesh[i].header.flags.HasFlag(BrgMeshFlag.ANIMTEXCOORDS))
                        {
                            if (Mesh[i].header.flags.HasFlag(BrgMeshFlag.TEXCOORDSA))
                            {
                                Maxscript.Animate("{0}.Unwrap_UVW.SetVertexPosition {1}s {2} {3}", mainObject, time, j + 1, Maxscript.NewPoint3Literal<float>(Mesh[i].texVertices[j].X, Mesh[i].texVertices[j].Y, 0));
                            }
                        }
                    }

                    foreach (BrgAttachpoint att in Mesh[i].Attachpoint)
                    {
                        Maxscript.Command("attachpoint = getNodeByName \"{0}\"", att.GetMaxName());
                        Maxscript.AnimateAtTime(time, "attachpoint.rotation = {0}", att.GetMaxTransform());
                        Maxscript.AnimateAtTime(time, "attachpoint.position = {0}", att.GetMaxPosition());
                        Maxscript.AnimateAtTime(time, "attachpoint.scale = {0}", att.GetMaxScale());
                    }

                    if (((Mesh[i].header.flags.HasFlag(BrgMeshFlag.COLORALPHACHANNEL) || Mesh[i].header.flags.HasFlag(BrgMeshFlag.COLORCHANNEL)) && !Mesh[i].header.flags.HasFlag(BrgMeshFlag.SECONDARYMESH))
                        || Mesh[i].header.flags.HasFlag(BrgMeshFlag.ANIMVERTCOLORALPHA))
                    {
                        for (int j = 0; j < Mesh[j].vertexColors.Length; j++)
                        {
                            if (Mesh[i].header.flags.HasFlag(BrgMeshFlag.COLORALPHACHANNEL))
                            {
                                Maxscript.AnimateAtTime(time, "meshop.setVertAlpha {0} -2 {1} {2}", mainObject, j + 1, Mesh[i].vertexColors[j].A);
                            }
                            else
                            {
                                Maxscript.AnimateAtTime(time, "meshop.setVertColor {0} 0 {1} (color {2} {3} {4})", mainObject, j + 1, Mesh[i].vertexColors[j].R, Mesh[i].vertexColors[j].G, Mesh[i].vertexColors[j].B);
                            }
                        }
                    }
                }

                // Still can't figure out why it updates/overwrites normals ( geometry:false topology:false)
                // Seems like it was fixed in 3ds Max 2015 with setNormal command
                Maxscript.Command("update {0} geometry:false topology:false normals:false", mainObject);
                Maxscript.Command("select {0}", mainObject);
                Maxscript.Command("max zoomext sel all");

                if (Material.Count > 0)
                {
                    Maxscript.CommentTitle("LOAD MATERIALS");
                    Maxscript.Command("matGroup = multimaterial numsubs:{0}", Material.Count);
                    for (int i = 0; i < Material.Count; i++)
                    {
                        Maxscript.Command("matGroup[{0}] = {1}", i + 1, Material[i].ExportToMax());
                        Maxscript.Command("matGroup.materialIDList[{0}] = {1}", i + 1, Material[i].id);
                    }
                    Maxscript.Command("{0}.material = matGroup", mainObject);
                }
            }
        }
        public void ImportFromMax(bool execute)
        {
            string mainObject = "mainObject";
            Maxscript.Command("{0} = selection[1]", mainObject);
            bool objectSelected = Maxscript.QueryBoolean("classOf {0} == Editable_mesh", mainObject);
            if (!objectSelected)
            {
                throw new Exception("No object selected!");
            }

            HashSet<float> keys = new HashSet<float>();
            if (ParentForm.Flags.HasFlag(BrgMeshFlag.ANIMTEXCOORDS))
            {
                int numTexKeys = Maxscript.QueryInteger("{0}.Unwrap_UVW[1].keys.count", mainObject);
                for (int i = 0; i < numTexKeys; i++)
                {
                    keys.Add(Maxscript.QueryFloat("{0}.Unwrap_UVW[1].keys[{1}].time as float", mainObject, i + 1) / 4800f);
                }
            }
            if (Maxscript.QueryBoolean("{0}.modifiers[#skin] != undefined", mainObject))
            {
                //System.Windows.Forms.MessageBox.Show("b1");
                Maxscript.Command("max modify mode");
                int numBones = Maxscript.QueryInteger("skinops.getnumberbones {0}.modifiers[#skin]", mainObject);
                for (int i = 0; i < numBones; i++)
                {
                    string boneName = Maxscript.QueryString("skinops.getbonename {0}.modifiers[#skin] {1} 0", mainObject, i + 1);
                    Maxscript.Command("boneNode = getNodeByName \"{0}\"", boneName);
                    int numBoneKeys = Maxscript.QueryInteger("boneNode.position.controller.keys.count");
                    //System.Windows.Forms.MessageBox.Show("b1.1");
                    for (int j = 0; j < numBoneKeys; j++)
                    {
                        keys.Add(Maxscript.QueryFloat("boneNode.position.controller.keys[{0}].time as float", j + 1) / 4800f);
                    }
                    numBoneKeys = Maxscript.QueryInteger("boneNode.rotation.controller.keys.count");
                    //System.Windows.Forms.MessageBox.Show("b1.2 " + numKeys);
                    for (int j = 0; j < numBoneKeys; j++)
                    {
                        //System.Windows.Forms.MessageBox.Show("b1.2.1 " + String.Format("boneNode.rotation.controller.keys[{0}] as float", j + 1));
                        keys.Add(Maxscript.QueryFloat("boneNode.rotation.controller.keys[{0}].time as float", j + 1) / 4800f);
                    }
                    numBoneKeys = Maxscript.QueryInteger("boneNode.scale.controller.keys.count");
                    //System.Windows.Forms.MessageBox.Show("b1.3");
                    for (int j = 0; j < numBoneKeys; j++)
                    {
                        //System.Windows.Forms.MessageBox.Show("b1.3.1");
                        keys.Add(Maxscript.QueryFloat("boneNode.scale.controller.keys[{0}].time as float", j + 1) / 4800f);
                    }
                    numBoneKeys = Maxscript.QueryInteger("boneNode.controller.keys.count");
                    //System.Windows.Forms.MessageBox.Show("b1.4");
                    for (int j = 0; j < numBoneKeys; j++)
                    {
                        //System.Windows.Forms.MessageBox.Show("b1.4.1");
                        keys.Add(Maxscript.QueryFloat("boneNode.controller.keys[{0}].time as float", j + 1) / 4800f);
                    }
                }
            }
            int numKeys = (int)Maxscript.Query("{0}.baseobject.mesh[1].keys.count", Maxscript.QueryType.Integer, mainObject);
            //System.Windows.Forms.MessageBox.Show(numKeys + " ");
            for (int i = 0; i < numKeys; i++)
            {
                keys.Add(Maxscript.QueryFloat("{0}.baseobject.mesh[1].keys[{1}].time as float", mainObject, i + 1) / 4800f);
            }
            Header.numMeshes = keys.Count;
            if (Header.numMeshes <= 0)
            {
                Header.numMeshes = 1;
                keys.Add(0);
            }
            List<float> keyTime = new List<float>(keys);
            keyTime.Sort();

            //fixTverts(mainObject);

            Header.numMaterials = (int)Maxscript.Query("{0}.material.materialList.count", Maxscript.QueryType.Integer, mainObject);
            //System.Windows.Forms.MessageBox.Show(Header.numMeshes + " " + Header.numMaterials);
            if (Header.numMaterials > 0)
            {
                Material = new List<BrgMaterial>(Header.numMaterials);
                for (int i = 0; i < Header.numMaterials; i++)
                {
                    Material.Add(new BrgMaterial(this));
                    Material[i].ImportFromMax(mainObject, i);
                }
            }

            Mesh = new List<BrgMesh>(Header.numMeshes);
            for (int i = 0; i < Header.numMeshes; i++)
            {
                Mesh.Add(new BrgMesh(this));
                Mesh[i].ImportFromMax(mainObject, keyTime[i], i);
            }

            if (Mesh[0].header.properties.HasFlag(BrgMeshAnimType.NONUNIFORM))
            {
                Mesh[0].animTimeAdjust = new float[Mesh.Count];
                for (int i = 0; i < Mesh.Count; i++)
                {
                    Mesh[0].animTimeAdjust[i] = keyTime[i] / Mesh[0].extendedHeader.animTime;
                }
            }
            updateAsetHeader();

            //Maxscript.Command("delete {0}", mainObject);
        }
        private void fixTverts(string mainObject)
        {
            // Figure out the used vertices, and corresponding tex verts
            int numVertices = (int)Maxscript.Query("{0}.numverts", Maxscript.QueryType.Integer, mainObject);
            int numFaces = (int)Maxscript.Query("{0}.numfaces", Maxscript.QueryType.Integer, mainObject);
            List<int> vertexMask = new List<int>(numVertices);
            HashSet<int> verticesToBreak = new HashSet<int>();
            for (int i = 0; i < numVertices; i++)
            {
                vertexMask.Add(0);
            }
            for (int i = 0; i < numFaces; i++)
            {
                Maxscript.Command("face = getFace {0} {1}", mainObject, i + 1);
                Maxscript.Command("tFace = getTVFace {0} {1}", mainObject, i + 1);
                int vert1 = Maxscript.QueryInteger("face[1]") - 1;
                int vert2 = Maxscript.QueryInteger("face[2]") - 1;
                int vert3 = Maxscript.QueryInteger("face[3]") - 1;
                int tVert1 = Maxscript.QueryInteger("tFace[1]");
                int tVert2 = Maxscript.QueryInteger("tFace[2]");
                int tVert3 = Maxscript.QueryInteger("tFace[3]");
                if (vertexMask[vert1] > 0 && vertexMask[vert1] != tVert1)
                {
                    verticesToBreak.Add(vert1 + 1);
                }
                if (vertexMask[vert2] > 0 && vertexMask[vert2] != tVert2)
                {
                    verticesToBreak.Add(vert2 + 1);
                }
                if (vertexMask[vert3] > 0 && vertexMask[vert3] != tVert3)
                {
                    verticesToBreak.Add(vert3 + 1);
                }

                vertexMask[vert1] = tVert1;
                vertexMask[vert2] = tVert2;
                vertexMask[vert3] = tVert3;
            }
            if (verticesToBreak.Count > 0)
            {
                System.Windows.Forms.MessageBox.Show("dd");
                string vertsToBreak = Maxscript.NewBitArray("vertsToBreak");
                foreach (int i in verticesToBreak)
                {
                    Maxscript.Command("{0}[{1}] = true", vertsToBreak, i);
                    //Maxscript.Append(vertsToBreak, i);
                }
                Maxscript.Command("meshop.breakVerts {0} {1}", mainObject, vertsToBreak);
                //Maxscript.Command("print {0}", vertsToBreak);
            }
        }

        public float GetFrameTime(int meshIndex)
        {
            if (Mesh[0].header.properties.HasFlag(BrgMeshAnimType.NONUNIFORM))
            {
                //System.Windows.Forms.MessageBox.Show("t1");
                return Mesh[0].animTimeAdjust[meshIndex] * Mesh[0].extendedHeader.animTime;
            }
            else if (Mesh.Count > 1)
            {
                //System.Windows.Forms.MessageBox.Show("t2");
                return (float)meshIndex / ((float)Mesh.Count - 1f) * Mesh[0].extendedHeader.animTime;
            }
            else
            {
                //System.Windows.Forms.MessageBox.Show("t3");
                return 0;
            }
        }
        public bool ContainsMaterialID(int id)
        {
            for (int i = 0; i < Material.Count; i++)
            {
                if (Material[i].id == id)
                {
                    return true;
                }
            }

            return false;
        }
        private void updateAsetHeader()
        {
            AsetHeader.numFrames = Mesh.Count;
            AsetHeader.frameStep = 1f / (float)AsetHeader.numFrames;
            AsetHeader.animTime = Mesh[0].extendedHeader.animTime;
            AsetHeader.frequency = 1f / (float)AsetHeader.animTime;
            AsetHeader.spf = AsetHeader.animTime / (float)AsetHeader.numFrames;
            AsetHeader.fps = (float)AsetHeader.numFrames / AsetHeader.animTime;
        }
    }
}
