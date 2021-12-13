using AoMEngineLibrary.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace AoMEngineLibrary.Graphics.Brg
{
    public class BrgMesh
    {
        public BrgMeshHeader Header { get; set; }

        public List<Vector3> Vertices { get; set; }
        public List<Vector3> Normals { get; set; }
        public List<BrgFace> Faces { get; set; }
        public List<float> ParticleRadius { get; }

        public List<Vector2> TextureCoordinates { get; set; }
        public List<Vector4> Colors { get; set; }
        public List<short> VertexMaterials { get; set; }

        public BrgMeshExtendedHeader ExtendedHeader { get; set; }

        public BrgDummyCollection Dummies { get; set; }

        public List<float> NonUniformKeys { get; set; }

        public BrgMesh()
        {
            Header = new BrgMeshHeader
            {
                Version = 22,
                ExtendedHeaderSize = 40
            };

            Vertices = new List<Vector3>();
            Normals = new List<Vector3>();
            Faces = new List<BrgFace>();
            ParticleRadius = new List<float>();

            TextureCoordinates = new List<Vector2>();
            Colors = new List<Vector4>();
            VertexMaterials = new List<short>();

            ExtendedHeader = new BrgMeshExtendedHeader
            {
                MaterialLibraryTimestamp = 191738312,
                ExportedScaleFactor = 1f
            };

            Dummies = new BrgDummyCollection();
            NonUniformKeys = new List<float>();
        }
        public BrgMesh(BrgBinaryReader reader)
            : this(reader, null)
        {
        }
        internal BrgMesh(BrgBinaryReader reader, BrgMesh? baseMesh = null)
            : this()
        {
            Header = new BrgMeshHeader(reader);

            if (Header.Flags.HasFlag(BrgMeshFlag.ParticlePoints))
            {
                ReadParticlePoints(reader);
                return;
            }

            Vertices = new List<Vector3>(Header.NumVertices);
            for (var i = 0; i < Header.NumVertices; i++)
            {
                Vertices.Add(reader.ReadVector3D(Header.Version == 22));
            }

            if (Header.Version >= 13 && Header.Version <= 17)
            {
                // Couldn't figure out compression, skip
                Normals.Clear();
                if (reader.BaseStream.CanSeek)
                    _ = reader.BaseStream.Seek(2 * Header.NumVertices, SeekOrigin.Current);
                else
                    _ = reader.ReadBytes(2 * Header.NumVertices);

                //for (var i = 0; i < Header.NumVertices; i++)
                //{
                //    // Compressed normal, let's generate full vec3
                //    var r = reader.ReadByte();
                //    var g = reader.ReadByte();

                //    // Convert from [0..1] space to [-1..1] space and calc Z
                //    var x = (r / 255.0f) * 2.0f - 1.0f;
                //    var y = (g / 255.0f) * 2.0f - 1.0f;
                //    var z = MathF.Sqrt(Math.Clamp(1.0f - (x * x + y * y), 0, 1));
                //    var normal = Vector3.Normalize(new Vector3(x, y, z));
                //    Normals.Add(normal);
                //}
            }
            else // v == 18, 19 or 22
            {
                Normals = new List<Vector3>(Header.NumVertices);
                for (var i = 0; i < Header.NumVertices; i++)
                {
                    Normals.Add(reader.ReadVector3D(Header.Version == 22));
                }
            }

            if ((!Header.Flags.HasFlag(BrgMeshFlag.Secondary) ||
                Header.Flags.HasFlag(BrgMeshFlag.AnimTxCoords) ||
                Header.Flags.HasFlag(BrgMeshFlag.ParticleSystem)) &&
                Header.Flags.HasFlag(BrgMeshFlag.Texture1))
            {
                TextureCoordinates = new List<Vector2>(Header.NumVertices);
                for (var i = 0; i < Header.NumVertices; i++)
                {
                    TextureCoordinates.Add(reader.ReadVector2D(Header.Version == 22));
                }
            }

            if ((!Header.Flags.HasFlag(BrgMeshFlag.Secondary) &&
                Header.Flags.HasFlag(BrgMeshFlag.Material)) ||
                Header.Flags.HasFlag(BrgMeshFlag.ParticleSystem))
            {
                Faces = new List<BrgFace>(Header.NumFaces);
                for (var i = 0; i < Header.NumFaces; ++i)
                {
                    var face = new BrgFace()
                    {
                        MaterialIndex = reader.ReadInt16()
                    };
                    Faces.Add(face);
                }

                for (var i = 0; i < Header.NumFaces; ++i)
                {
                    Faces[i].A = reader.ReadUInt16();
                    Faces[i].B = reader.ReadUInt16();
                    Faces[i].C = reader.ReadUInt16();
                }

                VertexMaterials = new List<short>(Header.NumVertices);
                for (var i = 0; i < Header.NumVertices; ++i)
                {
                    VertexMaterials.Add(reader.ReadInt16());
                }
            }

            // User data entries are useless and released AoM can't read them. Skip
            for (var i = 0; i < Header.UserDataEntryCount; i++)
            {
                _ = reader.ReadUserDataEntry();
            }

            ExtendedHeader = new BrgMeshExtendedHeader(reader, Header.ExtendedHeaderSize);

            if (Header.Version == 13)
            {
                // object name
                reader.ReadBytes(ExtendedHeader.NameLength);
            }

            if (Header.Version >= 13 && Header.Version <= 18 && ExtendedHeader.NumDummies > 0)
            {
                Header.Flags |= BrgMeshFlag.DummyObjects;
            }
            if (Header.Flags.HasFlag(BrgMeshFlag.DummyObjects))
            {
                Dummies.Read(reader, Header.Version, ExtendedHeader.NumDummies, ExtendedHeader.NumNameIndexes);
            }

            if (((Header.Flags.HasFlag(BrgMeshFlag.AlphaChannel) ||
                Header.Flags.HasFlag(BrgMeshFlag.ColorChannel)) &&
                !Header.Flags.HasFlag(BrgMeshFlag.Secondary)) ||
                Header.Flags.HasFlag(BrgMeshFlag.AnimVertexColor))
            {
                Colors = new List<Vector4>(Header.NumVertices);
                for (var i = 0; i < Header.NumVertices; i++)
                {
                    Colors.Add(reader.ReadTexel());
                }
            }

            if (Header.Version >= 14 && Header.Version <= 19)
            {
                // Face Normals - Vec3 per face
                if (reader.BaseStream.CanSeek)
                    _ = reader.BaseStream.Seek(Header.NumFaces * 12, SeekOrigin.Current);
                else
                    _ = reader.ReadBytes(Header.NumFaces * 12);
            }

            if (!Header.Flags.HasFlag(BrgMeshFlag.Secondary) &&
                Header.AnimationType == BrgMeshAnimType.NonUniform)
            {
                NonUniformKeys = new List<float>(ExtendedHeader.NumNonUniformKeys);
                for (var i = 0; i < ExtendedHeader.NumNonUniformKeys; i++)
                {
                    NonUniformKeys.Add(reader.ReadSingle());
                }
            }

            if (!Header.Flags.HasFlag(BrgMeshFlag.Secondary) && Header.Version < 22)
            {
                reader.ReadBytes(ExtendedHeader.ShadowNameLength0 + ExtendedHeader.ShadowNameLength1);
            }

            // Generate normals as final step
            if (Normals.Count <= 0)
                GenerateSmoothNormals(baseMesh);
        }

        private void ReadParticlePoints(BrgBinaryReader reader)
        {
            ExtendedHeader = new BrgMeshExtendedHeader(reader, Header.ExtendedHeaderSize);

            Vertices = new List<Vector3>(Header.NumVertices);
            for (var i = 0; i < Header.NumVertices; ++i)
            {
                Vertices.Add(reader.ReadVector3());
            }

            // radius
            ParticleRadius.Capacity = Header.NumVertices;
            for (var i = 0; i < Header.NumVertices; ++i)
            {
                ParticleRadius.Add(reader.ReadSingle());
            }

            // User data entries are useless and released AoM can't read them. Skip
            for (var i = 0; i < Header.UserDataEntryCount; i++)
            {
                _ = reader.ReadUserDataEntry();
            }

            // if ever needed object name would go here potentially? (versions <14)

            if (ExtendedHeader.NumDummies > 0)
            {
                Header.Flags |= BrgMeshFlag.DummyObjects;
            }
            if (Header.Flags.HasFlag(BrgMeshFlag.DummyObjects))
            {
                Dummies.Read(reader, Header.Version, ExtendedHeader.NumDummies, ExtendedHeader.NumNameIndexes);
            }
        }

        public void Write(BrgBinaryWriter writer)
        {
            if (Vertices.Count > ushort.MaxValue) throw new InvalidDataException($"Brg meshes cannot have more than {ushort.MaxValue} vertices.");
            if (Faces.Count > ushort.MaxValue) throw new InvalidDataException($"Brg meshes cannot have more than {ushort.MaxValue} faces.");

            Header.Version = 22;
            Header.NumVertices = (ushort)Vertices.Count;
            Header.NumFaces = (ushort)Faces.Count;
            Header.UserDataEntryCount = 0;
            Header.CenterRadius = Math.Max(Math.Max(Header.MaximumExtent.X, Header.MaximumExtent.Y), Header.MaximumExtent.Z);
            Header.ExtendedHeaderSize = 40;
            Header.Write(writer);

            for (var i = 0; i < Vertices.Count; i++)
            {
                writer.WriteVector3D(Vertices[i], true);
            }
            for (var i = 0; i < Vertices.Count; i++)
            {
                writer.WriteVector3D(Normals[i], true);
            }

            if ((!Header.Flags.HasFlag(BrgMeshFlag.Secondary) ||
                Header.Flags.HasFlag(BrgMeshFlag.AnimTxCoords) ||
                Header.Flags.HasFlag(BrgMeshFlag.ParticleSystem)) &&
                Header.Flags.HasFlag(BrgMeshFlag.Texture1))
            {
                for (var i = 0; i < TextureCoordinates.Count; i++)
                {
                    writer.WriteHalf(TextureCoordinates[i].X);
                    writer.WriteHalf(TextureCoordinates[i].Y);
                }
            }

            if ((!Header.Flags.HasFlag(BrgMeshFlag.Secondary) &&
                Header.Flags.HasFlag(BrgMeshFlag.Material)) ||
                Header.Flags.HasFlag(BrgMeshFlag.ParticleSystem))
            {
                for (var i = 0; i < Faces.Count; i++)
                {
                    writer.Write(Faces[i].MaterialIndex);
                }

                for (var i = 0; i < Faces.Count; i++)
                {
                    writer.Write(Faces[i].A);
                    writer.Write(Faces[i].B);
                    writer.Write(Faces[i].C);
                }

                for (var i = 0; i < VertexMaterials.Count; i++)
                {
                    writer.Write(VertexMaterials[i]);
                }
            }

            ExtendedHeader.NumNonUniformKeys = NonUniformKeys.Count;
            ExtendedHeader.Write(writer);

            if (Header.Flags.HasFlag(BrgMeshFlag.DummyObjects))
            {
                Dummies.Write(writer, Header.Version);
            }

            if (((Header.Flags.HasFlag(BrgMeshFlag.AlphaChannel) ||
                Header.Flags.HasFlag(BrgMeshFlag.ColorChannel)) &&
                !Header.Flags.HasFlag(BrgMeshFlag.Secondary)) ||
                Header.Flags.HasFlag(BrgMeshFlag.AnimVertexColor))
            {
                for (var i = 0; i < Colors.Count; ++i)
                {
                    writer.WriteTexel(Colors[i]);
                }
            }

            if (!Header.Flags.HasFlag(BrgMeshFlag.Secondary) &&
                Header.AnimationType == BrgMeshAnimType.NonUniform)
            {
                for (var i = 0; i < NonUniformKeys.Count; ++i)
                {
                    writer.Write(NonUniformKeys[i]);
                }
            }
        }

        private void GenerateSmoothNormals(BrgMesh? baseMesh)
        {
            var (faces, faceNormals) = ComputeSurfaceNormals(baseMesh);
            if (faces is null || faceNormals.Length <= 0)
                return;

            var norms = new Vector3[Vertices.Count];
            for (var i = 0; i < faces.Count; ++i)
            {
                var face = faces[i];

                norms[face.A] += faceNormals[i];
                norms[face.B] += faceNormals[i];
                norms[face.C] += faceNormals[i];
            }
            for (var i = 0; i < norms.Length; ++i)
            {
                var nrm = Vector3.Normalize(norms[i]);
                norms[i] = nrm.IsFinite() && nrm.LengthSquared() > 0.5f ? nrm : Vector3.UnitZ;
            }
            Normals.Clear();
            Normals.AddRange(norms);
        }
        private (IReadOnlyList<BrgFace>? Faces, Vector3[] FaceNormals) ComputeSurfaceNormals(BrgMesh? baseMesh)
        {
            var faces = Faces;

            if (baseMesh is not null && !Header.Flags.HasFlag(BrgMeshFlag.ParticleSystem))
                faces = baseMesh.Faces;

            if (faces is null || faces.Count == 0)
            {
                return (faces, Array.Empty<Vector3>());
            }

            // Go through each face and compute the normal.
            var faceNormals = new Vector3[faces.Count];
            for (var i = 0; i < faces.Count; ++i)
            {
                var face = faces[i];
                var index0 = face.A;
                var index1 = face.B;
                var index2 = face.C;

                // Vector from vertex 0 to 1
                var vec0 = Vertices[index1] - Vertices[index0];

                // Vector from vertex 1 to 2
                var vec1 = Vertices[index2] - Vertices[index1];

                // Get surface normal by taking the cross product.
                var normal = Vector3.Cross(vec0, vec1);
                var length = normal.Length();
                if (length != 0)
                {
                    normal /= length;
                }
                faceNormals[i] = normal;
            }

            return (faces, faceNormals);
        }
    }
}
