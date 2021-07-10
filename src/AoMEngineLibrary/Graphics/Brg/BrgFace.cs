using System;

namespace AoMEngineLibrary.Graphics.Brg
{
    public class BrgFace
    {
        public const int IndexCount = 3;

        public short MaterialIndex { get; set; }

        public ushort A { get; set; }

        public ushort B { get; set; }

        public ushort C { get; set; }

        public BrgFace()
        {
            MaterialIndex = -1;
        }

        public BrgFace(ushort a, ushort b, ushort c)
            : this()
        {
            A = a;
            B = b;
            C = c;
        }

        public ushort this[int index]
        {
            get
            {
                return index switch
                {
                    0 => A,
                    1 => B,
                    2 => C,
                    _ => throw new ArgumentOutOfRangeException(nameof(index), index, "Index must be 0, 1, or 2.")
                };
            }
            set
            {
                switch (index)
                {
                    case 0: A = value; break;
                    case 1: B = value; break;
                    case 2: C = value; break;
                    default: throw new ArgumentOutOfRangeException(nameof(index), index, "Index must be 0, 1, or 2.");
                };
            }
        }
    }
}
