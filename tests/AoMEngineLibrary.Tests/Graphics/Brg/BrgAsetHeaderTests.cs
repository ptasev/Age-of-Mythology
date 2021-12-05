using AoMEngineLibrary.Graphics.Brg;
using System.IO;
using Xunit;

namespace AoMEngineLibrary.Tests.Graphics.Brg;

public class BrgAsetHeaderTests
{
    private readonly BrgAsetHeader _brgAsetHeader = new BrgAsetHeader()
    {
        NumFrames = 1,
        InvFrames = 2.2f,
        AnimTime = 3.3f,
        Frequency = 4.4f,
        Spf = 5.5f,
        Fps = 6.6f,
        Reserved = 7
    };
    private readonly byte[] _data = new byte[]
    {
            0x01,0x00,0x00,0x00,
            0xCD,0xCC,0x0C,0x40,
            0x33,0x33,0x53,0x40,
            0xCD,0xCC,0x8C,0x40,
            0x00,0x00,0xB0,0x40,
            0x33,0x33,0xD3,0x40,
            0x07,0x00,0x00,0x00,
    };

    [Fact]
    public void Read_Works()
    {
        using (var ms = new MemoryStream())
        using (var reader = new BrgBinaryReader(ms))
        {
            ms.Write(_data, 0, _data.Length);
            ms.Flush();
            ms.Seek(0, SeekOrigin.Begin);

            var result = new BrgAsetHeader(reader);

            Assert.Equal(_brgAsetHeader.NumFrames, result.NumFrames);
            Assert.Equal(_brgAsetHeader.InvFrames, result.InvFrames);
            Assert.Equal(_brgAsetHeader.AnimTime, result.AnimTime);
            Assert.Equal(_brgAsetHeader.Frequency, result.Frequency);
            Assert.Equal(_brgAsetHeader.Spf, result.Spf);
            Assert.Equal(_brgAsetHeader.Fps, result.Fps);
            Assert.Equal(_brgAsetHeader.Reserved, result.Reserved);
        }
    }

    [Fact]
    public void Write_Works()
    {
        using (var ms = new MemoryStream())
        using (var writer = new BrgBinaryWriter(ms))
        {
            _brgAsetHeader.Write(writer);
            writer.Flush();
            var bytes = ms.ToArray();

            Assert.Equal(_data, bytes);
        }
    }
}
