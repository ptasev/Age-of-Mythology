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
                "ai marker.brg",
                (Func<string, BrgFile>)BrgFileTestCases.GetBrgFromTestFolder,
                (Func<BrgFile, BrgFileTestObject>)BrgFileTestCases.CreateAIMarkerTest
            },
            new object[] 
            { 
                "archer n throwing axeman_attacka.brg",
                (Func<string, BrgFile>)BrgFileTestCases.GetBrgFromTestFolder,
                (Func<BrgFile, BrgFileTestObject>)BrgFileTestCases.CreateArcherThrowingAxemanTest 
            },
            new object[]
            {
                "scenario a dwarven forge tred.brg",
                (Func<string, BrgFile>)BrgFileTestCases.GetBrgFromTestFolder,
                (Func<BrgFile, BrgFileTestObject>)BrgFileTestCases.CreateScenDwarvenForgeTredTest
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
    }
}
