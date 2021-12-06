using AoMEngineLibrary.Graphics.Brg;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using Xunit;
using Xunit.Abstractions;

namespace AoMEngineLibrary.Tests.Graphics.Brg;

public class BrgMeshTests
{
    public record MeshTest(BrgMesh Mesh, byte[] Data);

    public static readonly IEnumerable<object[]> Tests = new List<object[]>
        {
            new[] {
                new MeshTest(new BrgMesh()
                    {
                        Header = new BrgMeshHeader()
                        {
                            Version = 22,
                            Format = BrgMeshFormat.AnimationLength | BrgMeshFormat.HasFaceNormals,
                            NumVertices = 3,
                            NumFaces = 1,
                            InterpolationType = BrgMeshInterpolationType.Default,
                            AnimationType = BrgMeshAnimType.NonUniform,
                            UserDataEntryCount = 0,
                            CenterPosition = new Vector3(0.3f, 0.4f, 0.5f),
                            CenterRadius = 0.4f,
                            MassPosition = new Vector3(0.1f, 0.2f, 0.3f),
                            HotspotPosition = new Vector3(0.6f, 0.7f, 0.8f),
                            ExtendedHeaderSize = 40,
                            Flags = BrgMeshFlag.TEXCOORDSA | BrgMeshFlag.ATTACHPOINTS | BrgMeshFlag.MATERIAL | BrgMeshFlag.COLORCHANNEL,
                            MinimumExtent = new Vector3(-0.4f, -0.3f, -0.1f),
                            MaximumExtent = new Vector3(0.4f, 0.3f, 0.1f)
                        },
                        Vertices = new List<Vector3>()
                        {
                            new Vector3(1, 2, 3),
                            new Vector3(4, 5, 6),
                            new Vector3(7, 8, 9)
                        },
                        Normals = new List<Vector3>()
                        {
                            new Vector3(0, 1, 0),
                            new Vector3(1, 0, 0),
                            new Vector3(0, 0, 1)
                        },
                        Faces = new List<BrgFace>()
                        {
                            new BrgFace(0, 1, 2) { MaterialIndex = 4 }
                        },
                        TextureCoordinates = new List<Vector2>()
                        {
                            new Vector2(8, 2),
                            new Vector2(1, 7),
                            new Vector2(3, 4)
                        },
                        Colors = new List<Vector4>()
                        {
                            new Vector4(0.4f, 0.4f, 0.6f, 0.6f),
                            new Vector4(0.2f, 0.8f, 0.4f, 1f),
                            new Vector4(0.0f, 0.6f, 0.8f, 0.2f)
                        },
                        VertexMaterials = new List<short>()
                        {
                            4, 4, 4
                        },
                        ExtendedHeader = new BrgMeshExtendedHeader()
                        {
                            NumNameIndexes = 28,
                            NumDummies = 2,
                            NameLength = 0,
                            PointMaterial = 0,
                            PointRadius = 0,
                            NumMaterials = 1,
                            ShadowNameLength0 = 0,
                            ShadowNameLength1 = 0,
                            ShadowNameLength2 = 0,
                            AnimationLength = 0.92f,
                            MaterialLibraryTimestamp = 191738312,
                            Reserved = 8,
                            ExportedScaleFactor = 1.1f,
                            NumNonUniformKeys = 4,
                            NumUniqueMaterials = 1
                        },
                        UserDataEntries = new BrgUserDataEntry[0],
                        Dummies = new BrgDummyCollection()
                        {
                            new BrgDummy() { Type = BrgDummyType.Chin, Up = new Vector3(2), Forward = new Vector3(4), Right = new Vector3(6), Position = new Vector3(8)},
                            new BrgDummy() { Type = BrgDummyType.HitPointBar, Up = new Vector3(1), Forward = new Vector3(2), Right = new Vector3(3), Position = new Vector3(4) }
                        },
                        NonUniformKeys = new List<float>()
                        {
                            0.1f, 0.4f, 0.6f, 1.0f
                        }
                    },
                    new byte[]
                    {
                        0x16,0x00,0x0C,0x00,0x03,0x00,0x01,0x00,0x00,0x01,0x00,0x00,0x9A,0x99,0x99,0x3E,
                        0xCD,0xCC,0xCC,0x3E,0x00,0x00,0x00,0x3F,0xCD,0xCC,0xCC,0x3E,0xCD,0xCC,0xCC,0x3D,
                        0xCD,0xCC,0x4C,0x3E,0x9A,0x99,0x99,0x3E,0x9A,0x99,0x19,0x3F,0x33,0x33,0x33,0x3F,
                        0xCD,0xCC,0x4C,0x3F,0x28,0x00,0x62,0x01,0xCD,0xCC,0xCC,0xBE,0x9A,0x99,0x99,0xBE,
                        0xCD,0xCC,0xCC,0xBD,0xCD,0xCC,0xCC,0x3E,0x9A,0x99,0x99,0x3E,0xCD,0xCC,0xCC,0x3D,
                        0x80,0x3F,0x00,0x40,0x40,0x40,0x80,0x40,0xA0,0x40,0xC0,0x40,0xE0,0x40,0x00,0x41,
                        0x10,0x41,0x00,0x00,0x80,0x3F,0x00,0x00,0x80,0x3F,0x00,0x00,0x00,0x00,0x00,0x00,
                        0x00,0x00,0x80,0x3F,0x00,0x41,0x00,0x40,0x80,0x3F,0xE0,0x40,0x40,0x40,0x80,0x40,
                        0x04,0x00,0x00,0x00,0x01,0x00,0x02,0x00,0x04,0x00,0x04,0x00,0x04,0x00,0x1C,0x00,
                        0x02,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x01,0x00,0x00,0x00,0x1F,0x85,
                        0x6B,0x3F,0xC8,0xB1,0x6D,0x0B,0x00,0x00,0x00,0x41,0xCD,0xCC,0x8C,0x3F,0x04,0x00,
                        0x00,0x00,0x01,0x00,0x00,0x00,0x02,0x00,0x1C,0x00,0x01,0x00,0x00,0x40,0x00,0x40,
                        0x00,0x40,0x80,0x3F,0x80,0x3F,0x80,0x3F,0x80,0x40,0x80,0x40,0x80,0x40,0x00,0x40,
                        0x00,0x40,0x00,0x40,0xC0,0x40,0xC0,0x40,0xC0,0x40,0x40,0x40,0x40,0x40,0x40,0x40,
                        0x00,0x41,0x00,0x41,0x00,0x41,0x80,0x40,0x80,0x40,0x80,0x40,0x80,0xBE,0x80,0xBE,
                        0x80,0xBE,0x80,0xBE,0x80,0xBE,0x80,0xBE,0x80,0x3E,0x80,0x3E,0x80,0x3E,0x80,0x3E,
                        0x80,0x3E,0x80,0x3E,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
                        0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
                        0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
                        0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
                        0x00,0x00,0x00,0x00,0x01,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
                        0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
                        0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
                        0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
                        0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
                        0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
                        0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
                        0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
                        0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
                        0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x01,0x00,0x00,0x00,
                        0x00,0x00,0x00,0x00,0x00,0x01,0x99,0x66,0x66,0x99,0x66,0xCC,0x33,0xFF,0xCC,0x99,
                        0x00,0x33,0xCD,0xCC,0xCC,0x3D,0xCD,0xCC,0xCC,0x3E,0x9A,0x99,0x19,0x3F,0x00,0x00,
                        0x80,0x3F,
                    }
                )
            }
        };

    private readonly ITestOutputHelper _output;

    public BrgMeshTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Theory]
    [MemberData(nameof(Tests))]
    public void Read_Works(MeshTest test)
    {
        using (var ms = new MemoryStream())
        using (var reader = new BrgBinaryReader(ms))
        {
            ms.Write(test.Data, 0, test.Data.Length);
            ms.Flush();
            ms.Seek(0, SeekOrigin.Begin);

            var result = new BrgMesh(reader);

            Assert.Equal(test.Mesh.Header.Version, result.Header.Version);
            Assert.Equal(test.Mesh.Header.Format, result.Header.Format);
            Assert.Equal(test.Mesh.Header.NumVertices, result.Header.NumVertices);
            Assert.Equal(test.Mesh.Header.NumFaces, result.Header.NumFaces);
            Assert.Equal(test.Mesh.Header.InterpolationType, result.Header.InterpolationType);
            Assert.Equal(test.Mesh.Header.AnimationType, result.Header.AnimationType);
            Assert.Equal(test.Mesh.Header.UserDataEntryCount, result.Header.UserDataEntryCount);
            Assert.Equal(test.Mesh.Header.CenterPosition, result.Header.CenterPosition);
            Assert.Equal(test.Mesh.Header.CenterRadius, result.Header.CenterRadius);
            Assert.Equal(test.Mesh.Header.MassPosition, result.Header.MassPosition);
            Assert.Equal(test.Mesh.Header.HotspotPosition, result.Header.HotspotPosition);
            Assert.Equal(test.Mesh.Header.ExtendedHeaderSize, result.Header.ExtendedHeaderSize);
            Assert.Equal(test.Mesh.Header.Flags, result.Header.Flags);
            Assert.Equal(test.Mesh.Header.MinimumExtent, result.Header.MinimumExtent);
            Assert.Equal(test.Mesh.Header.MaximumExtent, result.Header.MaximumExtent);

            Assert.Equal(test.Mesh.Vertices, result.Vertices);
            Assert.Equal(test.Mesh.Normals, result.Normals);
            Assert.Equal(test.Mesh.Faces, result.Faces);
            Assert.Equal(test.Mesh.TextureCoordinates, result.TextureCoordinates);
            Assert.Equal(test.Mesh.Colors, result.Colors);
            Assert.Equal(test.Mesh.VertexMaterials, result.VertexMaterials);
            Assert.Equal(test.Mesh.UserDataEntries, result.UserDataEntries);
            Assert.Equal(test.Mesh.NonUniformKeys, result.NonUniformKeys);

            Assert.Equal(test.Mesh.ExtendedHeader.NumNameIndexes, result.ExtendedHeader.NumNameIndexes);
            Assert.Equal(test.Mesh.ExtendedHeader.NumDummies, result.ExtendedHeader.NumDummies);
            Assert.Equal(test.Mesh.ExtendedHeader.NameLength, result.ExtendedHeader.NameLength);
            Assert.Equal(test.Mesh.ExtendedHeader.PointMaterial, result.ExtendedHeader.PointMaterial);
            Assert.Equal(test.Mesh.ExtendedHeader.PointRadius, result.ExtendedHeader.PointRadius);
            Assert.Equal(test.Mesh.ExtendedHeader.NumMaterials, result.ExtendedHeader.NumMaterials);
            Assert.Equal(test.Mesh.ExtendedHeader.ShadowNameLength0, result.ExtendedHeader.ShadowNameLength0);
            Assert.Equal(test.Mesh.ExtendedHeader.ShadowNameLength1, result.ExtendedHeader.ShadowNameLength1);
            Assert.Equal(test.Mesh.ExtendedHeader.ShadowNameLength2, result.ExtendedHeader.ShadowNameLength2);
            Assert.Equal(test.Mesh.ExtendedHeader.AnimationLength, result.ExtendedHeader.AnimationLength);
            Assert.Equal(test.Mesh.ExtendedHeader.MaterialLibraryTimestamp, result.ExtendedHeader.MaterialLibraryTimestamp);
            Assert.Equal(test.Mesh.ExtendedHeader.Reserved, result.ExtendedHeader.Reserved);
            Assert.Equal(test.Mesh.ExtendedHeader.ExportedScaleFactor, result.ExtendedHeader.ExportedScaleFactor);
            Assert.Equal(test.Mesh.ExtendedHeader.NumNonUniformKeys, result.ExtendedHeader.NumNonUniformKeys);
            Assert.Equal(test.Mesh.ExtendedHeader.NumUniqueMaterials, result.ExtendedHeader.NumUniqueMaterials);

            Assert.Equal(test.Mesh.Dummies.Version, result.Dummies.Version);
            Assert.Equal(test.Mesh.Dummies.Count, result.Dummies.Count);
            foreach (var (expected, actual) in test.Mesh.Dummies.Zip(result.Dummies))
            {
                Assert.Equal(expected.Type, actual.Type);
                Assert.Equal(expected.Name, actual.Name);
                Assert.Equal(expected.Up, actual.Up);
                Assert.Equal(expected.Forward, actual.Forward);
                Assert.Equal(expected.Right, actual.Right);
                Assert.Equal(expected.Position, actual.Position);
                Assert.Equal(expected.BoundingBoxMin, actual.BoundingBoxMin);
                Assert.Equal(expected.BoundingBoxMax, actual.BoundingBoxMax);
            }
        }
    }

    [Theory]
    [MemberData(nameof(Tests))]
    public void Write_Works(MeshTest test)
    {
        using (var ms = new MemoryStream())
        using (var writer = new BrgBinaryWriter(ms))
        {
            test.Mesh.Write(writer);
            writer.Flush();
            var bytes = ms.ToArray();

            Assert.Equal(test.Data, bytes);
        }
    }
}
