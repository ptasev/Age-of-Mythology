using AoMEngineLibrary.Data.Bar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace AoMEngineLibrary.Tests.Data.Bar;

public class BarFileTests
{
    public record FileTest(BarFile File, byte[] Data, bool ReadTestOnly);

    public static readonly IEnumerable<object[]> Tests;

    static BarFileTests()
    {
        Tests = new object[][]
        {
            new object[] { Create1() },
            new object[] { CreateAlphaReadTest() }
        };

        static FileTest Create1()
        {
            var file = new BarFile();
            var e1 = file.CreateEntry("A");
            e1.Offset = 28;
            e1.SetData(new byte[] { 0x01, 0x11 }, new DateTime(2006, 2, 5, 1, 3, 4, 0));
            var e2 = file.CreateEntry("B");
            e2.Offset = 30;
            e2.SetData(new byte[] { 0x02, 0x22, 0x22, 0x02 }, new DateTime(2008, 10, 25, 6, 7, 9, 0));
            var data = new byte[]
            {
                0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x02,0x00,0x00,0x00,
                0x2C,0x00,0x00,0x00,0x22,0x00,0x00,0x00,0x08,0x00,0x00,0x00,0x01,0x11,0x02,0x22,
                0x22,0x02,0x00,0x00,0x00,0x00,0x16,0x00,0x00,0x00,0x1C,0x00,0x00,0x00,0x02,0x00,
                0x00,0x00,0x02,0x00,0x00,0x00,0xD6,0x07,0x02,0x05,0x01,0x03,0x04,0x00,0x41,0x00,
                0x1E,0x00,0x00,0x00,0x04,0x00,0x00,0x00,0x04,0x00,0x00,0x00,0xD8,0x07,0x0A,0x19,
                0x06,0x07,0x09,0x00,0x42,0x00,
            };
            return new FileTest(file, data, false);
        }
        static FileTest CreateAlphaReadTest()
        {
            var file = new BarFile();
            var e1 = file.CreateEntry("A");
            e1.Offset = 28;
            e1.SetData(new byte[] { 0x01, 0x11 }, new DateTime(2006, 2, 5, 1, 3, 4, 0));
            var e2 = file.CreateEntry("B");
            e2.Offset = 30;
            e2.SetData(new byte[] { 0x02, 0x22, 0x22, 0x02 }, new DateTime(2008, 10, 25, 6, 7, 9, 0));
            var data = new byte[]
            {
                0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x02,0x00,0x00,0x00,
                0x24,0x00,0x00,0x00,0x22,0x00,0x00,0x00,0x08,0x00,0x00,0x00,0x01,0x11,0x02,0x22,
                0x22,0x02,0x00,0x00,0x00,0x00,0x12,0x00,0x00,0x00,0x1C,0x00,0x00,0x00,0x02,0x00,
                0x00,0x00,0xD6,0x07,0x02,0x05,0x01,0x03,0x04,0x00,0x41,0x00,0x1E,0x00,0x00,0x00,
                0x04,0x00,0x00,0x00,0xD8,0x07,0x0A,0x19,0x06,0x07,0x09,0x00,0x42,0x00,
            };
            // we don't support writing alpha file so make this read test only
            return new FileTest(file, data, true);
        }
    }

    private readonly ITestOutputHelper _output;

    public BarFileTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Theory]
    [MemberData(nameof(Tests))]
    public void Read_Works(FileTest test)
    {
        const bool expectedLoadedInMem = true;
        Assert.Equal(expectedLoadedInMem, test.File.IsLoadedInMemory);

        using (var ms = new MemoryStream())
        {
            ms.Write(test.Data, 0, test.Data.Length);
            ms.Flush();
            ms.Seek(0, SeekOrigin.Begin);

            var result = BarFile.Open(ms, false);
            var expFile = test.File;

            Assert.Equal(expFile.Unknown, result.Unknown);
            Assert.Equal(expFile.Version, result.Version);
            Assert.NotEqual(expFile.IsLoadedInMemory, result.IsLoadedInMemory);

            Assert.Equal(expFile.Entries.Count, result.Entries.Count);
            foreach (var (expected, actual) in expFile.Entries.Zip(result.Entries))
            {
                Assert.Equal(expected.Offset, actual.Offset);
                Assert.Equal(expected.Size, actual.Size);
                Assert.Equal(expected.Size2, actual.Size2);
                Assert.Equal(expected.Modified, actual.Modified);
                Assert.Equal(expected.FilePath, actual.FilePath);
                Assert.Equal(expected.GetData(), actual.GetData());
                Assert.Equal(expectedLoadedInMem, expected.IsLoadedInMemory);
                Assert.NotEqual(expected.IsLoadedInMemory, result.IsLoadedInMemory);
            }
        }
    }

    [Theory]
    [MemberData(nameof(Tests))]
    public void Write_Works(FileTest test)
    {
        if (test.ReadTestOnly)
            return;

        using (var ms = new MemoryStream())
        {
            test.File.Write(ms, false);
            ms.Flush();
            var bytes = ms.ToArray();

            Assert.Equal(test.Data, bytes);
        }
    }

    [Fact]
    public void Write_file_maintains_existing_isLoadedInMemory()
    {
        // data contains two entries "A", and "B"
        var data = new byte[]
        {
            0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x02,0x00,0x00,0x00,
            0x2C,0x00,0x00,0x00,0x22,0x00,0x00,0x00,0x08,0x00,0x00,0x00,0x01,0x11,0x02,0x22,
            0x22,0x02,0x00,0x00,0x00,0x00,0x16,0x00,0x00,0x00,0x1C,0x00,0x00,0x00,0x02,0x00,
            0x00,0x00,0x02,0x00,0x00,0x00,0xD6,0x07,0x02,0x05,0x01,0x03,0x04,0x00,0x41,0x00,
            0x1E,0x00,0x00,0x00,0x04,0x00,0x00,0x00,0x04,0x00,0x00,0x00,0xD8,0x07,0x0A,0x19,
            0x06,0x07,0x09,0x00,0x42,0x00,
        };
        var aExpectedData = new byte[] { 0x01, 0x11 };
        var bExpectedData = new byte[] { 0x02, 0x22, 0x22, 0x02 };

        using (var ms = new MemoryStream())
        using (var ms2 = new MemoryStream())
        {
            ms.Write(data, 0, data.Length);
            ms.Flush();
            ms.Seek(0, SeekOrigin.Begin);

            var file = BarFile.Open(ms, false);
            Assert.False(file.IsLoadedInMemory);
            Assert.DoesNotContain(file.Entries, x => x.IsLoadedInMemory);

            // Make one of the files loaded in memory
            var ea = file.GetEntry("A");
            var eb = file.GetEntry("B");
            Assert.NotNull(ea);
            Assert.NotNull(eb);
            eb.LoadDataInMemory();

            // Verify state
            Assert.False(file.IsLoadedInMemory);
            Assert.False(ea.IsLoadedInMemory);
            Assert.True(eb.IsLoadedInMemory);

            // Write to stream, verify loaded in mem the same as before
            file.Write(ms2, false);
            Assert.False(file.IsLoadedInMemory);
            Assert.False(ea.IsLoadedInMemory);
            Assert.True(eb.IsLoadedInMemory);
            Assert.Equal(aExpectedData, ea.GetData());
            Assert.Equal(bExpectedData, eb.GetData());
        }
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void LoadInMemory_works_and_closes_stream_accordingly(bool leaveOpen)
    {
        // data contains two entries "A", and "B"
        var data = new byte[]
        {
            0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x02,0x00,0x00,0x00,
            0x2C,0x00,0x00,0x00,0x22,0x00,0x00,0x00,0x08,0x00,0x00,0x00,0x01,0x11,0x02,0x22,
            0x22,0x02,0x00,0x00,0x00,0x00,0x16,0x00,0x00,0x00,0x1C,0x00,0x00,0x00,0x02,0x00,
            0x00,0x00,0x02,0x00,0x00,0x00,0xD6,0x07,0x02,0x05,0x01,0x03,0x04,0x00,0x41,0x00,
            0x1E,0x00,0x00,0x00,0x04,0x00,0x00,0x00,0x04,0x00,0x00,0x00,0xD8,0x07,0x0A,0x19,
            0x06,0x07,0x09,0x00,0x42,0x00,
        };
        var aExpectedData = new byte[] { 0x01, 0x11 };
        var bExpectedData = new byte[] { 0x02, 0x22, 0x22, 0x02 };

        using (var ms = new MemoryStream())
        using (var ms2 = new MemoryStream())
        {
            ms.Write(data, 0, data.Length);
            ms.Flush();
            ms.Seek(0, SeekOrigin.Begin);

            var file = BarFile.Open(ms, leaveOpen);
            Assert.False(file.IsLoadedInMemory);
            Assert.DoesNotContain(file.Entries, x => x.IsLoadedInMemory);

            // Make all files loaded in memory
            file.LoadIntoMemory();
            Assert.True(file.Entries.All(x => x.IsLoadedInMemory));
            Assert.True(file.IsLoadedInMemory);

            // Verify stream taken care of accordingly
            if (leaveOpen)
            {
                Assert.NotNull(file.ArchiveStream);
                _ = ms.Seek(0, SeekOrigin.Begin); // should not throw
            }
            else
            {
                Assert.Null(file.ArchiveStream);
                Assert.Throws<ObjectDisposedException>(() => ms.Seek(0, SeekOrigin.Begin));
            }

            // Verify data loaded in mem
            var ea = file.GetEntry("A");
            var eb = file.GetEntry("B");
            Assert.NotNull(ea);
            Assert.Equal(aExpectedData, ea.GetData());
            Assert.NotNull(eb);
            Assert.Equal(bExpectedData, eb.GetData());
        }
    }
}
