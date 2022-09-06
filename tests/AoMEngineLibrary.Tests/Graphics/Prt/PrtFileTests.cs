using AoMEngineLibrary.Graphics.Prt;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
            var filePath = Path.Combine(basePath, fileName);

            byte[] newData;
            using (var fs = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var ms = new MemoryStream())
            {
                var file = new PrtFile(fs);
                file.Write(ms);

                ms.Flush();
                ms.Seek(0, SeekOrigin.Begin);
                newData = ms.ToArray();
            }

            var origData = File.ReadAllBytes(filePath);

            Assert.Equal(origData, newData);
        }

        [Theory]
        [MemberData(nameof(Tests))]
        public void XmlSerialize_Deserialize_Test(string fileName)
        {
            var filePath = Path.Combine(basePath, fileName);

            byte[] newData;
            using (var fs = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var ms = new MemoryStream())
            using (var ms2 = new MemoryStream())
            {
                var file = new PrtFile(fs);
                file.SerializeAsXml(ms);
                ms.Flush();
                ms.Seek(0, SeekOrigin.Begin);
                file = PrtFile.DeserializeAsXml(ms);

                file.Write(ms2);
                ms2.Flush();
                ms2.Seek(0, SeekOrigin.Begin);
                newData = ms2.ToArray();
            }

            var origData = File.ReadAllBytes(filePath);

            Assert.Equal(origData, newData);
        }
    }
}
