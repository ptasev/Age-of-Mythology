using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AoMEngineLibrary.Graphics.Textures
{
    public class BcBlockDecoder<TBlock>
		where TBlock: unmanaged, IBcBlock
    {
		public const int PixelsPerBlock = 16;
		public const int BlockPixelWidth = 4;
		public const int BlockPixelHeight = 4;

		public static BcBlockDecoder<TBlock> Instance { get; } = new BcBlockDecoder<TBlock>();

		public readonly int BlockSize = Unsafe.SizeOf<TBlock>();

		public void Decode<TPixel>(ReadOnlySpan<byte> source, Image<TPixel> destination)
			where TPixel : unmanaged, IPixel<TPixel>
		{
			int blockWidth = (int)MathF.Ceiling(destination.Width / (float)BlockPixelWidth);
			int blockHeight = (int)MathF.Ceiling(destination.Height / (float)BlockPixelHeight);
			var blockCount = blockWidth * blockHeight;

			if (source.Length != (blockCount * BlockSize))
			{
				throw new InvalidDataException("The source image bytes are not the required length.");
			}

			var vectors = new Vector4[PixelsPerBlock].AsSpan();
			var encodedBlocks = MemoryMarshal.Cast<byte, TBlock>(source);
			int blockIndex = 0;
			var pixOps = PixelOperations<TPixel>.Instance;
			for (int blockRow = 0; blockRow < blockHeight; ++blockRow)
			{
				var startRow = blockRow * BlockPixelHeight;
				var endRow = startRow + BlockPixelHeight;
				for (int blockCol = 0; blockCol < blockWidth; ++blockCol, ++blockIndex)
				{
					var block = encodedBlocks[blockIndex];
					block.Decode(vectors);

					var vecIndex = 0;
					var startCol = blockCol * BlockPixelWidth;
					for (int row = startRow; row < endRow; ++row, vecIndex += BlockPixelWidth)
					{
						var rowSpan = destination.GetPixelRowSpan(row).Slice(startCol, BlockPixelWidth);
						var vecSpan = vectors.Slice(vecIndex, BlockPixelWidth);
						pixOps.FromVector4Destructive(Configuration.Default, vecSpan, rowSpan, PixelConversionModifiers.Scale);
                    }
				}
			}
		}

		public void Decode<TPixel>(ReadOnlySpan<byte> source, Span<TPixel> destination, int width, int height)
			where TPixel : unmanaged, IPixel<TPixel>
		{
			int blockWidth = (int)MathF.Ceiling(width / (float)BlockPixelWidth);
			int blockHeight = (int)MathF.Ceiling(height / (float)BlockPixelHeight);
			var blockCount = blockWidth * blockHeight;

			if (source.Length != (blockCount * BlockSize))
			{
				throw new InvalidDataException("The source image bytes are not the required length.");
			}

			var vectors = new Vector4[PixelsPerBlock].AsSpan();
			var encodedBlocks = MemoryMarshal.Cast<byte, TBlock>(source);
			int blockIndex = 0;
			var pixOps = PixelOperations<TPixel>.Instance;
			for (int blockRow = 0; blockRow < blockHeight; ++blockRow)
			{
				var startRow = blockRow * BlockPixelHeight;
				var endRow = startRow + BlockPixelHeight;
				for (int blockCol = 0; blockCol < blockWidth; ++blockCol, ++blockIndex)
				{
					var block = encodedBlocks[blockIndex];
					block.Decode(vectors);

					var vecIndex = 0;
					var startCol = blockCol * BlockPixelWidth;
					for (int row = startRow; row < endRow; ++row, vecIndex += BlockPixelWidth)
					{
						var rowSpan = destination.Slice(row * width + startCol, BlockPixelWidth);
						var vecSpan = vectors.Slice(vecIndex, BlockPixelWidth);
						pixOps.FromVector4Destructive(Configuration.Default, vecSpan, rowSpan, PixelConversionModifiers.Scale);
					}
				}
			}
		}
	}
}
