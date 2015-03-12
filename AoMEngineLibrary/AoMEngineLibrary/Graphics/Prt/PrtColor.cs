namespace AoMEngineLibrary.Graphics.Prt
{
    using System;
    using System.Collections.Generic;

    public class PrtColor
    {
        public bool UsePalette { get; set; }
        public bool LoopingCycle { get; set; }
        public Int32 NumPaletteColors { get; set; }
        public Int32 NumStages { get; set; }
        public float CycleTime { get; set; }
        public float CycleTimeVar { get; set; }
        public float WorldLightingInfluence { get; set; }
        public VertexColor Color { get; set; }

        public List<VertexColor> PaletteColors
        {
            get;
            set;
        }
        public List<PrtColorStage> ColorStages
        {
            get;
            set;
        }

        private PrtColor()
        {
            PaletteColors = new List<VertexColor>();
            ColorStages = new List<PrtColorStage>();
        }
        public PrtColor(PrtBinaryReader reader)
        {
            this.UsePalette = reader.ReadBoolean();
            this.LoopingCycle = reader.ReadBoolean();
            reader.ReadBytes(2);

            this.NumPaletteColors = reader.ReadInt32();
            this.NumStages = reader.ReadInt32();
            this.CycleTime = reader.ReadSingle();
            this.CycleTimeVar = reader.ReadSingle();
            this.WorldLightingInfluence = reader.ReadSingle();
            this.Color = reader.ReadVertexColor();

            this.PaletteColors = new List<VertexColor>(this.NumPaletteColors);
            this.ColorStages = new List<PrtColorStage>(this.NumStages);
        }

        public void Write(PrtBinaryWriter writer)
        {
            writer.Write(this.UsePalette);
            writer.Write(this.LoopingCycle);
            writer.Write(new byte[2]);

            this.NumPaletteColors = this.PaletteColors.Count;
            this.NumStages = this.ColorStages.Count;
            writer.Write(this.NumPaletteColors);
            writer.Write(this.NumStages);
            writer.Write(this.CycleTime);
            writer.Write(this.CycleTimeVar);
            writer.Write(this.WorldLightingInfluence);
            writer.WriteVertexColor(this.Color);
        }
    }
}
