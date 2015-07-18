namespace AoMEngineLibrary.Graphics.Grn.Nodes
{
    using AoMEngineLibrary.Graphics.Model;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    public class GrnAnimationTransformTrackKeysNode : GrnNode
    {
        public Int32 TransformChannelIndex { get; set; }
        public Int32[] Unknown { get; set; }
        public List<float> PositionKeys { get; set; }
        public List<float> RotationKeys { get; set; }
        public List<float> ScaleKeys { get; set; }
        public Int32[] Unknown2 { get; set; }

        public List<Vector3D> Positions { get; set; }
        public List<Quaternion> Rotations { get; set; }
        public List<Matrix3x3> Scales { get; set; }

        public GrnAnimationTransformTrackKeysNode(GrnNode parentNode)
            : base(parentNode, GrnNodeType.AnimationTransformTrackKeys)
        {
            this.Unknown = new Int32[5] { 0, 1, 2, 2, 1 };
            this.PositionKeys = new List<float>();
            this.RotationKeys = new List<float>();
            this.ScaleKeys = new List<float>();
            this.Unknown2 = new Int32[4] { 0, 1, 2, 0 };
            this.Positions = new List<Vector3D>();
            this.Rotations = new List<Quaternion>();
            this.Scales = new List<Matrix3x3>();
        }

        public override void ReadData(GrnBinaryReader reader, int directoryOffset)
        {
            reader.Seek((int)(this.Offset + directoryOffset), SeekOrigin.Begin);
            this.TransformChannelIndex = reader.ReadInt32();

            // read 5 unknowns
            for (int i = 0; i < 5; ++i)
            {
                this.Unknown[i] = reader.ReadInt32();
            }

            int numPositions = reader.ReadInt32();
            int numRotations = reader.ReadInt32();
            int numTransforms = reader.ReadInt32();

            // read 4 unknowns
            for (int i = 0; i < 4; ++i)
            {
                this.Unknown2[i] = reader.ReadInt32();
            }

            for (int i = 0; i < numPositions; ++i)
            {
                this.PositionKeys.Add(reader.ReadSingle());
            }
            for (int i = 0; i < numRotations; ++i)
            {
                this.RotationKeys.Add(reader.ReadSingle());
            }
            for (int i = 0; i < numTransforms; ++i)
            {
                this.ScaleKeys.Add(reader.ReadSingle());
            }

            for (int i = 0; i < numPositions; ++i)
            {
                this.Positions.Add(reader.ReadVector3D());
            }
            for (int i = 0; i < numRotations; ++i)
            {
                this.Rotations.Add(reader.ReadQuaternion());
            }
            for (int i = 0; i < numTransforms; ++i)
            {
                this.Scales.Add(reader.ReadMatrix3x3());
            }
        }

        public override void WriteData(GrnBinaryWriter writer)
        {
            writer.Write(this.TransformChannelIndex);

            for (int i = 0; i < 5; ++i)
            {
                writer.Write(this.Unknown[i]);
            }

            writer.Write(this.Positions.Count);
            writer.Write(this.Rotations.Count);
            writer.Write(this.Scales.Count);

            for (int i = 0; i < 4; ++i)
            {
                writer.Write(this.Unknown2[i]);
            }

            for (int i = 0; i < this.PositionKeys.Count; ++i)
            {
                writer.Write(this.PositionKeys[i]);
            }
            for (int i = 0; i < this.RotationKeys.Count; ++i)
            {
                writer.Write(this.RotationKeys[i]);
            }
            for (int i = 0; i < this.ScaleKeys.Count; ++i)
            {
                writer.Write(this.ScaleKeys[i]);
            }

            for (int i = 0; i < this.Positions.Count; ++i)
            {
                writer.Write(this.Positions[i]);
            }
            for (int i = 0; i < this.Rotations.Count; ++i)
            {
                writer.Write(this.Rotations[i]);
            }
            for (int i = 0; i < this.Scales.Count; ++i)
            {
                writer.Write(this.Scales[i]);
            }
        }

        public override void CreateFolderFile(string folder)
        {
            XDocument doc = new XDocument();
            XElement root = new XElement("attKeys", 
                new XAttribute("TransformChannelIndex", this.TransformChannelIndex));
            doc.Add(root);

            root.Add(new XElement("Unknown", this.Unknown[0]));
            root.Add(new XElement("Unknown", this.Unknown[1]));
            root.Add(new XElement("Unknown", this.Unknown[2]));
            root.Add(new XElement("Unknown", this.Unknown[3]));
            root.Add(new XElement("Unknown", this.Unknown[4]));
            root.Add(new XElement("Unknown", this.Unknown2[0]));
            root.Add(new XElement("Unknown", this.Unknown2[1]));
            root.Add(new XElement("Unknown", this.Unknown2[2]));
            root.Add(new XElement("Unknown", this.Unknown2[3]));

            XElement positions = new XElement("positions", new XAttribute("count", this.Positions.Count));
            for (int i = 0; i < this.Positions.Count; ++i)
            {
                positions.Add(new XElement("pos", new XAttribute("time", 
                    this.PositionKeys[i]), this.Positions[i]));
            }
            root.Add(positions);

            XElement rotations = new XElement("rotations", new XAttribute("count", this.Rotations.Count));
            for (int i = 0; i < this.Rotations.Count; ++i)
            {
                rotations.Add(new XElement("rot", new XAttribute("time",
                    this.RotationKeys[i]), this.Rotations[i]));
            }
            root.Add(rotations);

            XElement scales = new XElement("scales", new XAttribute("count", this.Scales.Count));
            for (int i = 0; i < this.Scales.Count; ++i)
            {
                scales.Add(new XElement("scale", new XAttribute("time",
                    this.ScaleKeys[i]), this.Scales[i]));
            }
            root.Add(scales);

            string fileName = System.IO.Path.Combine(folder, "attKeys.xml");
            using (FileStream stream = System.IO.File.Create(fileName))
            {
                doc.Save(stream);
            }
        }

        public override int GetWriteDataLength()
        {
            int length = 52; // 4 * 13

            length += this.PositionKeys.Count * 4;
            length += this.RotationKeys.Count * 4;
            length += this.ScaleKeys.Count * 4;

            length += this.Positions.Count * 12;
            length += this.Rotations.Count * 16;
            length += this.Scales.Count * 36;

            return length;
        }
    }
}
