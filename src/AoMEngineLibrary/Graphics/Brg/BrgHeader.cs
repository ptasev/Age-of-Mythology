namespace AoMEngineLibrary.Graphics.Brg
{
    public class BrgHeader
    {
        public const uint BangMagic = 1196310850; // BANG chars

        // all of these are unsigned in game code, but use int for some for ease of use in C#
        public uint Magic { get; set; }
        public uint Reserved1 { get; set; }
        public int NumMaterials { get; set; }
        public uint Reserved2 { get; set; }
        public int NumMeshes { get; set; }
        public uint Reserved3 { get; set; }
        public byte Revision { get; set; }
        public byte[] Padding { get; }

        public BrgHeader()
        {
            Magic = BangMagic;
            Revision = 3;
            Padding = new byte[] { 0x64, 0x34, 0x77 };
        }
        public BrgHeader(BrgBinaryReader reader)
        {
            Magic = reader.ReadUInt32();
            Reserved1 = reader.ReadUInt32();
            NumMaterials = reader.ReadInt32();
            Reserved2 = reader.ReadUInt32();
            NumMeshes = reader.ReadInt32();
            Reserved3 = reader.ReadUInt32();
            Revision = reader.ReadByte();
            Padding = reader.ReadBytes(3);
        }

        public void Write(BrgBinaryWriter writer)
        {
            writer.Write(Magic);
            writer.Write(Reserved1);
            writer.Write(NumMaterials);
            writer.Write(Reserved2);
            writer.Write(NumMeshes);
            writer.Write(Reserved3);
            writer.Write(Revision);
            writer.Write(Padding);
        }
    }
}
