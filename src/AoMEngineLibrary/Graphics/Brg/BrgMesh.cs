using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace AoMEngineLibrary.Graphics.Brg
{
    public class BrgMesh
    {
        public BrgFile ParentFile { get; set; }
        public BrgMeshHeader Header { get; set; }

        public List<Vector3> Vertices { get; set; }
        public List<Vector3> Normals { get; set; }
        public List<BrgFace> Faces { get; set; }

        public List<Vector2> TextureCoordinates { get; set; }
        public List<Vector4> Colors { get; set; }
        public List<short> VertexMaterials { get; set; }

        public BrgMeshExtendedHeader ExtendedHeader { get; set; }
        public BrgUserDataEntry[] UserDataEntries { get; set; }
        private float[] particleData;

        public BrgDummyCollection Dummies { get; set; }

        public List<float> NonUniformKeys { get; set; }

        public BrgMesh(BrgFile file)
        {
            ParentFile = file;
            Header = new BrgMeshHeader
            {
                Version = 22,
                ExtendedHeaderSize = 40
            };

            Vertices = new List<Vector3>();
            Normals = new List<Vector3>();
            Faces = new List<BrgFace>();

            TextureCoordinates = new List<Vector2>();
            Colors = new List<Vector4>();
            VertexMaterials = new List<short>();

            ExtendedHeader = new BrgMeshExtendedHeader
            {
                MaterialLibraryTimestamp = 191738312,
                ExportedScaleFactor = 1f
            };

            UserDataEntries = new BrgUserDataEntry[0];
            particleData = new float[0];

            Dummies = new BrgDummyCollection();
            NonUniformKeys = new List<float>();
        }
        public BrgMesh(BrgBinaryReader reader, BrgFile file)
            : this(file)
        {
            ParentFile = file;
            Header = new BrgMeshHeader(reader);

            if (!Header.Flags.HasFlag(BrgMeshFlag.PARTICLEPOINTS))
            {
                Vertices = new List<Vector3>(Header.NumVertices);
                for (var i = 0; i < Header.NumVertices; i++)
                {
                    Vertices.Add(reader.ReadVector3D(Header.Version == 22));
                }

                Normals = new List<Vector3>(Header.NumVertices);
                for (var i = 0; i < Header.NumVertices; i++)
                {
                    if (Header.Version >= 13 && Header.Version <= 17)
                    {
                        reader.ReadInt16(); // TODO figure this out
                    }
                    else // v == 18, 19 or 22
                    {
                        Normals.Add(reader.ReadVector3D(Header.Version == 22));
                    }
                }

                if ((!Header.Flags.HasFlag(BrgMeshFlag.SECONDARYMESH) ||
                    Header.Flags.HasFlag(BrgMeshFlag.ANIMTEXCOORDS) ||
                    Header.Flags.HasFlag(BrgMeshFlag.PARTICLESYSTEM)) &&
                    Header.Flags.HasFlag(BrgMeshFlag.TEXCOORDSA))
                {
                    TextureCoordinates = new List<Vector2>(Header.NumVertices);
                    for (var i = 0; i < Header.NumVertices; i++)
                    {
                        TextureCoordinates.Add(reader.ReadVector2D(Header.Version == 22));
                    }
                }

                if (!Header.Flags.HasFlag(BrgMeshFlag.SECONDARYMESH) ||
                    Header.Flags.HasFlag(BrgMeshFlag.PARTICLESYSTEM))
                {
                    Faces = new List<BrgFace>(Header.NumFaces);
                    for (var i = 0; i < Header.NumFaces; ++i)
                    {
                        Faces.Add(new BrgFace());
                    }

                    if (Header.Flags.HasFlag(BrgMeshFlag.MATERIAL))
                    {
                        for (var i = 0; i < Header.NumFaces; i++)
                        {
                            Faces[i].MaterialIndex = reader.ReadInt16();
                        }
                    }

                    for (var i = 0; i < Header.NumFaces; i++)
                    {
                        Faces[i].A = reader.ReadUInt16();
                        Faces[i].B = reader.ReadUInt16();
                        Faces[i].C = reader.ReadUInt16();
                    }

                    if (Header.Flags.HasFlag(BrgMeshFlag.MATERIAL))
                    {
                        VertexMaterials = new List<short>(Header.NumVertices);
                        for (var i = 0; i < Header.NumVertices; i++)
                        {
                            VertexMaterials.Add(reader.ReadInt16());
                        }
                    }
                }
            }

            UserDataEntries = new BrgUserDataEntry[Header.UserDataEntryCount];
            if (!Header.Flags.HasFlag(BrgMeshFlag.PARTICLEPOINTS))
            {
                for (var i = 0; i < Header.UserDataEntryCount; i++)
                {
                    UserDataEntries[i] = reader.ReadUserDataEntry(false);
                }
            }

            ExtendedHeader = new BrgMeshExtendedHeader(reader, Header.ExtendedHeaderSize);
            
            if (Header.Flags.HasFlag(BrgMeshFlag.PARTICLEPOINTS))
            {
                particleData = new float[4 * Header.NumVertices];
                for (var i = 0; i < particleData.Length; i++)
                {
                    particleData[i] = reader.ReadSingle();
                }
                for (var i = 0; i < Header.UserDataEntryCount; i++)
                {
                    UserDataEntries[i] = reader.ReadUserDataEntry(true);
                }
            }

            if (Header.Version == 13)
            {
                reader.ReadBytes(ExtendedHeader.NameLength);
            }

            if (Header.Version >= 13 && Header.Version <= 18)
            {
                Header.Flags |= BrgMeshFlag.ATTACHPOINTS;
            }
            if (Header.Flags.HasFlag(BrgMeshFlag.ATTACHPOINTS))
            {
                Dummies.Read(reader, Header.Version, ExtendedHeader.NumDummies, ExtendedHeader.NumNameIndexes);
            }

            if (((Header.Flags.HasFlag(BrgMeshFlag.COLORALPHACHANNEL) ||
                Header.Flags.HasFlag(BrgMeshFlag.COLORCHANNEL)) &&
                !Header.Flags.HasFlag(BrgMeshFlag.SECONDARYMESH)) ||
                Header.Flags.HasFlag(BrgMeshFlag.ANIMVERTCOLORALPHA))
            {
                Colors = new List<Vector4>(Header.NumVertices);
                for (var i = 0; i < Header.NumVertices; i++)
                {
                    Colors.Add(reader.ReadTexel());
                }
            }

            // Only seen on first mesh
            if (!Header.Flags.HasFlag(BrgMeshFlag.SECONDARYMESH) &&
                Header.AnimationType.HasFlag(BrgMeshAnimType.NonUniform))
            {
                NonUniformKeys = new List<float>(ExtendedHeader.NumNonUniformKeys);
                for (var i = 0; i < ExtendedHeader.NumNonUniformKeys; i++)
                {
                    NonUniformKeys.Add(reader.ReadSingle());
                }
            }

            if (Header.Version >= 14 && Header.Version <= 19)
            {
                // Face Normals??
                var legacy = new Vector3[Header.NumFaces];
                for (var i = 0; i < Header.NumFaces; i++)
                {
                    legacy[i] = reader.ReadVector3D(false);
                }
            }

            if (!Header.Flags.HasFlag(BrgMeshFlag.SECONDARYMESH) && Header.Version != 22)
            {
                reader.ReadBytes(ExtendedHeader.ShadowNameLength0 + ExtendedHeader.ShadowNameLength1);
            }
        }

        public void Write(BrgBinaryWriter writer)
        {
            if (Vertices.Count > ushort.MaxValue) throw new InvalidDataException($"Brg meshes cannot have more than {ushort.MaxValue} vertices.");
            if (Faces.Count > ushort.MaxValue) throw new InvalidDataException($"Brg meshes cannot have more than {ushort.MaxValue} faces.");

            Header.Version = 22;
            if (Header.Flags.HasFlag(BrgMeshFlag.PARTICLEPOINTS))
            {
                Header.NumVertices = (ushort)(particleData.Length / 4);
            }
            else
            {
                Header.NumVertices = (ushort)Vertices.Count;
            }
            Header.NumFaces = (ushort)Faces.Count;
            Header.UserDataEntryCount = (ushort)UserDataEntries.Length;
            Header.CenterRadius = Math.Max(Math.Max(Header.MaximumExtent.X, Header.MaximumExtent.Y), Header.MaximumExtent.Z);
            Header.ExtendedHeaderSize = 40;
            Header.Write(writer);

            if (!Header.Flags.HasFlag(BrgMeshFlag.PARTICLEPOINTS))
            {
                for (var i = 0; i < Vertices.Count; i++)
                {
                    writer.WriteVector3D(Vertices[i], true);
                }
                for (var i = 0; i < Vertices.Count; i++)
                {
                    writer.WriteVector3D(Normals[i], true);
                }

                if ((!Header.Flags.HasFlag(BrgMeshFlag.SECONDARYMESH) ||
                    Header.Flags.HasFlag(BrgMeshFlag.ANIMTEXCOORDS) ||
                    Header.Flags.HasFlag(BrgMeshFlag.PARTICLESYSTEM)) &&
                    Header.Flags.HasFlag(BrgMeshFlag.TEXCOORDSA))
                {
                    for (var i = 0; i < TextureCoordinates.Count; i++)
                    {
                        writer.WriteHalf(TextureCoordinates[i].X);
                        writer.WriteHalf(TextureCoordinates[i].Y);
                    }
                }

                if (!Header.Flags.HasFlag(BrgMeshFlag.SECONDARYMESH) ||
                    Header.Flags.HasFlag(BrgMeshFlag.PARTICLESYSTEM))
                {
                    if (Header.Flags.HasFlag(BrgMeshFlag.MATERIAL))
                    {
                        for (var i = 0; i < Faces.Count; i++)
                        {
                            writer.Write(Faces[i].MaterialIndex);
                        }
                    }

                    for (var i = 0; i < Faces.Count; i++)
                    {
                        writer.Write(Faces[i].A);
                        writer.Write(Faces[i].B);
                        writer.Write(Faces[i].C);
                    }

                    if (Header.Flags.HasFlag(BrgMeshFlag.MATERIAL))
                    {
                        for (var i = 0; i < VertexMaterials.Count; i++)
                        {
                            writer.Write(VertexMaterials[i]);
                        }
                    }
                }
            }

            if (!Header.Flags.HasFlag(BrgMeshFlag.PARTICLEPOINTS))
            {
                for (var i = 0; i < UserDataEntries.Length; i++)
                {
                    writer.WriteUserDataEntry(ref UserDataEntries[i], false);
                }
            }

            ExtendedHeader.NumNonUniformKeys = NonUniformKeys.Count;
            ExtendedHeader.Write(writer);

            if (Header.Flags.HasFlag(BrgMeshFlag.PARTICLEPOINTS))
            {
                for (var i = 0; i < particleData.Length; i++)
                {
                    writer.Write(particleData[i]);
                }
                for (var i = 0; i < UserDataEntries.Length; i++)
                {
                    writer.WriteUserDataEntry(ref UserDataEntries[i], true);
                }
            }

            if (Header.Flags.HasFlag(BrgMeshFlag.ATTACHPOINTS))
            {
                Dummies.Write(writer, Header.Version);
            }

            if (((Header.Flags.HasFlag(BrgMeshFlag.COLORALPHACHANNEL) ||
                Header.Flags.HasFlag(BrgMeshFlag.COLORCHANNEL)) &&
                !Header.Flags.HasFlag(BrgMeshFlag.SECONDARYMESH)) ||
                Header.Flags.HasFlag(BrgMeshFlag.ANIMVERTCOLORALPHA))
            {
                for (var i = 0; i < Colors.Count; i++)
                {
                    writer.WriteTexel(Colors[i]);
                }
            }

            if (!Header.Flags.HasFlag(BrgMeshFlag.SECONDARYMESH) &&
                Header.AnimationType.HasFlag(BrgMeshAnimType.NonUniform))
            {
                for (var i = 0; i < NonUniformKeys.Count; i++)
                {
                    writer.Write(NonUniformKeys[i]);
                }
            }
        }
    }
}
