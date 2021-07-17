using System.Numerics;

namespace AoMEngineLibrary.Graphics.Brg
{
    public class BrgMeshHeader
    {
        public ushort Version { get; set; }
        public BrgMeshFormat Format { get; set; }
        public ushort NumVertices { get; set; }
        public ushort NumFaces { get; set; }
        public BrgMeshInterpolationType InterpolationType { get; set; }
        public BrgMeshAnimType AnimationType { get; set; }
        public ushort UserDataEntryCount { get; set; }
        public Vector3 CenterPosition { get; set; }
        public float CenterRadius { get; set; }
        public Vector3 MassPosition { get; set; }
        public Vector3 HotspotPosition { get; set; }
        public ushort ExtendedHeaderSize { get; set; }
        public BrgMeshFlag Flags { get; set; }
        public Vector3 MinimumExtent { get; set; }
        public Vector3 MaximumExtent { get; set; }

        public BrgMeshHeader()
        {

        }
        public BrgMeshHeader(BrgBinaryReader reader)
        {
            Version = reader.ReadUInt16();
            Format = (BrgMeshFormat)reader.ReadInt16();
            NumVertices = reader.ReadUInt16();
            NumFaces = reader.ReadUInt16();
            InterpolationType = (BrgMeshInterpolationType)reader.ReadByte();
            AnimationType = (BrgMeshAnimType)reader.ReadByte();
            UserDataEntryCount = reader.ReadUInt16();
            CenterPosition = reader.ReadVector3D(false);
            CenterRadius = reader.ReadSingle();
            MassPosition = reader.ReadVector3D(false);
            HotspotPosition = reader.ReadVector3D(false);
            ExtendedHeaderSize = reader.ReadUInt16();
            Flags = (BrgMeshFlag)reader.ReadInt16();
            MinimumExtent = reader.ReadVector3D(false);
            MaximumExtent = reader.ReadVector3D(false);
        }

        public void Write(BrgBinaryWriter writer)
        {
            writer.Write(Version);
            writer.Write((ushort)Format);
            writer.Write(NumVertices);
            writer.Write(NumFaces);
            writer.Write((byte)InterpolationType);
            writer.Write((byte)AnimationType);
            writer.Write(UserDataEntryCount);
            writer.WriteVector3D(CenterPosition, false);
            writer.Write(CenterRadius);//unknown03
            writer.WriteVector3D(MassPosition, false);
            writer.WriteVector3D(HotspotPosition, false);
            writer.Write(ExtendedHeaderSize);
            writer.Write((ushort)Flags);
            writer.WriteVector3D(MinimumExtent, false);
            writer.WriteVector3D(MaximumExtent, false);
        }
    }
}
