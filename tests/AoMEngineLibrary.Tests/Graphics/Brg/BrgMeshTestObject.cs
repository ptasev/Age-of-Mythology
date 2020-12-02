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

        public int VertCount { get; init; }
        public int NormalsCount { get; init; }
        public int TexCoordsCount { get; init; }
        public int FaceCount { get; init; }

        public List<float> NonUniformKeys { get; init;}

        public BrgMeshTestObject(BrgMesh mesh)
        {
            _mesh = mesh;
        }

        public void Validate()
        {
            Assert.NotNull(_mesh.Header);
            Assert.Equal(HeaderVersion, _mesh.Header.Version);

            HasValidVertCount();
            HasValidNormalsCount();
            HasValidTexCoordsCount();
            HasValidFacesCount();

            HasValidNonUniformKeys();
        }

        public void HasValidVertCount()
        {
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
            Assert.Equal(FaceCount, _mesh.Faces.Count);
        }

        public void HasValidNonUniformKeys()
        {
            Assert.Equal(NonUniformKeys.Count, _mesh.NonUniformKeys.Count);
            Assert.Equal(NonUniformKeys, _mesh.NonUniformKeys);
        }
    }
}
