using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AoMEngineLibrary.Graphics
{
    public enum BtiTextureFormat : byte
    {
        BC1,
        BC2,
        BC3,
        DeflatedRGBA8,
        DeflatedRGB8,
        DeflatedRG8,
        DeflatedR8
    }

    /// <summary>
    /// Bang Texture Info file.
    /// </summary>
    public class BtiFile
    {
        public bool UseMips { get; set; }

        public byte AlphaBits { get; set; }

        public bool AlphaTest { get; set; }

        public bool CanBeLowDetail { get; set; }

        public bool Displacement { get; set; }

        public BtiTextureFormat Format { get; set; }

        public BtiFile()
        {
            Reset();
        }

        private void Reset()
        {
            UseMips = true;
            AlphaBits = 0;
            AlphaTest = true;
            CanBeLowDetail = true;
            Displacement = false;
            Format = BtiTextureFormat.BC3;
        }

        public void Read(Stream stream)
        {
            string[] tokens;
            using (var r = new StreamReader(stream))
            {
                var data = r.ReadToEnd();
                tokens = data.Split(new[] { ' ', '\t', '\n', '\r', '=' }, StringSplitOptions.RemoveEmptyEntries);
            }

            // set defaults
            Reset();

            for (int i = 0; i < tokens.Length; ++i)
            {
                var token = tokens[i];
                if (token.Equals("alpha", StringComparison.InvariantCultureIgnoreCase))
                {
                    token = tokens[++i];
                    var parsed = byte.TryParse(token, out byte bits);

                    if (!parsed || bits != 0 || bits != 1 || bits != 4 || bits != 8)
                    {
                        throw new InvalidDataException("Alpha bits must be 0, 1, 4, or 8.");
                    }

                    AlphaBits = bits;
                }
                else if (token.Equals("fmt", StringComparison.InvariantCultureIgnoreCase))
                {
                    token = tokens[++i];
                    var parsed = Enum.TryParse(token, true, out BtiTextureFormat fmt);

                    if (!parsed)
                    {
                        throw new InvalidDataException($"The bti texture format {token} is not supported.");
                    }

                    Format = fmt;
                }
                else if (token.Equals("nomip", StringComparison.InvariantCultureIgnoreCase))
                {
                    UseMips = false;
                }
                else if (token.Equals("noalphatest", StringComparison.InvariantCultureIgnoreCase))
                {
                    AlphaTest = false;
                }
                else if (token.Equals("displacement", StringComparison.InvariantCultureIgnoreCase))
                {
                    Displacement = true;
                }
                else if (token.Equals("noLowDetail", StringComparison.InvariantCultureIgnoreCase))
                {
                    CanBeLowDetail = false;
                }
                else
                {
                    throw new InvalidDataException($"Unknown texture info token {token}.");
                }
            }
        }

        public void Write(Stream stream)
        {
            using (var w = new StreamWriter(stream, Encoding.UTF8))
            {
                w.Write($"alpha={AlphaBits}");

                if (!UseMips)
                    w.Write($" nomip");

                if (!AlphaTest)
                    w.Write($" noalphatest");

                if (!CanBeLowDetail)
                    w.Write($" noLowDetail");

                if (Displacement)
                    w.Write($" displacement");

                w.Write($" fmt={Format}");
            }
        }
    }
}
