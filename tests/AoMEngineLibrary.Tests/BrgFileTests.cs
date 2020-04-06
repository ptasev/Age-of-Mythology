using AoMEngineLibrary.Graphics.Brg;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace AoMEngineLibrary.Tests
{
    public class BrgFileTests
    {
        private static readonly string basePath = Path.Combine(Environment.CurrentDirectory, "../../../../data");

        public static readonly IEnumerable<object[]> nonUniformKeyTestData = new List<object[]>
        {
            new object[] { "archer n throwing axeman_attacka.brg", new List<float> { 0, 0.05f, 0.1f, 0.2f, 0.26f, 0.28f, 0.29f, 0.33f, 0.4f, 0.55f, 0.65f, 0.75f, 0.85f, 1 } }
        };

        [Theory]
        [InlineData("ai marker.brg")]
        [InlineData("archer n throwing axeman_attacka.brg")]
        [InlineData("scenario a dwarven forge tred.brg")]
        public void BrgFileReadWriteTest(string fileName)
        {
            // TODO: Implement proper equality test for brg.
            string filePath = Path.Combine(basePath, fileName);

            byte[] newData;
            using (var fs = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var ms = new MemoryStream())
            {
                BrgFile file = new BrgFile(fs);
                file.Write(ms);

                ms.Flush();
                ms.Seek(0, SeekOrigin.Begin);
                newData = ms.ToArray();

                file.Write(File.Open(filePath + "2", FileMode.Create, FileAccess.Write, FileShare.Read));
            }

            byte[] origData = File.ReadAllBytes(filePath);

            Assert.Equal(origData, newData);
        }

        [Theory]
        [MemberData(nameof(nonUniformKeyTestData))]
        public void NonUniformKeysTest(string fileName, List<float> expectedKeys)
        {
            string filePath = Path.Combine(basePath, fileName);

            using (var fs = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var ms = new MemoryStream())
            {
                BrgFile file = new BrgFile(fs);
                
                Assert.Equal(expectedKeys, file.Meshes[0].NonUniformKeys);
            }

        }
    }
}
