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

        public float AnimationDuration { get; init; }

        public Func<BrgFile, List<BrgMeshTestObject>> CreateMeshTests { get; init; }

        public Func<BrgFile, List<BrgMaterialTestObject>> CreateMatTests { get; init; }

        public BrgFileTestObject(BrgFile brg)
        {
            _brg = brg;
        }

        public void Validate()
        {
            Assert.NotNull(_brg.Header);
            Assert.Equal(BrgHeader.BangMagic, _brg.Header.Magic);

            // Test Meshes
            HasValidMeshData();

            // Test Materials
            Assert.Equal(MaterialCount, _brg.Header.NumMaterials);
            Assert.Equal(MaterialCount, _brg.Materials.Count);
            var matTests = CreateMatTests(_brg);
            foreach (var test in matTests)
            {
                test.Validate();
            }

            // Animation
            Assert.Equal(AnimationDuration, _brg.Animation.Duration);
            if (MeshCount > 0)
            {
                Assert.Equal(AnimationDuration, _brg.Meshes[0].ExtendedHeader.AnimationLength);
            }
        }

        public void HasValidMeshData()
        {
            Assert.Equal(MeshCount, _brg.Header.NumMeshes);
            Assert.Equal(MeshCount, _brg.Meshes.Count);

            var meshTests = CreateMeshTests(_brg);
            foreach (var test in meshTests)
            {
                test.Validate();
            }
        }
    }
}
