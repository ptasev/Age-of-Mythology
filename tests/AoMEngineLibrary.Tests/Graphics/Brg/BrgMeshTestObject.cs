using AoMEngineLibrary.Graphics.Brg;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AoMEngineLibrary.Tests.Graphics.Brg
{
    public class BrgMeshTestObject
    {
        private readonly BrgMesh _mesh;

        public int HeaderVersion { get; init; }
        public BrgMeshFormat Format { get; init; }
        public BrgMeshInterpolationType InterpType { get; init; }
        public BrgMeshAnimType AnimType { get; init; }
        public BrgMeshFlag Flags { get; init; }

        public int VertCount { get; init; }
        public int NormalsCount { get; init; }
        public int TexCoordsCount { get; init; }
        public int FaceCount { get; init; }

        public List<float> NonUniformKeys { get; init;}

        public BrgMeshTestObject(BrgMesh mesh)
        {
            _mesh = mesh;
            NonUniformKeys = new List<float>();
        }

        public void Validate()
        {
            Assert.NotNull(_mesh.Header);
            Assert.Equal(HeaderVersion, _mesh.Header.Version);

            Assert.Equal(Format, _mesh.Header.Format);
            Assert.Equal(InterpType, _mesh.Header.InterpolationType);
            Assert.Equal(AnimType, _mesh.Header.AnimationType);
            Assert.Equal(Flags, _mesh.Header.Flags);

            HasValidVertCount();
            HasValidNormalsCount();
            HasValidTexCoordsCount();
            HasValidFacesCount();

            HasValidNonUniformKeys();
        }

        public void HasValidVertCount()
        {
            Assert.Equal(VertCount, _mesh.Header.NumVertices);
            Assert.Equal(VertCount, _mesh.Vertices.Count);
        }

        public void HasValidNormalsCount()
        {
            Assert.Equal(NormalsCount, _mesh.Normals.Count);
        }

        public void HasValidTexCoordsCount()
        {
            Assert.Equal(TexCoordsCount, _mesh.TextureCoordinates.Count);
        }

        public void HasValidFacesCount()
        {
            if (!_mesh.Header.Flags.HasFlag(BrgMeshFlag.Secondary))
                Assert.Equal(FaceCount, _mesh.Header.NumFaces);
            Assert.Equal(FaceCount, _mesh.Faces.Count);
        }

        public void HasValidNonUniformKeys()
        {
            Assert.Equal(NonUniformKeys.Count, _mesh.ExtendedHeader.NumNonUniformKeys);
            Assert.Equal(NonUniformKeys.Count, _mesh.NonUniformKeys.Count);
            Assert.Equal(NonUniformKeys, _mesh.NonUniformKeys);
        }
    }
}
