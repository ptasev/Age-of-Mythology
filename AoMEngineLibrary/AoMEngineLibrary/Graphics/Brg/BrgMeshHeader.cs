namespace AoMEngineLibrary.Graphics.Brg
{
    using System;

    public class BrgMeshHeader
    {
        public Int16 Version { get; set; }
        public BrgMeshFormat Format { get; set; }
        public Int16 NumVertices { get; set; }
        public Int16 NumFaces { get; set; }
        public byte InterpolationType { get; set; }
        public BrgMeshAnimType AnimationType { get; set; }
        public Int16 UserDataEntryCount { get; set; }
        public Vector3<Single> CenterPosition { get; set; }
        public Single CenterRadius { get; set; }
        public Vector3<Single> MassPosition { get; set; }
        public Vector3<Single> HotspotPosition { get; set; }
        public Int16 ExtendedHeaderSize { get; set; }
        public BrgMeshFlag Flags { get; set; }
        public Vector3<Single> MinimumExtent { get; set; }
        public Vector3<Single> MaximumExtent { get; set; }

        public BrgMeshHeader()
        {

        }
        public BrgMeshHeader(BrgBinaryReader reader)
        {
            this.Version = reader.ReadInt16();
            this.Format = (BrgMeshFormat)reader.ReadInt16();
            this.NumVertices = reader.ReadInt16();
            this.NumFaces = reader.ReadInt16();
            this.InterpolationType = reader.ReadByte();
            this.AnimationType = (BrgMeshAnimType)reader.ReadByte();
            this.UserDataEntryCount = reader.ReadInt16();
            this.CenterPosition = reader.ReadVector3Single(true, false);
            this.CenterRadius = reader.ReadSingle();
            this.MassPosition = reader.ReadVector3Single(true, false);
            this.HotspotPosition = reader.ReadVector3Single(true, false);
            this.ExtendedHeaderSize = reader.ReadInt16();
            this.Flags = (BrgMeshFlag)reader.ReadInt16();
            this.MinimumExtent = reader.ReadVector3Single(true, false);
            this.MaximumExtent = reader.ReadVector3Single(true, false);
        }

        public void Write(BrgBinaryWriter writer)
        {
            writer.Write(this.Version);
            writer.Write((UInt16)this.Format);
            writer.Write(this.NumVertices);
            writer.Write(this.NumFaces);
            writer.Write((uint)this.AnimationType);
            writer.WriteVector3(this.CenterPosition, true);
            writer.Write(this.CenterRadius);//unknown03
            writer.WriteVector3(this.MassPosition, true);
            writer.WriteVector3(this.HotspotPosition, true);
            writer.Write(this.ExtendedHeaderSize);
            writer.Write((UInt16)this.Flags);
            writer.WriteVector3(this.MinimumExtent, true);
            writer.WriteVector3(this.MaximumExtent, true);
        }
    }
}
