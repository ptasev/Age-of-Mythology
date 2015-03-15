namespace AoMEngineLibrary.Graphics.Prt
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization; 

    public class PrtOpacity
    {
        public bool LoopingCycle { get; set; }
        public Int32 NumStages { get; set; }
        public float Opacity { get; set; }
        public float OpacityVar { get; set; }
        public float CycleTime { get; set; }
        public float CycleTimeVar { get; set; }

        [XmlArrayItem("OpacityStage")]
        public List<PrtOpacityStage> OpacityStages
        {
            get;
            set;
        }

        private PrtOpacity()
        {
            OpacityStages = new List<PrtOpacityStage>();
        }
        public PrtOpacity(PrtBinaryReader reader)
        {
            this.LoopingCycle = reader.ReadBoolean();
            reader.ReadBytes(3);

            this.NumStages = reader.ReadInt32();
            this.Opacity = reader.ReadSingle();
            this.OpacityVar = reader.ReadSingle();
            this.CycleTime = reader.ReadSingle();
            this.CycleTimeVar = reader.ReadSingle();

            this.OpacityStages = new List<PrtOpacityStage>(this.NumStages);
        }

        public void Write(PrtBinaryWriter writer)
        {
            writer.Write(this.LoopingCycle);
            writer.Write(new byte[3]);

            this.NumStages = this.OpacityStages.Count;
            writer.Write(this.NumStages);
            writer.Write(this.Opacity);
            writer.Write(this.OpacityVar);
            writer.Write(this.CycleTime);
            writer.Write(this.CycleTimeVar);
        }
    }
}
