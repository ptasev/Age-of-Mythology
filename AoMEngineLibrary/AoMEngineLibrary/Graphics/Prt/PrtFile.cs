namespace AoMEngineLibrary.Graphics.Prt
{
    using MiscUtil.Conversion;
    using System;
    using System.IO;
    using System.Xml.Serialization;
    using XmlCommentSerialization;

    public class PrtFile
    {
        [XmlAttribute]
        public Int32 Version
        {
            get;
            set;
        }
        public PrtEmitter Emitter
        {
            get;
            set;
        }
        public PrtShape Shape
        {
            get;
            set;
        }
        public PrtAppearance Appearance
        {
            get;
            set;
        }
        public PrtOpacity Opacity
        {
            get;
            set;
        }
        public PrtScale Scale
        {
            get;
            set;
        }
        public PrtColor Color
        {
            get;
            set;
        }
        public PrtForces Forces
        {
            get;
            set;
        }
        public PrtCollision Collision
        {
            get;
            set;
        }
        public string BrgFileName
        {
            get;
            set;
        }

        public PrtFile()
        {
            Version = 12;
        }
        public PrtFile(Stream stream)
        {
            using (PrtBinaryReader reader = new PrtBinaryReader(new LittleEndianBitConverter(), stream))
            {
                this.Version = reader.ReadInt32();

                this.Emitter = new PrtEmitter(reader);
                this.Shape = new PrtShape(reader);
                this.Appearance = new PrtAppearance(reader);
                this.Opacity = new PrtOpacity(reader);
                this.Scale = new PrtScale(reader);
                this.Color = new PrtColor(reader);
                this.Forces = new PrtForces(reader);
                this.Collision = new PrtCollision(reader);

                this.BrgFileName = reader.ReadString();

                for (int i = 0; i < this.Color.NumPaletteColors; i++)
                {
                    this.Color.PaletteColors.Add(reader.ReadTexel());
                }

                for (int i = 0; i < this.Appearance.NumFiles; i++)
                {
                    this.Appearance.AppearanceWeights.Add(reader.ReadSingle());
                }
                for (int i = 0; i < this.Appearance.NumFiles; i++)
                {
                    this.Appearance.AppearanceFiles.Add(reader.ReadString());
                }

                for (int i = 0; i < this.Opacity.NumStages; i++)
                {
                    this.Opacity.OpacityStages.Add(new PrtOpacityStage(reader));
                }

                for (int i = 0; i < this.Scale.NumStages; i++)
                {
                    this.Scale.ScaleStages.Add(new PrtScaleStage(reader));
                }

                for (int i = 0; i < this.Color.NumStages; i++)
                {
                    this.Color.ColorStages.Add(new PrtColorStage(reader));
                }

                for (int i = 0; i < this.Collision.NumTypes; i++)
                {
                    this.Collision.CollisionTypes.Add(new PrtCollisionType(reader));
                }
            }
        }


        public void SerializeAsXml(Stream stream)
        {
            using (XmlCommentWriter writer = new XmlCommentWriter(stream, new System.Xml.XmlWriterSettings() { Indent = true}))
            {
                writer.Alphabetize = true;
                writer.Metadata = false;
                writer.Repeat = true;
                XmlSerializer serializer = new XmlSerializer(typeof(PrtFile));
                serializer.Serialize(writer, this);
            }
            //XmlSerializer serializer = new XmlSerializer(typeof(PrtFile));
            //using (TextWriter writer = new StreamWriter(stream))
            //{
            //    serializer.Serialize(writer, this);
            //}
        }

        public static PrtFile DeserializeAsXml(Stream stream)
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(PrtFile));
            using (TextReader reader = new StreamReader(stream))
            {
                return deserializer.Deserialize(reader) as PrtFile;
            }
        }

        public void Write(Stream stream)
        {
            using (PrtBinaryWriter writer = new PrtBinaryWriter(new LittleEndianBitConverter(), stream))
            {
                writer.Write(this.Version);

                this.Emitter.Write(writer);
                this.Shape.Write(writer);
                this.Appearance.Write(writer);
                this.Opacity.Write(writer);
                this.Scale.Write(writer);
                this.Color.Write(writer);
                this.Forces.Write(writer);
                this.Collision.Write(writer);

                writer.Write(this.BrgFileName);

                for (int i = 0; i < this.Color.NumPaletteColors; i++)
                {
                    writer.WriteTexel(this.Color.PaletteColors[i]);
                }

                for (int i = 0; i < this.Appearance.NumFiles; i++)
                {
                    writer.Write(this.Appearance.AppearanceWeights[i]);
                }
                for (int i = 0; i < this.Appearance.NumFiles; i++)
                {
                    writer.Write(this.Appearance.AppearanceFiles[i]);
                }

                for (int i = 0; i < this.Opacity.NumStages; i++)
                {
                    this.Opacity.OpacityStages[i].Write(writer);
                }

                for (int i = 0; i < this.Scale.NumStages; i++)
                {
                    this.Scale.ScaleStages[i].Write(writer);
                }

                for (int i = 0; i < this.Color.NumStages; i++)
                {
                    this.Color.ColorStages[i].Write(writer);
                }

                for (int i = 0; i < this.Collision.NumTypes; i++)
                {
                    this.Collision.CollisionTypes[i].Write(writer);
                }
            }
        }
    }
}
