using AoMEngineLibrary.Graphics.Brg;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using Xunit;
using Xunit.Abstractions;

namespace AoMEngineLibrary.Tests.Graphics.Brg;

public class BrgMaterialTests
{
    public record MaterialTest(BrgMaterial Material, byte[] Data);

    public static readonly IEnumerable<object[]> Tests = new List<object[]>
        {
            new[] {
                new MaterialTest(new BrgMaterial()
                    {
                        Id = 1,
                        Flags = BrgMatFlag.BumpMap | BrgMatFlag.SpecularExponent | BrgMatFlag.Alpha | BrgMatFlag.CubeMapInfo,
                        Reserved = 3,
                        DiffuseColor = new Vector3(0.4f),
                        AmbientColor = new Vector3(0.5f),
                        SpecularColor = new Vector3(0.6f),
                        EmissiveColor = new Vector3(0.7f),
                        Opacity = 0.8f,
                        SpecularExponent = 9,
                        DiffuseMapName = "ABC",
                        BumpMapName = "DEF",
                        CubeMapInfo = new BrgCubeMapInfo()
                        {
                            Mode = 10,
                            TextureFactor = 11,
                            CubeMapName = "GHI",
                            TextureMapName = "JKL"
                        }
                    },
                    new byte[]
                    {
                        0x01,0x00,0x00,0x00,0x00,0x80,0x82,0x04,0x00,0x00,0x40,0x40,0x03,0x00,0x00,0x00,
                        0x00,0x00,0x00,0x3F,0x00,0x00,0x00,0x3F,0x00,0x00,0x00,0x3F,0xCD,0xCC,0xCC,0x3E,
                        0xCD,0xCC,0xCC,0x3E,0xCD,0xCC,0xCC,0x3E,0x9A,0x99,0x19,0x3F,0x9A,0x99,0x19,0x3F,
                        0x9A,0x99,0x19,0x3F,0x33,0x33,0x33,0x3F,0x33,0x33,0x33,0x3F,0x33,0x33,0x33,0x3F,
                        0x41,0x42,0x43,0x03,0x00,0x00,0x00,0x44,0x45,0x46,0x00,0x00,0x10,0x41,0xCD,0xCC,
                        0x4C,0x3F,0x0A,0x0B,0x03,0x03,0x47,0x48,0x49,0x4A,0x4B,0x4C,
                    }
                )
            }
        };

    private readonly ITestOutputHelper _output;

    public BrgMaterialTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Theory]
    [MemberData(nameof(Tests))]
    public void Read_Works(MaterialTest test)
    {
        using (var ms = new MemoryStream())
        using (var reader = new BrgBinaryReader(ms))
        {
            ms.Write(test.Data, 0, test.Data.Length);
            ms.Flush();
            ms.Seek(0, SeekOrigin.Begin);

            var result = new BrgMaterial(reader);

            Assert.Equal(test.Material.Id, result.Id);
            Assert.Equal(test.Material.Flags, result.Flags);
            Assert.Equal(test.Material.Reserved, result.Reserved);
            Assert.Equal(test.Material.DiffuseColor, result.DiffuseColor);
            Assert.Equal(test.Material.AmbientColor, result.AmbientColor);
            Assert.Equal(test.Material.SpecularColor, result.SpecularColor);
            Assert.Equal(test.Material.EmissiveColor, result.EmissiveColor);
            Assert.Equal(test.Material.Opacity, result.Opacity);
            Assert.Equal(test.Material.SpecularExponent, result.SpecularExponent);
            Assert.Equal(test.Material.DiffuseMapName, result.DiffuseMapName);
            Assert.Equal(test.Material.BumpMapName, result.BumpMapName);
            Assert.Equal(test.Material.CubeMapInfo, result.CubeMapInfo);
        }
    }

    [Theory]
    [MemberData(nameof(Tests))]
    public void Write_Works(MaterialTest test)
    {
        using (var ms = new MemoryStream())
        using (var writer = new BrgBinaryWriter(ms))
        {
            test.Material.Write(writer);
            writer.Flush();
            var bytes = ms.ToArray();

            Assert.Equal(test.Data, bytes);
        }
    }
}
