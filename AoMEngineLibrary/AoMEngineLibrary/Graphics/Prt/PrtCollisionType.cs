namespace AoMEngineLibrary.Graphics.Prt
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class PrtCollisionType
    {
        bool SpawnSystem { get; set; }
        bool CollideTerrain { get; set; }
        bool CollideWater { get; set; }
        bool CollideUnits { get; set; }
        Int32 Result { get; set; }
        Int32 NumFileNames { get; set; }
        float LingerTime { get; set; }
        float LingerTimeVar { get; set; }
        float FadeTime { get; set; }
        float FadeTimeVar { get; set; }
        float EnergyLoss { get; set; }
        float EnergyLossVar { get; set; }
        public string Name { get; set; }
        [XmlArrayItem("FileName")]
        public List<string> FileNames { get; set; }

        private PrtCollisionType()
        {
            FileNames = new List<string>();
        }
        public PrtCollisionType(PrtBinaryReader reader)
        {
            this.SpawnSystem = reader.ReadBoolean();
            this.CollideTerrain = reader.ReadBoolean();
            this.CollideWater = reader.ReadBoolean();
            this.CollideUnits = reader.ReadBoolean();

            this.Result = reader.ReadInt32();
            this.NumFileNames = reader.ReadInt32();
            this.LingerTime = reader.ReadSingle();
            this.LingerTimeVar = reader.ReadSingle();
            this.FadeTime = reader.ReadSingle();
            this.FadeTimeVar = reader.ReadSingle();
            this.EnergyLoss = reader.ReadSingle();
            this.EnergyLossVar = reader.ReadSingle();

            this.Name = reader.ReadString();
            this.FileNames = new List<string>(this.NumFileNames);
            for (int i = 0; i < this.NumFileNames; i++)
            {
                this.FileNames.Add(reader.ReadString());
            }
        }

        internal void Write(PrtBinaryWriter writer)
        {
            writer.Write(this.SpawnSystem);
            writer.Write(this.CollideTerrain);
            writer.Write(this.CollideWater);
            writer.Write(this.CollideUnits);

            this.NumFileNames = this.FileNames.Count;
            writer.Write(this.Result);
            writer.Write(this.NumFileNames);
            writer.Write(this.LingerTime);
            writer.Write(this.LingerTimeVar);
            writer.Write(this.FadeTime);
            writer.Write(this.FadeTimeVar);
            writer.Write(this.EnergyLoss);
            writer.Write(this.EnergyLossVar);

            writer.Write(this.Name);
            for (int i = 0; i < this.NumFileNames; i++)
            {
                writer.Write(this.FileNames[i]);
            }
        }
    }
}
