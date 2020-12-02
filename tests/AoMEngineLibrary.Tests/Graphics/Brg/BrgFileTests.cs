using AoMEngineLibrary.Graphics.Brg;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace AoMEngineLibrary.Tests.Graphics.Brg
{
    public class BrgFileTests
    {
        private static readonly string basePath = Path.Combine(Environment.CurrentDirectory, "../../../../data");

        public static readonly IEnumerable<object[]> Tests = new List<object[]>
        {
            new object[] 
            { 
                "archer n throwing axeman_attacka.brg",
                (Func<string, BrgFile>)BrgFileTestCases.GetBrgFromTestFolder,
                (Func<BrgFile, BrgFileTestObject>)BrgFileTestCases.CreateThrowingAxemanTest 
            }
        };

        [Theory]
        [MemberData(nameof(Tests))]
        public void BrgFile_CanReadData(string fileName, Func<string, BrgFile> createBrgFunc, Func<BrgFile, BrgFileTestObject> createTestFunc)
        {
            // Arrange/Act
            var brg = createBrgFunc(fileName);
            var test = createTestFunc(brg);

            // Assert
            test.Validate();
        }

        [Theory]
        [MemberData(nameof(Tests))]
        public void BrgFile_CanWriteData(string fileName, Func<string, BrgFile> createBrgFunc, Func<BrgFile, BrgFileTestObject> createTestFunc)
        {
            // Arrange
            var brg = createBrgFunc(fileName);

            // Act
            using (var ms = new MemoryStream())
            {
                brg.Write(ms);
                ms.Flush();
                ms.Seek(0, SeekOrigin.Begin);
                brg = new BrgFile(ms);
            }
            var test = createTestFunc(brg);

            // Assert
            test.Validate();
        }

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
    }
}
