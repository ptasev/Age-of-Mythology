namespace AoMEngineLibrary.Graphics.Prt
{
    using System;

    public class PrtScaleStage
    {
        public float Scale { get; set; }
        public float ScaleVar { get; set; }
        public float Hold { get; set; }
        public float Fade { get; set; }

        private PrtScaleStage()
        {

        }
        public PrtScaleStage(PrtBinaryReader reader)
        {
            this.Scale = reader.ReadSingle();
            this.ScaleVar = reader.ReadSingle();
            this.Hold = reader.ReadSingle();
            this.Fade = reader.ReadSingle();
        }

        public void Write(PrtBinaryWriter writer)
        {
            writer.Write(this.Scale);
            writer.Write(this.ScaleVar);
            writer.Write(this.Hold);
            writer.Write(this.Fade);
        }
    }
}
