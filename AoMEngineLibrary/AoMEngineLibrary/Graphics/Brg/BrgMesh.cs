namespace AoMEngineLibrary.Graphics.Brg
{
    using AoMEngineLibrary.Graphics.Model;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class BrgMesh : Mesh
    {
        public BrgFile ParentFile { get; set; }
        public BrgMeshHeader Header { get; set; }

        public Int16[] VertexMaterials { get; set; }

        public BrgMeshExtendedHeader ExtendedHeader { get; set; }
        public BrgUserDataEntry[] UserDataEntries { get; set; }
        private float[] particleData;

        public BrgAttachpointCollection Attachpoints { get; set; }
        public float[] NonUniformKeys { get; set; }

        public BrgMesh(BrgBinaryReader reader, BrgFile file)
            : base()
        {
            this.ParentFile = file;
            this.Header = new BrgMeshHeader(reader);

            this.VertexMaterials = new Int16[0];
            if (!this.Header.Flags.HasFlag(BrgMeshFlag.PARTICLEPOINTS))
            {
                this.Vertices = new List<Vector3D>(this.Header.NumVertices);
                for (int i = 0; i < this.Header.NumVertices; i++)
                {
                    this.Vertices.Add(reader.ReadVector3D(true, this.Header.Version == 22));
                }
                this.Normals = new List<Vector3D>(this.Header.NumVertices);
                for (int i = 0; i < this.Header.NumVertices; i++)
                {
                    if (this.Header.Version >= 13 && this.Header.Version <= 17)
                    {
                        reader.ReadInt16(); // No idea what this is
                    }
                    else // v == 18, 19 or 22
                    {
                        this.Normals.Add(reader.ReadVector3D(true, this.Header.Version == 22));
                    }
                }

                if ((!this.Header.Flags.HasFlag(BrgMeshFlag.SECONDARYMESH) ||
                    this.Header.Flags.HasFlag(BrgMeshFlag.ANIMTEXCOORDS) ||
                    this.Header.Flags.HasFlag(BrgMeshFlag.PARTICLESYSTEM)) &&
                    this.Header.Flags.HasFlag(BrgMeshFlag.TEXCOORDSA))
                {
                    this.TextureCoordinates = new List<Vector3D>(this.Header.NumVertices);
                    for (int i = 0; i < this.Header.NumVertices; i++)
                    {
                        this.TextureCoordinates.Add(new Vector3D(reader.ReadVector2D(this.Header.Version == 22), 0f));
                    }
                }

                if (!this.Header.Flags.HasFlag(BrgMeshFlag.SECONDARYMESH) ||
                    this.Header.Flags.HasFlag(BrgMeshFlag.PARTICLESYSTEM))
                {
                    this.Faces = new List<Face>(this.Header.NumFaces);
                    for (int i = 0; i < this.Header.NumFaces; ++i)
                    {
                        this.Faces.Add(new Face());
                    }

                    if (this.Header.Flags.HasFlag(BrgMeshFlag.MATERIAL))
                    {
                        for (int i = 0; i < this.Header.NumFaces; i++)
                        {
                            this.Faces[i].MaterialIndex = reader.ReadInt16();
                        }
                    }

                    for (int i = 0; i < this.Header.NumFaces; i++)
                    {
                        this.Faces[i].Indices.Add(reader.ReadInt16());
                        this.Faces[i].Indices.Add(reader.ReadInt16());
                        this.Faces[i].Indices.Add(reader.ReadInt16());
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
                    attpts[i].XVector = reader.ReadVector3D(true, this.Header.Version == 22);
                }
                for (int i = 0; i < numMatrix; i++)
                {
                    attpts[i].YVector = reader.ReadVector3D(true, this.Header.Version == 22);
                }
                for (int i = 0; i < numMatrix; i++)
                {
                    attpts[i].ZVector = reader.ReadVector3D(true, this.Header.Version == 22);
                }
                if (this.Header.Version == 19 || this.Header.Version == 22)
                {
                    for (int i = 0; i < numMatrix; i++)
                    {
                        attpts[i].Position = reader.ReadVector3D(true, this.Header.Version == 22);
                    }
                }
                for (int i = 0; i < numMatrix; i++)
                {
                    attpts[i].BoundingBoxMin = reader.ReadVector3D(true, this.Header.Version == 22);
                }
                for (int i = 0; i < numMatrix; i++)
                {
                    attpts[i].BoundingBoxMax = reader.ReadVector3D(true, this.Header.Version == 22);
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

            if (((this.Header.Flags.HasFlag(BrgMeshFlag.COLORALPHACHANNEL) ||
                this.Header.Flags.HasFlag(BrgMeshFlag.COLORCHANNEL)) &&
                !this.Header.Flags.HasFlag(BrgMeshFlag.SECONDARYMESH)) ||
                this.Header.Flags.HasFlag(BrgMeshFlag.ANIMVERTCOLORALPHA))
            {
                this.Colors = new List<Color4D>(this.Header.NumVertices);
                for (int i = 0; i < this.Header.NumVertices; i++)
                {
                    this.Colors.Add(reader.ReadVertexColor());
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
                Vector3D[] legacy = new Vector3D[this.Header.NumFaces];
                for (int i = 0; i < this.Header.NumFaces; i++)
                {
                    legacy[i] = reader.ReadVector3D();
                }
            }

            if (!this.Header.Flags.HasFlag(BrgMeshFlag.SECONDARYMESH) && this.Header.Version != 22)
            {
                reader.ReadBytes(ExtendedHeader.ShadowNameLength0 + ExtendedHeader.ShadowNameLength1);
            }
        }
        public BrgMesh(BrgFile file)
            : base()
        {
            this.ParentFile = file;
            this.Header = new BrgMeshHeader();
            this.Header.Version = 22;
            this.Header.ExtendedHeaderSize = 40;

            this.VertexMaterials = new Int16[0];

            this.ExtendedHeader = new BrgMeshExtendedHeader();
            this.ExtendedHeader.MaterialLibraryTimestamp = 191738312;
            this.ExtendedHeader.ExportedScaleFactor = 1f;

            this.UserDataEntries = new BrgUserDataEntry[0];
            this.particleData = new float[0];

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
                this.Header.NumVertices = (Int16)this.Vertices.Count;
            }
            this.Header.NumFaces = (Int16)this.Faces.Count;
            this.Header.UserDataEntryCount = (Int16)this.UserDataEntries.Length;
            this.Header.CenterRadius = this.Header.MaximumExtent.LongestAxisLength();
            this.Header.ExtendedHeaderSize = 40;
            this.Header.Write(writer);

            if (!this.Header.Flags.HasFlag(BrgMeshFlag.PARTICLEPOINTS))
            {
                for (int i = 0; i < this.Vertices.Count; i++)
                {
                    writer.WriteVector3D(this.Vertices[i], true, true);
                }
                for (int i = 0; i < this.Vertices.Count; i++)
                {
                    writer.WriteVector3D(this.Normals[i], true, true);
                }

                if ((!this.Header.Flags.HasFlag(BrgMeshFlag.SECONDARYMESH) ||
                    this.Header.Flags.HasFlag(BrgMeshFlag.ANIMTEXCOORDS) ||
                    this.Header.Flags.HasFlag(BrgMeshFlag.PARTICLESYSTEM)) &&
                    this.Header.Flags.HasFlag(BrgMeshFlag.TEXCOORDSA))
                {
                    for (int i = 0; i < this.TextureCoordinates.Count; i++)
                    {
                        writer.WriteHalf(this.TextureCoordinates[i].X);
                        writer.WriteHalf(this.TextureCoordinates[i].Y);
                    }
                }

                if (!this.Header.Flags.HasFlag(BrgMeshFlag.SECONDARYMESH) ||
                    this.Header.Flags.HasFlag(BrgMeshFlag.PARTICLESYSTEM))
                {
                    if (this.Header.Flags.HasFlag(BrgMeshFlag.MATERIAL))
                    {
                        for (int i = 0; i < this.Faces.Count; i++)
                        {
                            writer.Write(this.Faces[i].MaterialIndex);
                        }
                    }

                    for (int i = 0; i < this.Faces.Count; i++)
                    {
                        writer.Write((Int16)this.Faces[i].Indices[0]);
                        writer.Write((Int16)this.Faces[i].Indices[1]);
                        writer.Write((Int16)this.Faces[i].Indices[2]);
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
                    writer.WriteVector3D(att.XVector, true, true);
                }
                foreach (BrgAttachpoint att in this.Attachpoints)
                {
                    writer.WriteVector3D(att.YVector, true, true);
                }
                foreach (BrgAttachpoint att in this.Attachpoints)
                {
                    writer.WriteVector3D(att.ZVector, true, true);
                }
                foreach (BrgAttachpoint att in this.Attachpoints)
                {
                    writer.WriteVector3D(att.Position, true, true);
                }
                foreach (BrgAttachpoint att in this.Attachpoints)
                {
                    writer.WriteVector3D(att.BoundingBoxMin, true, true);
                }
                foreach (BrgAttachpoint att in this.Attachpoints)
                {
                    writer.WriteVector3D(att.BoundingBoxMax, true, true);
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
                for (int i = 0; i < this.Colors.Count; i++)
                {
                    writer.Write((Byte)(this.Colors[i].R * Byte.MaxValue));
                    writer.Write((Byte)(this.Colors[i].G * Byte.MaxValue));
                    writer.Write((Byte)(this.Colors[i].B * Byte.MaxValue));
                    writer.Write((Byte)(this.Colors[i].A * Byte.MaxValue));
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
