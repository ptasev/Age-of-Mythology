using AoMEngineLibrary.Graphics;
using AoMEngineLibrary.Graphics.Ddt;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Buffers.Binary;

namespace AoMEngineLibrary.Benchmarks
{
    public class Program
    {
        public class DdtConversionBenchmark
        {
            private readonly Rgb24 TwoColorEndpoint0 = new Rgb24(32, 64, 128);
            private readonly Rgb24 TwoColorEndpoint1 = new Rgb24(16, 32, 64);
            private readonly Bgr565 _bgr565col0;
            private readonly Bgr565 _bgr565col1;
            private readonly Rgb24 _bgr565rgb24col0;
            private readonly Rgb24 _bgr565rgb24col1;

            private readonly DdtFile _ddt;
            private readonly DdtImageConverter bc11;
            private readonly DdtImageConverter bc12;
            private readonly DdtImageConverter bc13;

            [Params(1000, 10000, 100000)]
            public int N;

            public DdtConversionBenchmark()
            {
                _bgr565col0 = new Bgr565();
                _bgr565col0.FromRgb24(TwoColorEndpoint0);
                _bgr565col1 = new Bgr565();
                _bgr565col1.FromRgb24(TwoColorEndpoint1);

                _bgr565rgb24col0 = new Rgb24();
                _bgr565rgb24col0.FromVector4(_bgr565col0.ToVector4());
                _bgr565rgb24col1 = new Rgb24();
                _bgr565rgb24col1.FromVector4(_bgr565col1.ToVector4());

                _ddt = new DdtFile();
                _ddt.Properties = DdtProperty.Normal;
                _ddt.Format = DdtFormat.Dxt1;
                _ddt.AlphaBits = 0;
                _ddt.MipMapLevels = 1;
                _ddt.Width = 4;
                _ddt.Height = 4;
                _ddt.Data = new byte[1][][]
                {
                new byte[1][] { GenerateBc1Block() }
                };
                // this test is no longer meaningful
                bc11 = new DdtImageConverter();
                bc12 = new DdtImageConverter();
                bc13 = new DdtImageConverter();
            }

            [Benchmark(Baseline = true)]
            public void BC11()
            {
                for (int i = 0; i < N; ++i)
                    bc11.Convert(_ddt);
            }

            [Benchmark]
            public void BC12()
            {
                for (int i = 0; i < N; ++i)
                    bc12.Convert(_ddt);
            }

            [Benchmark]
            public void BC13()
            {
                for (int i = 0; i < N; ++i)
                    bc13.Convert(_ddt);
            }

            private byte[] GenerateBc1Block()
            {
                Bgr565 col0 = _bgr565col0;
                Bgr565 col1 = _bgr565col1;

                var dest = new byte[8];
                Span<byte> destSpan = dest.AsSpan();
                BinaryPrimitives.WriteUInt16LittleEndian(destSpan, col0.PackedValue);
                BinaryPrimitives.WriteUInt16LittleEndian(destSpan.Slice(2), col1.PackedValue);

                // First half of pixels use col0, second half col1
                const int pixels = 16;
                const int halfwayPoint = pixels / 2;

                // Set the indices, 2bpp
                int shift = 0;
                uint indices = 0;
                for (int i = 0; i < pixels; ++i, shift += 2)
                {
                    if (i < halfwayPoint)
                    {
                        // use col0 in the first half
                        indices |= (0u << shift);
                    }
                    else
                    {
                        // use col1 in the second half
                        indices |= (1u << shift);
                    }
                }

                BinaryPrimitives.WriteUInt32LittleEndian(destSpan.Slice(4), indices);
                return dest;
            }
        }

        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<DdtConversionBenchmark>();
        }
    }
}
