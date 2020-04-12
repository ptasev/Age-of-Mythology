namespace AoMEngineLibrary.Graphics.Prt
{
    using System;
    using System.Numerics;

    public class PrtColorStage
    {
        public bool UsePalette { get; set; }
        public byte Unk1 { get; set; }
        public byte Unk2 { get; set; }
        public byte Unk3 { get; set; }
        public Vector4 Color { get; set; }
        public float Hold { get; set; }
        public float Fade { get; set; }

        private PrtColorStage()
        {

        }
        public PrtColorStage(PrtBinaryReader reader)
        {
            this.UsePalette = reader.ReadBoolean();

            // There seems to be data here sometimes
            // Not sure what this is (see sfx_e_inferno_smoke_circle.prt)
            this.Unk1 = reader.ReadByte();
            this.Unk2 = reader.ReadByte();
            this.Unk3 = reader.ReadByte();

            this.Color = reader.ReadTexel();
            this.Hold = reader.ReadSingle();
            this.Fade = reader.ReadSingle();
        }

        public void Write(PrtBinaryWriter writer)
        {
            writer.Write(this.UsePalette);

            writer.Write(this.Unk1);
            writer.Write(this.Unk2);
            writer.Write(this.Unk3);

            writer.WriteTexel(this.Color);
            writer.Write(this.Hold);
            writer.Write(this.Fade);
        }
    }
}
