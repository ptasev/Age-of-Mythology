namespace AoMEngineLibrary.Graphics.Brg
{
    using MiscUtil.Conversion;
    using System;
    using System.Collections.Generic;

    public class BrgFile
    {
        public string FileName { get; set; }

        public BrgHeader Header { get; set; }
        public BrgAsetHeader AsetHeader { get; set; }
        public List<BrgMesh> Meshes
        {
            get;
            set;
        }
        public List<BrgMaterial> Materials
        {
            get;
            set;
        }

        public BrgFile(System.IO.FileStream fileStream)
        {
            using (BrgBinaryReader reader = new BrgBinaryReader(new LittleEndianBitConverter(), fileStream))
            {
                this.FileName = fileStream.Name;
                this.Header = new BrgHeader(reader);
                if (this.Header.Magic != "BANG")
                {
                    throw new Exception("This is not a BRG file!");
                }

                int asetCount = 0;
                this.Meshes = new List<BrgMesh>(this.Header.NumMeshes);
                this.Materials = new List<BrgMaterial>();
                while (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    string magic = reader.ReadString(4);
                    if (magic == "ASET")
                    {
                        this.AsetHeader = new BrgAsetHeader(reader);
                        ++asetCount;
                    }
                    else if (magic == "MESI")
                    {
                        this.Meshes.Add(new BrgMesh(reader, this));
                    }
                    else if (magic == "MTRL")
                    {
                        BrgMaterial mat = new BrgMaterial(reader, this);
                        if (!ContainsMaterialID(mat.id))
                        {
                            Materials.Add(mat);
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
                    //throw new Exception("Multiple ASETs!");
                }

                if (Header.NumMeshes < Meshes.Count)
                {
                    throw new Exception("Inconsistent mesh count!");
                }

                if (Header.NumMaterials < Materials.Count)
                {
                    throw new Exception("Inconsistent material count!");
                }

                if (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    throw new Exception("The end of stream was not reached!");
                }
            }
        }
        public BrgFile()
        {
            this.FileName = string.Empty;
            this.Header = new BrgHeader();
            this.Header.Unknown03 = 1999922179;
            this.AsetHeader = new BrgAsetHeader();

            this.Meshes = new List<BrgMesh>();
            this.Materials = new List<BrgMaterial>();
        }

        public void Write(System.IO.FileStream fileStream)
        {
            using (BrgBinaryWriter writer = new BrgBinaryWriter(new LittleEndianBitConverter(), fileStream))
            {
                this.FileName = fileStream.Name;

                this.Header.NumMeshes = this.Meshes.Count;
                this.Header.NumMaterials = this.Materials.Count;
                this.Header.Write(writer);

                if (this.Header.NumMeshes > 1)
                {
                    updateAsetHeader();
                    writer.Write(1413829441); // magic "ASET"
                    this.AsetHeader.Write(writer);
                }

                for (int i = 0; i < this.Meshes.Count; i++)
                {
                    writer.Write(1230193997); // magic "MESI"
                    this.Meshes[i].Write(writer);
                }

                for (int i = 0; i < this.Materials.Count; i++)
                {
                    writer.Write(1280463949); // magic "MTRL"
                    this.Materials[i].Write(writer);
                }
            }
        }

        public void ExportToMax()
        {
            if (Meshes.Count > 1)
            {
                Maxscript.Command("frameRate = {0}", Math.Round(Meshes.Count / Meshes[0].ExtendedHeader.AnimationLength));
                Maxscript.Interval(0, Meshes[0].ExtendedHeader.AnimationLength);
            }
            else
            {
                Maxscript.Command("frameRate = 1");
                Maxscript.Interval(0, 1);
            }

            if (Meshes.Count > 0)
            {
                string mainObject = "mainObj";
                for (int i = 0; i < Meshes.Count; i++)
                {
                    Maxscript.CommentTitle("ANIMATE FRAME " + i);
                    this.Meshes[i].ExportToMax(mainObject, GetFrameTime(i));
                }

                // Still can't figure out why it updates/overwrites normals ( geometry:false topology:false)
                // Seems like it was fixed in 3ds Max 2015 with setNormal command
                Maxscript.Command("update {0} geometry:false topology:false normals:false", mainObject);
                Maxscript.Command("select {0}", mainObject);
                Maxscript.Command("max zoomext sel all");

                if (Materials.Count > 0)
                {
                    Maxscript.CommentTitle("LOAD MATERIALS");
                    Maxscript.Command("matGroup = multimaterial numsubs:{0}", Materials.Count);
                    for (int i = 0; i < Materials.Count; i++)
                    {
                        Maxscript.Command("matGroup[{0}] = {1}", i + 1, Materials[i].ExportToMax());
                        Maxscript.Command("matGroup.materialIDList[{0}] = {1}", i + 1, Materials[i].id);
                    }
                    Maxscript.Command("{0}.material = matGroup", mainObject);
                }
            }
        }
        public void ImportFromMax(BrgMeshFlag flags, BrgMeshFormat format, BrgMeshAnimType animType, byte interpolationType, float exportedScaleFactor)
        {
            string mainObject = "mainObject";
            Maxscript.Command("{0} = selection[1]", mainObject);
            bool objectSelected = Maxscript.QueryBoolean("classOf {0} == Editable_mesh", mainObject);
            if (!objectSelected)
            {
                throw new Exception("No object selected!");
            }

            HashSet<float> keys = new HashSet<float>();
            if (flags.HasFlag(BrgMeshFlag.ANIMTEXCOORDS))
            {
                Maxscript.Command("max modify mode");
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
            Header.NumMeshes = keys.Count;
            if (Header.NumMeshes <= 0)
            {
                Header.NumMeshes = 1;
                keys.Add(0);
            }
            List<float> keyTime = new List<float>(keys);
            keyTime.Sort();

            //fixTverts(mainObject);

            Header.NumMaterials = (int)Maxscript.Query("{0}.material.materialList.count", Maxscript.QueryType.Integer, mainObject);
            //System.Windows.Forms.MessageBox.Show(Header.numMeshes + " " + Header.numMaterials);
            if (Header.NumMaterials > 0)
            {
                Materials = new List<BrgMaterial>(Header.NumMaterials);
                for (int i = 0; i < Header.NumMaterials; i++)
                {
                    Materials.Add(new BrgMaterial(this));
                    Materials[i].ImportFromMax(mainObject, i);
                }
            }

            Meshes = new List<BrgMesh>(Header.NumMeshes);
            for (int i = 0; i < Header.NumMeshes; i++)
            {
                Meshes.Add(new BrgMesh(this));
                this.UpdateMeshSettings(i, flags, format, animType, interpolationType, exportedScaleFactor);
                Meshes[i].ImportFromMax(mainObject, keyTime[i]);
            }

            if (Meshes[0].Header.AnimationType.HasFlag(BrgMeshAnimType.NONUNIFORM))
            {
                Meshes[0].NonUniformKeys = new float[Meshes.Count];
                for (int i = 0; i < Meshes.Count; i++)
                {
                    Meshes[0].NonUniformKeys[i] = keyTime[i] / Meshes[0].ExtendedHeader.AnimationLength;
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
            if (Meshes[0].Header.AnimationType.HasFlag(BrgMeshAnimType.NONUNIFORM))
            {
                //System.Windows.Forms.MessageBox.Show("t1");
                return Meshes[0].NonUniformKeys[meshIndex] * Meshes[0].ExtendedHeader.AnimationLength;
            }
            else if (Meshes.Count > 1)
            {
                //System.Windows.Forms.MessageBox.Show("t2");
                return (float)meshIndex / ((float)Meshes.Count - 1f) * Meshes[0].ExtendedHeader.AnimationLength;
            }
            else
            {
                //System.Windows.Forms.MessageBox.Show("t3");
                return 0;
            }
        }
        public bool ContainsMaterialID(int id)
        {
            for (int i = 0; i < Materials.Count; i++)
            {
                if (Materials[i].id == id)
                {
                    return true;
                }
            }

            return false;
        }
        private void updateAsetHeader()
        {
            AsetHeader.numFrames = Meshes.Count;
            AsetHeader.frameStep = 1f / (float)AsetHeader.numFrames;
            AsetHeader.animTime = Meshes[0].ExtendedHeader.AnimationLength;
            AsetHeader.frequency = 1f / (float)AsetHeader.animTime;
            AsetHeader.spf = AsetHeader.animTime / (float)AsetHeader.numFrames;
            AsetHeader.fps = (float)AsetHeader.numFrames / AsetHeader.animTime;
        }

        public void UpdateMeshSettings(BrgMeshFlag flags, BrgMeshFormat format, BrgMeshAnimType animType, byte interpolationType, float exportedScaleFactor)
        {
            for (int i = 0; i < this.Meshes.Count; i++)
            {
                UpdateMeshSettings(i, flags, format, animType, interpolationType, exportedScaleFactor);
            }
        }
        public void UpdateMeshSettings(int meshIndex, BrgMeshFlag flags, BrgMeshFormat format, BrgMeshAnimType animType, byte interpolationType, float exportedScaleFactor)
        {
            this.Meshes[meshIndex].Header.Flags = flags;
            this.Meshes[meshIndex].Header.Format = format;
            this.Meshes[meshIndex].Header.AnimationType = animType;
            this.Meshes[meshIndex].Header.InterpolationType = interpolationType;
            this.Meshes[meshIndex].ExtendedHeader.ExportedScaleFactor = exportedScaleFactor;
            if (meshIndex > 0)
            {
                this.Meshes[meshIndex].Header.Flags |= BrgMeshFlag.SECONDARYMESH;
                this.Meshes[meshIndex].Header.AnimationType &= ~BrgMeshAnimType.NONUNIFORM;
            }
        }
    }
}
