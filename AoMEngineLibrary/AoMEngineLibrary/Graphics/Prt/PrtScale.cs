namespace AoMEngineLibrary.Graphics.Prt
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class PrtScale
    {
        public bool LoopingCycle { get; set; }
        public Int32 NumStages { get; set; }
        public float Scale { get; set; }
        public float ScaleVar { get; set; }
        public float XScale { get; set; }
        public float XScaleVar { get; set; }
        public float YScale { get; set; }
        public float YScaleVar { get; set; }
        public float ZScale { get; set; }
        public float ZScaleVar { get; set; }
        public float CycleTime { get; set; }
        public float CycleTimeVar { get; set; }

        [XmlArrayItem("ScaleStage")]
        public List<PrtScaleStage> ScaleStages
        {
            get;
            set;
        }

        private PrtScale()
        {
            ScaleStages = new List<PrtScaleStage>();
        }
        public PrtScale(PrtBinaryReader reader)
        {
            this.LoopingCycle = reader.ReadBoolean();
            reader.ReadBytes(3);

            this.NumStages = reader.ReadInt32();
            this.Scale = reader.ReadSingle();
            this.ScaleVar = reader.ReadSingle();
            this.XScale = reader.ReadSingle();
            this.XScaleVar = reader.ReadSingle();
            this.YScale = reader.ReadSingle();
            this.YScaleVar = reader.ReadSingle();
            this.ZScale = reader.ReadSingle();
            this.ZScaleVar = reader.ReadSingle();
            this.CycleTime = reader.ReadSingle();
            this.CycleTimeVar = reader.ReadSingle();

            this.ScaleStages = new List<PrtScaleStage>(this.NumStages);
        }

        public void Write(PrtBinaryWriter writer)
        {
            writer.Write(this.LoopingCycle);
            writer.Write(new byte[3]);

            this.NumStages = this.ScaleStages.Count;
            writer.Write(this.NumStages);
            writer.Write(this.Scale);
            writer.Write(this.ScaleVar);
            writer.Write(this.XScale);
            writer.Write(this.XScaleVar);
            writer.Write(this.YScale);
            writer.Write(this.YScaleVar);
            writer.Write(this.ZScale);
            writer.Write(this.ZScaleVar);
            writer.Write(this.CycleTime);
            writer.Write(this.CycleTimeVar);
        }
    }
}
