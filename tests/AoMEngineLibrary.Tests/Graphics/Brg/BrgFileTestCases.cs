using AoMEngineLibrary.Graphics.Brg;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
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

        public static BrgFileTestObject CreateAIMarkerTest(BrgFile brg)
        {
            var test = new BrgFileTestObject(brg)
            {
                AnimationDuration = 0.0f,
                MeshCount = 1,
                MaterialCount = 2,
                CreateMeshTests = CreateMeshTests,
                CreateMatTests = CreateMatTests
            };

            return test;

            static List<BrgMeshTestObject> CreateMeshTests(BrgFile brg)
            {
                var test0 = new BrgMeshTestObject(brg.Meshes[0])
                {
                    HeaderVersion = 22,
                    Format = BrgMeshFormat.HASFACENORMALS,
                    InterpType = BrgMeshInterpolationType.Default,
                    AnimType = BrgMeshAnimType.KeyFrame,
                    Flags = BrgMeshFlag.TEXCOORDSA | BrgMeshFlag.MATERIAL,
                    VertCount = 79,
                    NormalsCount = 79,
                    TexCoordsCount = 79,
                    FaceCount = 40,
                    NonUniformKeys = new()
                };

                return new List<BrgMeshTestObject>() { test0 };
            }
            static List<BrgMaterialTestObject> CreateMatTests(BrgFile brg)
            {
                var test0 = new BrgMaterialTestObject(brg.Materials[0])
                {
                    Flags = BrgMatFlag.SpecularExponent | BrgMatFlag.Alpha,
                    DiffuseMapName = "",
                    AmbientColor = new Vector3(0.0f, 0.9176471f, 0.0f),
                    DiffuseColor = new Vector3(0.0f, 0.9176471f, 0.0f)
                };

                var test1 = new BrgMaterialTestObject(brg.Materials[1])
                {
                    Flags = BrgMatFlag.SpecularExponent | BrgMatFlag.Alpha,
                    DiffuseMapName = "",
                    AmbientColor = new Vector3(1.0f, 0.9176471f, 0.0f),
                    DiffuseColor = new Vector3(1.0f, 0.9176471f, 0.0f)
                };

                return new List<BrgMaterialTestObject>() { test0, test1 };
            }
        }
        public static BrgFileTestObject CreateArcherThrowingAxemanTest(BrgFile brg)
        {
            var test = new BrgFileTestObject(brg)
            {
                AnimationDuration = 1.3f,
                MeshCount = 14,
                MaterialCount = 2,
                CreateMeshTests = CreateMeshTests,
                CreateMatTests = CreateMatTests
            };

            return test;

            static List<BrgMeshTestObject> CreateMeshTests(BrgFile brg)
            {
                var test0 = new BrgMeshTestObject(brg.Meshes[0])
                {
                    HeaderVersion = 22,
                    Format = BrgMeshFormat.HASFACENORMALS | BrgMeshFormat.ANIMATED,
                    InterpType = BrgMeshInterpolationType.Default,
                    AnimType = BrgMeshAnimType.NonUniform,
                    Flags = BrgMeshFlag.TEXCOORDSA | BrgMeshFlag.MATERIAL | BrgMeshFlag.ATTACHPOINTS,
                    VertCount = 129,
                    NormalsCount = 129,
                    TexCoordsCount = 129,
                    FaceCount = 126,
                    NonUniformKeys = new() { 0, 0.05f, 0.1f, 0.2f, 0.26f, 0.28f, 0.29f, 0.33f, 0.4f, 0.55f, 0.65f, 0.75f, 0.85f, 1 }
                };

                var test1 = new BrgMeshTestObject(brg.Meshes[1])
                {
                    HeaderVersion = 22,
                    Format = BrgMeshFormat.HASFACENORMALS | BrgMeshFormat.ANIMATED,
                    InterpType = BrgMeshInterpolationType.Default,
                    AnimType = BrgMeshAnimType.KeyFrame,
                    Flags = BrgMeshFlag.TEXCOORDSA | BrgMeshFlag.MATERIAL | BrgMeshFlag.ATTACHPOINTS | BrgMeshFlag.SECONDARYMESH,
                    VertCount = 129,
                    NormalsCount = 129,
                    TexCoordsCount = 0,
                    FaceCount = 0,
                    NonUniformKeys = new()
                };

                return new List<BrgMeshTestObject>() { test0, test1 };
            }
            static List<BrgMaterialTestObject> CreateMatTests(BrgFile brg)
            {
                var test0 = new BrgMaterialTestObject(brg.Materials[0])
                {
                    Flags = BrgMatFlag.WrapUTx1 | BrgMatFlag.WrapVTx1 | BrgMatFlag.Alpha,
                    DiffuseMapName = "Archer N Throwing Axeman Standard",
                    AmbientColor = new Vector3(1.0f, 1.0f, 1.0f),
                    DiffuseColor = new Vector3(1.0f, 1.0f, 1.0f)
                };

                var test1 = new BrgMaterialTestObject(brg.Materials[1])
                {
                    Flags = BrgMatFlag.WrapUTx1 | BrgMatFlag.WrapVTx1 | BrgMatFlag.Alpha | BrgMatFlag.PixelXForm1,
                    DiffuseMapName = "Archer N Throwing Axeman Standard",
                    AmbientColor = new Vector3(1.0f, 1.0f, 1.0f),
                    DiffuseColor = new Vector3(1.0f, 1.0f, 1.0f)
                };

                return new List<BrgMaterialTestObject>() { test0, test1 };
            }
        }

        public static BrgFileTestObject CreateScenDwarvenForgeTredTest(BrgFile brg)
        {
            var test = new BrgFileTestObject(brg)
            {
                AnimationDuration = 2f,
                MeshCount = 40,
                MaterialCount = 1,
                CreateMeshTests = CreateMeshTests,
                CreateMatTests = CreateMatTests
            };

            return test;

            static List<BrgMeshTestObject> CreateMeshTests(BrgFile brg)
            {
                var test0 = new BrgMeshTestObject(brg.Meshes[0])
                {
                    HeaderVersion = 22,
                    Format = BrgMeshFormat.HASFACENORMALS | BrgMeshFormat.ANIMATED | BrgMeshFormat.ANIMTEXCOORDSNAP,
                    InterpType = BrgMeshInterpolationType.Default,
                    AnimType = BrgMeshAnimType.KeyFrame,
                    Flags = BrgMeshFlag.TEXCOORDSA | BrgMeshFlag.MATERIAL | BrgMeshFlag.ATTACHPOINTS | BrgMeshFlag.ANIMTEXCOORDS,
                    VertCount = 16,
                    NormalsCount = 16,
                    TexCoordsCount = 16,
                    FaceCount = 10,
                    NonUniformKeys = new()
                };

                var test1 = new BrgMeshTestObject(brg.Meshes[1])
                {
                    HeaderVersion = 22,
                    Format = BrgMeshFormat.HASFACENORMALS | BrgMeshFormat.ANIMTEXCOORDSNAP,
                    InterpType = BrgMeshInterpolationType.Default,
                    AnimType = BrgMeshAnimType.KeyFrame,
                    Flags = BrgMeshFlag.TEXCOORDSA | BrgMeshFlag.MATERIAL | BrgMeshFlag.ATTACHPOINTS | BrgMeshFlag.SECONDARYMESH | BrgMeshFlag.ANIMTEXCOORDS,
                    VertCount = 16,
                    NormalsCount = 16,
                    TexCoordsCount = 16,
                    FaceCount = 0,
                    NonUniformKeys = new()
                };

                return new List<BrgMeshTestObject>() { test0, test1 };
            }
            static List<BrgMaterialTestObject> CreateMatTests(BrgFile brg)
            {
                var test0 = new BrgMaterialTestObject(brg.Materials[0])
                {
                    Flags = BrgMatFlag.WrapUTx1 | BrgMatFlag.WrapVTx1 | BrgMatFlag.Alpha,
                    DiffuseMapName = "Scenario A Dwarven Forge Belt",
                    AmbientColor = new Vector3(1.0f, 1.0f, 1.0f),
                    DiffuseColor = new Vector3(1.0f, 1.0f, 1.0f)
                };

                return new List<BrgMaterialTestObject>() { test0 };
            }
        }
    }
}
