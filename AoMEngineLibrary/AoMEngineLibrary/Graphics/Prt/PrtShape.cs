namespace AoMEngineLibrary.Graphics.Prt
{
    using System;

    public class PrtShape
    {
        public bool StartFull { get; set; }
        public bool EmitAwayFromBias { get; set; }
        public bool UseSpreader { get; set; }
        public Int32 ShapeType { get; set; }
        public float OuterXRadius { get; set; }
        public float InnerXRadius { get; set; }
        public float OuterYRadius { get; set; }
        public float InnerYRadius { get; set; }
        public float OuterZRadius { get; set; }
        public float InnerZRadius { get; set; }
        public float CenterHeight { get; set; }
        public float OffAxis { get; set; }
        public float OffAxisSpread { get; set; }
        public float OffPlane { get; set; }
        public float OffPlaneSpread { get; set; }
        public float BiasPointHeight { get; set; }

        private PrtShape()
        {

        }
        public PrtShape(PrtBinaryReader reader)
        {
            this.StartFull = reader.ReadBoolean();
            this.EmitAwayFromBias = reader.ReadBoolean();
            this.UseSpreader = reader.ReadBoolean();
            reader.ReadByte();

            this.ShapeType = reader.ReadInt32();
            this.OuterXRadius = reader.ReadSingle();
            this.InnerXRadius = reader.ReadSingle();
            this.OuterYRadius = reader.ReadSingle();
            this.InnerYRadius = reader.ReadSingle();
            this.OuterZRadius = reader.ReadSingle();
            this.InnerZRadius = reader.ReadSingle();
            this.CenterHeight = reader.ReadSingle();
            this.OffAxis = reader.ReadSingle();
            this.OffAxisSpread = reader.ReadSingle();
            this.OffPlane = reader.ReadSingle();
            this.OffPlaneSpread = reader.ReadSingle();
            this.BiasPointHeight = reader.ReadSingle();
        }

        public void Write(PrtBinaryWriter writer)
        {
            writer.Write(this.StartFull);
            writer.Write(this.EmitAwayFromBias);
            writer.Write(this.UseSpreader);
            writer.Write((byte)0);

            writer.Write(this.ShapeType);
            writer.Write(this.OuterXRadius);
            writer.Write(this.InnerXRadius);
            writer.Write(this.OuterYRadius);
            writer.Write(this.InnerYRadius);
            writer.Write(this.OuterZRadius);
            writer.Write(this.InnerZRadius);
            writer.Write(this.CenterHeight);
            writer.Write(this.OffAxis);
            writer.Write(this.OffAxisSpread);
            writer.Write(this.OffPlane);
            writer.Write(this.OffPlaneSpread);
            writer.Write(this.BiasPointHeight);
        }
    }
}
