namespace AoMEngineLibrary.Graphics.Brg
{
    using AoMEngineLibrary.Graphics.Model;
    using Extensions;
    using Newtonsoft.Json;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Numerics;

    public class BrgMesh : Mesh
    {
        public BrgFile ParentFile { get; set; }
        public BrgMeshHeader Header { get; set; }

        public List<Int16> VertexMaterials { get; set; }

        public BrgMeshExtendedHeader ExtendedHeader { get; set; }
        public BrgUserDataEntry[] UserDataEntries { get; set; }
        private float[] particleData;

        public List<BrgAttachpoint> Attachpoints { get; set; }
        public List<float> NonUniformKeys { get; set; }

        public BrgMesh(BrgFile file)
            : base()
        {
            this.ParentFile = file;
            this.Header = new BrgMeshHeader();
            this.Header.Version = 22;
            this.Header.ExtendedHeaderSize = 40;

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
                    this.Vertices.Add(reader.ReadVector3D(true, this.Header.Version == 22));
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
                        this.Normals.Add(reader.ReadVector3D(true, this.Header.Version == 22));
                    }
                }

                if ((!this.Header.Flags.HasFlag(BrgMeshFlag.SECONDARYMESH) ||
                    this.Header.Flags.HasFlag(BrgMeshFlag.ANIMTEXCOORDS) ||
                    this.Header.Flags.HasFlag(BrgMeshFlag.PARTICLESYSTEM)) &&
                    this.Header.Flags.HasFlag(BrgMeshFlag.TEXCOORDSA))
                {
                    this.TextureCoordinates = new List<Vector3>(this.Header.NumVertices);
                    for (int i = 0; i < this.Header.NumVertices; i++)
                    {
                        this.TextureCoordinates.Add(new Vector3(reader.ReadVector2D(this.Header.Version == 22), 0f));
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
                this.Colors = new List<Color4D>(this.Header.NumVertices);
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
                    this.NonUniformKeys[i] = reader.ReadSingle();
                }
            }

            if (this.Header.Version >= 14 && this.Header.Version <= 19)
            {
                // Face Normals??
                Vector3[] legacy = new Vector3[this.Header.NumFaces];
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

        public void ReadJson(JsonReader reader)
        {
            int count = 0;

            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.EndObject)
                {
                    break;
                }

                if (reader.TokenType == JsonToken.PropertyName)
                {
                    switch ((string)reader.Value)
                    {
                        case nameof(Header):
                            Header.ReadJson(reader);
                            break;
                        case nameof(ExtendedHeader):
                            ExtendedHeader.ReadJson(reader);
                            break;
                        case nameof(UserDataEntries):
                            while (reader.Read())
                            {
                                if (reader.TokenType == JsonToken.StartArray)
                                {
                                    continue;
                                }
                                if (reader.TokenType == JsonToken.EndArray)
                                {
                                    break;
                                }

                                // TODO
                            }
                            break;
                        case nameof(particleData):
                            while (reader.Read())
                            {
                                if (reader.TokenType == JsonToken.StartArray)
                                {
                                    continue;
                                }
                                if (reader.TokenType == JsonToken.EndArray)
                                {
                                    break;
                                }

                                // TODO
                            }
                            break;
                        case nameof(NonUniformKeys):
                            while (reader.Read())
                            {
                                if (reader.TokenType == JsonToken.StartArray)
                                {
                                    continue;
                                }
                                if (reader.TokenType == JsonToken.EndArray)
                                {
                                    break;
                                }

                                NonUniformKeys.Add((float)(double)reader.Value);
                            }
                            break;
                        case nameof(Attachpoints):
                            while (reader.Read())
                            {
                                if (reader.TokenType == JsonToken.StartArray)
                                {
                                    continue;
                                }
                                if (reader.TokenType == JsonToken.EndArray)
                                {
                                    break;
                                }

                                BrgAttachpoint a = new BrgAttachpoint();
                                a.ReadJson(reader);
                                Attachpoints.Add(a);
                            }
                            break;
                        case nameof(Vertices):
                            reader.Read(); // StartArray
                            while (reader.Read())
                            {
                                if (reader.TokenType == JsonToken.EndArray)
                                {
                                    break;
                                }

                                Vector3 v = reader.ReadAsVector3();
                                Vertices.Add(v);
                            }
                            break;
                        case nameof(Normals):
                            reader.Read(); // StartArray
                            while (reader.Read())
                            {
                                if (reader.TokenType == JsonToken.EndArray)
                                {
                                    break;
                                }

                                Vector3 v = reader.ReadAsVector3();
                                Normals.Add(v);
                            }
                            break;
                        case nameof(TextureCoordinates):
                            reader.Read(); // StartArray
                            while (reader.Read())
                            {
                                if (reader.TokenType == JsonToken.EndArray)
                                {
                                    break;
                                }

                                Vector3 v = reader.ReadAsVector3();
                                TextureCoordinates.Add(v);
                            }
                            break;
                        case nameof(Colors):
                            reader.Read(); // StartArray
                            while (reader.Read())
                            {
                                if (reader.TokenType == JsonToken.EndArray)
                                {
                                    break;
                                }

                                Color4D c = new Color4D();
                                c.ReadJson(reader);
                                Colors.Add(c);
                            }
                            break;
                        case nameof(Faces):
                            count = 0;
                            reader.Read(); // StartArray
                            while (reader.Read())
                            {
                                if (reader.TokenType == JsonToken.EndArray)
                                {
                                    break;
                                }

                                Face f;
                                if (Faces.Count <= count)
                                {
                                    f = new Face();
                                    Faces.Add(f);
                                }
                                else
                                {
                                    f = Faces[count];
                                }
                                ++count;

                                while (reader.Read())
                                {
                                    if (reader.TokenType == JsonToken.StartArray)
                                    {
                                        continue;
                                    }
                                    if (reader.TokenType == JsonToken.EndArray)
                                    {
                                        break;
                                    }

                                    f.Indices.Add((Int16)(Int64)reader.Value);
                                }
                            }
                            break;
                        case "FaceMaterials":
                            count = 0;
                            reader.Read(); // StartArray
                            while (reader.Read())
                            {
                                if (reader.TokenType == JsonToken.EndArray)
                                {
                                    break;
                                }

                                Face f;
                                if (Faces.Count <= count)
                                {
                                    f = new Face();
                                    Faces.Add(f);
                                }
                                else
                                {
                                    f = Faces[count];
                                }
                                ++count;

                                f.MaterialIndex = (Int16)(Int64)reader.Value;
                            }
                            break;
                        case nameof(MeshAnimations):
                            reader.Read(); // StartArray
                            while (reader.Read())
                            {
                                if (reader.TokenType == JsonToken.EndArray)
                                {
                                    break;
                                }

                                BrgMesh m = new BrgMesh(this.ParentFile);
                                m.ReadJson(reader);
                                MeshAnimations.Add(m);
                            }
                            break;
                        default:
                            throw new Exception("Unexpected property name!");
                    }
                }
                else if (reader.TokenType != JsonToken.StartObject)
                {
                    throw new Exception("Unexpected token type! " + reader.TokenType);
                }
            }
        }

        public void WriteJson(JsonWriter writer)
        {
            writer.WriteStartObject();

            writer.WritePropertyName(nameof(Header));
            Header.WriteJson(writer);

            writer.WritePropertyName(nameof(ExtendedHeader));
            ExtendedHeader.WriteJson(writer);

            writer.WritePropertyName(nameof(UserDataEntries));
            writer.WriteStartArray();
            for (int i = 0; i < UserDataEntries.Length; ++i)
            {
                UserDataEntries[i].WriteJson(writer);
            }
            writer.WriteEndArray();

            writer.WritePropertyName(nameof(particleData));
            writer.WriteStartArray();
            for (int i = 0; i < particleData.Length; ++i)
            {
                writer.WriteRawValue(particleData[i].ToRoundTripString());
            }
            writer.WriteEndArray();

            writer.WritePropertyName(nameof(NonUniformKeys));
            writer.WriteStartArray();
            for (int i = 0; i < NonUniformKeys.Count; ++i)
            {
                writer.WriteRawValue(NonUniformKeys[i].ToRoundTripString());
            }
            writer.WriteEndArray();

            writer.WritePropertyName(nameof(Attachpoints));
            writer.WriteStartArray();
            foreach (BrgAttachpoint attachpoint in Attachpoints)
            {
                attachpoint.WriteJson(writer);
            }
            writer.WriteEndArray();

            writer.WritePropertyName(nameof(Vertices));
            writer.WriteStartArray();
            for (int i = 0; i < Vertices.Count; ++i)
            {
                Vertices[i].WriteJson(writer);
            }
            writer.WriteEndArray();

            writer.WritePropertyName(nameof(Normals));
            writer.WriteStartArray();
            for (int i = 0; i < Normals.Count; ++i)
            {
                Normals[i].WriteJson(writer);
            }
            writer.WriteEndArray();

            writer.WritePropertyName(nameof(TextureCoordinates));
            writer.WriteStartArray();
            for (int i = 0; i < TextureCoordinates.Count; ++i)
            {
                TextureCoordinates[i].WriteJson(writer);
            }
            writer.WriteEndArray();

            writer.WritePropertyName(nameof(Colors));
            writer.WriteStartArray();
            for (int i = 0; i < Colors.Count; ++i)
            {
                Colors[i].WriteJson(writer);
            }
            writer.WriteEndArray();

            writer.WritePropertyName(nameof(Faces));
            writer.WriteStartArray();
            for (int i = 0; i < Faces.Count; ++i)
            {
                writer.WriteStartArray();
                writer.WriteValue(Faces[i].Indices[0]);
                writer.WriteValue(Faces[i].Indices[1]);
                writer.WriteValue(Faces[i].Indices[2]);
                writer.WriteEndArray();
            }
            writer.WriteEndArray();

            writer.WritePropertyName("FaceMaterials");
            writer.WriteStartArray();
            for (int i = 0; i < Faces.Count; ++i)
            {
                writer.WriteValue(Faces[i].MaterialIndex);
            }
            writer.WriteEndArray();

            writer.WritePropertyName(nameof(MeshAnimations));
            writer.WriteStartArray();
            for (int i = 0; i < MeshAnimations.Count; ++i)
            {
                ((BrgMesh)MeshAnimations[i]).WriteJson(writer);
            }
            writer.WriteEndArray();

            writer.WriteEndObject();
        }
    }
}
