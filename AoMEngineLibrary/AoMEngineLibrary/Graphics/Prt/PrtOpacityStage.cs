namespace AoMEngineLibrary.Graphics.Prt
{
    using System;

    public class PrtOpacityStage
    {
        public float Opacity { get; set; }
        public float OpacityVar { get; set; }
        public float Hold { get; set; }
        public float Fade { get; set; }

        private PrtOpacityStage()
        {

        }
        public PrtOpacityStage(PrtBinaryReader reader)
        {
            this.Opacity = reader.ReadSingle();
            this.OpacityVar = reader.ReadSingle();
            this.Hold = reader.ReadSingle();
            this.Fade = reader.ReadSingle();
        }

        public void Write(PrtBinaryWriter writer)
        {
            writer.Write(this.Opacity);
            writer.Write(this.OpacityVar);
            writer.Write(this.Hold);
            writer.Write(this.Fade);
        }
    }
}
