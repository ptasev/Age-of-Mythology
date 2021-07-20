using System.Numerics;

namespace AoMEngineLibrary.Graphics.Brg
{
    public class BrgDummy
    {
        public BrgDummyType Type { get; set; }

        public string Name => Type.GetInfo().Name;

        /// <summary>
        /// The Y Axis pointing up in a LH coordinate system. (Green axis in AoM editor)
        /// </summary>
        public Vector3 Up { get; set; } = new Vector3(0, 1, 0);

        /// <summary>
        /// The Z axis pointing forward in a LH coordinate system. (Red axis in AoM editor)
        /// </summary>
        public Vector3 Forward { get; set; } = new Vector3(0, 0, -1);

        /// <summary>
        /// The X axis pointing to the right in a LH coordinate system. (Blue axis in AoM editor)
        /// </summary>
        public Vector3 Right { get; set; } = new Vector3(-1, 0, 0);

        public Vector3 Position { get; set; }

        public Vector3 BoundingBoxMin { get; set; } = new Vector3(-0.25f);

        public Vector3 BoundingBoxMax { get; set; } = new Vector3(0.25f);

        public BrgDummy()
        {
        }
    }
}
