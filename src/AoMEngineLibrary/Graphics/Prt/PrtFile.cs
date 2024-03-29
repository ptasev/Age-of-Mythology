﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Xml.Serialization;
using XmlCommentSerialization;

namespace AoMEngineLibrary.Graphics.Prt
{
    [XmlRoot("ParticleFile", IsNullable = false)]
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
            Emitter = new PrtEmitter();
            Shape = new PrtShape();
            Appearance = new PrtAppearance();
            Opacity = new PrtOpacity();
            Scale = new PrtScale();
            Color = new PrtColor();
            Forces = new PrtForces();
            Collision = new PrtCollision();
            BrgFileName = string.Empty;
        }
        public PrtFile(Stream stream)
        {
            using (PrtBinaryReader reader = new PrtBinaryReader(stream))
            {
                this.Version = reader.ReadInt32();

                this.Emitter = new PrtEmitter(reader, Version);
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

        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(PrtAppearance))]
        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(PrtAppearanceType))]
        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(PrtCollision))]
        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(PrtCollisionType))]
        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(PrtColor))]
        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(PrtColorStage))]
        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(PrtEmitter))]
        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(PrtFile))]
        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(PrtForces))]
        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(PrtMaterialType))]
        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(PrtOpacity))]
        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(PrtOpacityStage))]
        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(PrtScale))]
        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(PrtScaleStage))]
        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(PrtShape))]
        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(PrtShapeType))]
        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(PrtTerrainInteractionType))]
        [UnconditionalSuppressMessage("Trimming",
            "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code",
            Justification = "Annotated with dynamic types above.")]
        public void SerializeAsXml(Stream stream)
        {
            using (var writer = new XmlCommentWriter(stream, new System.Xml.XmlWriterSettings() { Indent = true, CloseOutput = false }))
            {
                writer.Alphabetize = true;
                writer.Metadata = false;
                writer.Repeat = true;
                var serializer = new XmlSerializer(typeof(PrtFile));
                serializer.Serialize(writer, this);
            }
            //XmlSerializer serializer = new XmlSerializer(typeof(PrtFile));
            //using (TextWriter writer = new StreamWriter(stream))
            //{
            //    serializer.Serialize(writer, this);
            //}
        }

        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(PrtAppearance))]
        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(PrtAppearanceType))]
        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(PrtCollision))]
        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(PrtCollisionType))]
        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(PrtColor))]
        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(PrtColorStage))]
        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(PrtEmitter))]
        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(PrtFile))]
        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(PrtForces))]
        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(PrtMaterialType))]
        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(PrtOpacity))]
        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(PrtOpacityStage))]
        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(PrtScale))]
        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(PrtScaleStage))]
        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(PrtShape))]
        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(PrtShapeType))]
        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(PrtTerrainInteractionType))]
        [UnconditionalSuppressMessage("Trimming",
            "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code",
            Justification = "Annotated with dynamic types above.")]
        public static PrtFile DeserializeAsXml(Stream stream)
        {
            var deserializer = new XmlSerializer(typeof(PrtFile));
            using (TextReader reader = new StreamReader(stream, null, true, -1, true))
            {
                return (PrtFile?)deserializer.Deserialize(reader) ??
                       throw new InvalidOperationException("Failed to deserialize particle file.");
            }
        }

        public void Write(Stream stream)
        {
            using (PrtBinaryWriter writer = new PrtBinaryWriter(stream))
            {
                writer.Write(this.Version);

                this.Emitter.Write(writer, Version);
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
