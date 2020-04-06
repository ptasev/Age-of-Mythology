using AoMEngineLibrary.Graphics.Brg;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace AoMEngineLibrary.Tests
{
    public class MtrlFileTests
    {
        private static readonly string basePath = Path.Combine(Environment.CurrentDirectory, "../../../../data");

        [Theory]
        [InlineData("ai marker_0.mtrl")]
        public void MtrlFileReadWriteTest(string fileName)
        {
            string filePath = Path.Combine(basePath, fileName);

            byte[] newData;
            using (var fs = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var ms = new MemoryStream())
            {
                MtrlFile file = new MtrlFile();
                file.Read(fs);
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
