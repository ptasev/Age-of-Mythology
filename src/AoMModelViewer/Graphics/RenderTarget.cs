using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AoMModelViewer.Graphics
{
    public class RenderTarget
    {
        private readonly byte[] rawImage;
        private readonly int wm1;

        public int Width { get; }
        public int Height { get; }

        public RenderTarget(int width, int height)
        {
            wm1 = width - 1;
            Width = width;
            Height = height;
            rawImage = new byte[width * height * 4];

            Color black = new Color(0, 0, 0, 255);
            for (int i = 0; i < height; ++i)
            {
                for (int j = 0; j < width; ++j)
                {
                    Set(j, i, ref black);
                }
            }
        }

        public byte[] GetRawData()
        {
            return (byte[])rawImage.Clone();
        }

        public void Set(int col, int row, ref Color color)
        {
            if (row >= Height || row < 0 || col >= Width || col < 0) return;
            Span<byte> image = rawImage;
            MemoryMarshal.Write<uint>(image.Slice(((wm1 - row) * Width + col) * 4, 4), ref color.color);
        }

        public static BitmapSource CreateBitmapSource(byte[] rawImage, int width, int height)
        {
            PixelFormat pf = PixelFormats.Bgra32;
            var bitmap = BitmapSource.Create(width, height,
                96, 96, pf, null,
                rawImage, (width * pf.BitsPerPixel + 7) / 8);
            return bitmap;
        }
    }
}
