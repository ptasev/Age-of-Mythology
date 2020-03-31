namespace AoMEngineLibrary.Graphics.Brg
{
    using Newtonsoft.Json;
    using System;

    public class BrgHeader
    {
        public string Magic { get; set; }
        public int Unknown01 { get; set; }
        public int NumMaterials { get; set; }
        public int Unknown02 { get; set; }
        public int NumMeshes { get; set; }
        public int Reserved { get; set; }
        public int Unknown03 { get; set; }

        public BrgHeader()
        {
            this.Magic = "BANG";
            this.Unknown03 = 1999922179;
        }
        public BrgHeader(BrgBinaryReader reader)
        {
            this.Magic = reader.ReadString(4);
            this.Unknown01 = reader.ReadInt32();
            this.NumMaterials = reader.ReadInt32();
            this.Unknown02 = reader.ReadInt32();
            this.NumMeshes = reader.ReadInt32();
            this.Reserved = reader.ReadInt32();
            this.Unknown03 = reader.ReadInt32();
        }

        public void Write(BrgBinaryWriter writer)
        {
            writer.Write(1196310850); // magic "BANG"
            writer.Write(this.Unknown01);
            writer.Write(this.NumMaterials);
            writer.Write(this.Unknown02);
            writer.Write(this.NumMeshes);
            writer.Write(this.Reserved);
            writer.Write(1999922179);
        }
    }
}
