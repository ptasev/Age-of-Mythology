namespace AoMEngineLibrary.Graphics.Prt
{
    using System;
    using System.Collections.Generic;
    using System.Numerics;
    using System.Xml.Serialization;
    using XmlCommentSerialization;

    public class PrtAppearance : XmlAnnotate
    {
        public bool OrientByMotion { get; set; }
        public Int32 NumFiles { get; set; }
        public Int32 NumFrames { get; set; }
        public Int32 FrameWidth { get; set; }
        public Int32 FrameHeight { get; set; }
        [XmlComment]
        public PrtMaterialType MaterialType { get; set; }
        public Vector4 Emissive { get; set; }
        public Vector4 Specular { get; set; }
        public float SpecularExponent { get; set; }
        public float FramesPerSecond { get; set; }
        public float AnimationRate { get; set; }
        public float AnimationRateVar { get; set; }

        [XmlArrayItem("Weight")]
        public List<float> AppearanceWeights
        {
            get;
            set;
        }
        [XmlArrayItem("File")]
        public List<string> AppearanceFiles
        {
            get;
            set;
        }

        public PrtAppearance()
        {
            AppearanceWeights = new List<float>();
            AppearanceFiles = new List<string>();
        }
        public PrtAppearance(PrtBinaryReader reader)
        {
            this.OrientByMotion = reader.ReadBoolean();
            reader.ReadBytes(3);

            this.NumFiles = reader.ReadInt32();
            this.NumFrames = reader.ReadInt32();
            this.FrameWidth = reader.ReadInt32();
            this.FrameHeight = reader.ReadInt32();
            this.MaterialType = (PrtMaterialType)reader.ReadInt32();
            this.Emissive = reader.ReadTexel();
            this.Specular = reader.ReadTexel();
            this.SpecularExponent = reader.ReadSingle();
            this.FramesPerSecond = reader.ReadSingle();
            this.AnimationRate = reader.ReadSingle();
            this.AnimationRateVar = reader.ReadSingle();

            this.AppearanceWeights = new List<float>(this.NumFiles);
            this.AppearanceFiles = new List<string>(this.NumFiles);
        }

        public void Write(PrtBinaryWriter writer)
        {
            writer.Write(this.OrientByMotion);
            writer.Write(new byte[3]);

            this.NumFiles = this.AppearanceFiles.Count;
            writer.Write(this.NumFiles);
            writer.Write(this.NumFrames);
            writer.Write(this.FrameWidth);
            writer.Write(this.FrameHeight);
            writer.Write((Int32)this.MaterialType);
            writer.WriteTexel(this.Emissive);
            writer.WriteTexel(this.Specular);
            writer.Write(this.SpecularExponent);
            writer.Write(this.FramesPerSecond);
            writer.Write(this.AnimationRate);
            writer.Write(this.AnimationRateVar);
        }
    }
}
