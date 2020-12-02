using AoMEngineLibrary.Graphics.Brg;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace AoMEngineLibrary.Tests.Graphics.Brg
{
    public static class BrgFileTestCases
    {
        private static readonly string _basePath = Path.Combine(Environment.CurrentDirectory, "../../../../data");
        private static readonly Assembly _assy;

        static BrgFileTestCases()
        {
            _assy = typeof(BrgFileTestCases).Assembly;
        }

        public static BrgFileTestObject CreateThrowingAxemanTest(BrgFile brg)
        {
            var test = new BrgFileTestObject(brg)
            {
                MeshCount = 14,
                MaterialCount = 2,
                CreateMeshTests = CreateMeshTests
            };

            return test;

            static List<BrgMeshTestObject> CreateMeshTests(BrgFile brg)
            {
                var test0 = new BrgMeshTestObject(brg.Meshes[0])
                {
                    HeaderVersion = 22,
                    VertCount = 129,
                    NormalsCount = 129,
                    TexCoordsCount = 129,
                    FaceCount = 126,
                    NonUniformKeys = new() { 0, 0.05f, 0.1f, 0.2f, 0.26f, 0.28f, 0.29f, 0.33f, 0.4f, 0.55f, 0.65f, 0.75f, 0.85f, 1 }
                };

                var test1 = new BrgMeshTestObject(brg.Meshes[1])
                {
                    HeaderVersion = 22,
                    VertCount = 129,
                    NormalsCount = 129,
                    TexCoordsCount = 0,
                    FaceCount = 0,
                    NonUniformKeys = new()
                };

                return new List<BrgMeshTestObject>() { test0, test1 };
            }
        }

        public static BrgFile GetBrgFromTestFolder(string fileName)
        {
            string filePath = Path.Combine(_basePath, fileName);
            using (var fs = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var brg = new BrgFile(fs);
                return brg;
            }
        }

        private static BrgFile GetBrgFromResources(string fileName)
        {
            var resName = _assy.GetManifestResourceNames().First(n => n.EndsWith(fileName, StringComparison.InvariantCultureIgnoreCase));
            using (var stream = _assy.GetManifestResourceStream(resName))
            {
                var brg = new BrgFile(stream);
                return brg;
            }
        }
    }
}
