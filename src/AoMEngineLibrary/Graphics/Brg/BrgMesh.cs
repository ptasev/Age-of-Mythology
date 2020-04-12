namespace AoMEngineLibrary.Graphics.Brg
{
    using AoMEngineLibrary.Graphics.Model;
    using Extensions;
    using Newtonsoft.Json;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Numerics;

    public class BrgMesh
    {
        public BrgFile ParentFile { get; set; }
        public BrgMeshHeader Header { get; set; }

        public List<Vector3> Vertices { get; set; }
        public List<Vector3> Normals { get; set; }
        public List<BrgFace> Faces { get; set; }

        public List<Vector2> TextureCoordinates { get; set; }
        public List<Vector4> Colors { get; set; }
        public List<Int16> VertexMaterials { get; set; }

        public BrgMeshExtendedHeader ExtendedHeader { get; set; }
        public BrgUserDataEntry[] UserDataEntries { get; set; }
        private float[] particleData;

        public List<BrgAttachpoint> Attachpoints { get; set; }
        public List<float> NonUniformKeys { get; set; }

        public BrgMesh(BrgFile file)
        {
            this.ParentFile = file;
            this.Header = new BrgMeshHeader();
            this.Header.Version = 22;
            this.Header.ExtendedHeaderSize = 40;

            this.Vertices = new List<Vector3>();
            this.Normals = new List<Vector3>();
            this.Faces = new List<BrgFace>();

            this.TextureCoordinates = new List<Vector2>();
            this.Colors = new List<Vector4>();
            this.VertexMaterials = new List<Int16>();

            this.ExtendedHeader = new BrgMeshExtendedHeader();
            this.ExtendedHeader.MaterialLibraryTimestamp = 191738312;
            this.ExtendedHeader.ExportedScaleFactor = 1f;

            this.UserDataEntries = new BrgUserDataEntry[0];
            this.particleData = new float[0];

            this.Attachpoints = new List<BrgAttachpoint>();
            this.NonUniformKeys = new List<float>();
        }
        public BrgMesh(BrgBinaryReader reader, BrgFile file)
            : this(file)
        {
            this.ParentFile = file;
            this.Header = new BrgMeshHeader(reader);
            
            if (!this.Header.Flags.HasFlag(BrgMeshFlag.PARTICLEPOINTS))
            {
                this.Vertices = new List<Vector3>(this.Header.NumVertices);
                for (int i = 0; i < this.Header.NumVertices; i++)
                {
                    this.Vertices.Add(reader.ReadVector3D(this.Header.Version == 22));
                }
                this.Normals = new List<Vector3>(this.Header.NumVertices);
                for (int i = 0; i < this.Header.NumVertices; i++)
                {
                    if (this.Header.Version >= 13 && this.Header.Version <= 17)
                    {
                        reader.ReadInt16(); // TODO figure this out
                    }
                    else // v == 18, 19 or 22
                    {
                        this.Normals.Add(reader.ReadVector3D(this.Header.Version == 22));
                    }
                }

                if ((!this.Header.Flags.HasFlag(BrgMeshFlag.SECONDARYMESH) ||
                    this.Header.Flags.HasFlag(BrgMeshFlag.ANIMTEXCOORDS) ||
                    this.Header.Flags.HasFlag(BrgMeshFlag.PARTICLESYSTEM)) &&
                    this.Header.Flags.HasFlag(BrgMeshFlag.TEXCOORDSA))
                {
                    this.TextureCoordinates = new List<Vector2>(this.Header.NumVertices);
                    for (int i = 0; i < this.Header.NumVertices; i++)
                    {
                        this.TextureCoordinates.Add(reader.ReadVector2D(this.Header.Version == 22));
                    }
                }

                if (!this.Header.Flags.HasFlag(BrgMeshFlag.SECONDARYMESH) ||
                    this.Header.Flags.HasFlag(BrgMeshFlag.PARTICLESYSTEM))
                {
                    this.Faces = new List<BrgFace>(this.Header.NumFaces);
                    for (int i = 0; i < this.Header.NumFaces; ++i)
                    {
                        this.Faces.Add(new BrgFace());
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
                        this.VertexMaterials = new List<Int16>(this.Header.NumVertices);
                        for (int i = 0; i < this.Header.NumVertices; i++)
                        {
                            this.VertexMaterials.Add(reader.ReadInt16());
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
                    attpts[i].Up = reader.ReadVector3D(this.Header.Version == 22);
                }
                for (int i = 0; i < numMatrix; i++)
                {
                    attpts[i].Forward = reader.ReadVector3D(this.Header.Version == 22);
                }
                for (int i = 0; i < numMatrix; i++)
                {
                    attpts[i].Right = reader.ReadVector3D(this.Header.Version == 22);
                }
                if (this.Header.Version == 19 || this.Header.Version == 22)
                {
                    for (int i = 0; i < numMatrix; i++)
                    {
                        attpts[i].Position = reader.ReadVector3D(this.Header.Version == 22);
                    }
                }
                for (int i = 0; i < numMatrix; i++)
                {
                    attpts[i].BoundingBoxMin = reader.ReadVector3D(this.Header.Version == 22);
                }
                for (int i = 0; i < numMatrix; i++)
                {
                    attpts[i].BoundingBoxMax = reader.ReadVector3D(this.Header.Version == 22);
                }

                List<int> nameId = new List<int>();
                for (int i = 0; i < numIndex; i++)
                {
                    int duplicate = reader.ReadInt32(); // have yet to find a model with duplicates
                    reader.ReadInt32(); // TODO figure out what this means
                    for (int j = 0; j < duplicate; j++)
                    {
                        nameId.Add(i);
                    }
                }
                
                for (int i = 0; i < nameId.Count; i++)
                {
                    this.Attachpoints.Add(new BrgAttachpoint(attpts[reader.ReadByte()]));
                    this.Attachpoints[i].NameId = nameId[i];
                    //attpts[reader.ReadByte()].NameId = nameId[i];
                }
                //attachpoints = new List<BrgAttachpoint>(attpts);
            }

            if (((this.Header.Flags.HasFlag(BrgMeshFlag.COLORALPHACHANNEL) ||
                this.Header.Flags.HasFlag(BrgMeshFlag.COLORCHANNEL)) &&
                !this.Header.Flags.HasFlag(BrgMeshFlag.SECONDARYMESH)) ||
                this.Header.Flags.HasFlag(BrgMeshFlag.ANIMVERTCOLORALPHA))
            {
                this.Colors = new List<Vector4>(this.Header.NumVertices);
                for (int i = 0; i < this.Header.NumVertices; i++)
                {
                    this.Colors.Add(reader.ReadTexel());
                }
            }

            // Only seen on first mesh
            if (this.Header.AnimationType.HasFlag(BrgMeshAnimType.NonUniform))
            {
                this.NonUniformKeys = new List<float>(this.ExtendedHeader.NumNonUniformKeys);
                for (int i = 0; i < this.ExtendedHeader.NumNonUniformKeys; i++)
                {
                    this.NonUniformKeys.Add(reader.ReadSingle());
                }
            }

            if (this.Header.Version >= 14 && this.Header.Version <= 19)
            {
                // Face Normals??
                Vector3[] legacy = new Vector3[this.Header.NumFaces];
                for (int i = 0; i < this.Header.NumFaces; i++)
                {
                    legacy[i] = reader.ReadVector3D(false);
                }
            }

            if (!this.Header.Flags.HasFlag(BrgMeshFlag.SECONDARYMESH) && this.Header.Version != 22)
            {
                reader.ReadBytes(ExtendedHeader.ShadowNameLength0 + ExtendedHeader.ShadowNameLength1);
            }
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
            this.Header.CenterRadius = Math.Max(Math.Max(this.Header.MaximumExtent.X, this.Header.MaximumExtent.Y), this.Header.MaximumExtent.Z);
            this.Header.ExtendedHeaderSize = 40;
            this.Header.Write(writer);

            if (!this.Header.Flags.HasFlag(BrgMeshFlag.PARTICLEPOINTS))
            {
                for (int i = 0; i < this.Vertices.Count; i++)
                {
                    writer.WriteVector3D(this.Vertices[i], true);
                }
                for (int i = 0; i < this.Vertices.Count; i++)
                {
                    writer.WriteVector3D(this.Normals[i], true);
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
                        for (int i = 0; i < this.VertexMaterials.Count; i++)
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

            this.ExtendedHeader.NumNonUniformKeys = NonUniformKeys.Count;
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
                    writer.WriteVector3D(att.Up, true);
                }
                foreach (BrgAttachpoint att in this.Attachpoints)
                {
                    writer.WriteVector3D(att.Forward, true);
                }
                foreach (BrgAttachpoint att in this.Attachpoints)
                {
                    writer.WriteVector3D(att.Right, true);
                }
                foreach (BrgAttachpoint att in this.Attachpoints)
                {
                    writer.WriteVector3D(att.Position, true);
                }
                foreach (BrgAttachpoint att in this.Attachpoints)
                {
                    writer.WriteVector3D(att.BoundingBoxMin, true);
                }
                foreach (BrgAttachpoint att in this.Attachpoints)
                {
                    writer.WriteVector3D(att.BoundingBoxMax, true);
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
                    writer.WriteTexel(this.Colors[i]);
                }
            }

            if (this.Header.AnimationType.HasFlag(BrgMeshAnimType.NonUniform))
            {
                for (int i = 0; i < this.NonUniformKeys.Count; i++)
                {
                    writer.Write(this.NonUniformKeys[i]);
                }
            }
        }
    }
}
