using AoMEngineLibrary.Graphics.Prt;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace AoMEngineLibrary.Tests.Graphics.Prt
{
    public class PrtFileTests
    {
        private static readonly string basePath = Path.Combine(Environment.CurrentDirectory, "../../../../data/prt");

        public static readonly IEnumerable<object[]> Tests = Directory.GetFiles(basePath, "*.prt", SearchOption.TopDirectoryOnly).Select(x => new[] { x });

        [Theory]
        [MemberData(nameof(Tests))]
        public void PrtFileReadWriteTest(string fileName)
        {
            string filePath = Path.Combine(basePath, fileName);

            byte[] newData;
            using (var fs = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var ms = new MemoryStream())
            {
                PrtFile file = new PrtFile(fs);
                file.Write(ms);

                ms.Flush();
                ms.Seek(0, SeekOrigin.Begin);
                newData = ms.ToArray();
            }

            byte[] origData = File.ReadAllBytes(filePath);

            Assert.Equal(origData, newData);
        }
    }
}
