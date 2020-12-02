using AoMEngineLibrary.Graphics.Brg;
using System;
using System.Collections.Generic;
using Xunit;

namespace AoMEngineLibrary.Tests.Graphics.Brg
{
    public class BrgFileTestObject
    {
        private readonly BrgFile _brg;

        public int MeshCount { get; init; }

        public int MaterialCount { get; init; }

        public Func<BrgFile, List<BrgMeshTestObject>> CreateMeshTests { get; init; }

        public BrgFileTestObject(BrgFile brg)
        {
            _brg = brg;
        }

        public void Validate()
        {
            Assert.NotNull(_brg.Header);
            Assert.Equal("BANG", _brg.Header.Magic);

            HasValidMeshCount();
            HasValidMatCount();
            HasValidMeshData();
        }

        public void HasValidMeshCount()
        {
            Assert.Equal(MeshCount, _brg.Header.NumMeshes);
            Assert.Equal(MeshCount, _brg.Meshes.Count);
        }

        public void HasValidMatCount()
        {
            Assert.Equal(MaterialCount, _brg.Header.NumMaterials);
            Assert.Equal(MaterialCount, _brg.Materials.Count);
        }

        public void HasValidMeshData()
        {
            var meshTests = CreateMeshTests(_brg);
            foreach (var test in meshTests)
            {
                test.Validate();
            }
        }
    }
}
