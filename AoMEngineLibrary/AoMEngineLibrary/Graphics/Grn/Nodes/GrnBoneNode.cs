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

    public class GrnBoneNode : GrnNode
    {
        public Int32 ParentIndex
        {
            get;
            set;
        }

        public Vector3D Position
        {
            get;
            set;
        }

        public Quaternion Rotation
        {
            get;
            set;
        }

        public Matrix3x3 Scale
        {
            get;
            set;
        }

        public GrnBoneNode(GrnNode parentNode)
            : base(parentNode, GrnNodeType.Bone)
        {
        }

        public override void ReadData(GrnBinaryReader reader, int directoryOffset)
        {
            reader.Seek((int)(this.Offset + directoryOffset), SeekOrigin.Begin);
            this.ParentIndex = reader.ReadInt32();
            this.Position = reader.ReadVector3D();
            this.Rotation = reader.ReadQuaternion();
            this.Scale = reader.ReadMatrix3x3();
            if (this.GetReadDataLength() != this.GetWriteDataLength()) throw new Exception("bbb");
        }

        public override void WriteData(GrnBinaryWriter writer)
        {
            writer.Write(this.ParentIndex);
            writer.Write(this.Position);
            writer.Write(this.Rotation);
            writer.Write(this.Scale);
        }

        public override void CreateFolderFile(string folder)
        {
            XDocument doc = new XDocument();
            XElement root = new XElement("boneData");
            doc.Add(root);

            root.Add(new XElement("parentIndex", this.ParentIndex));
            root.Add(new XElement("position", this.Position));
            root.Add(new XElement("rotation", this.Rotation));
            root.Add(new XElement("scale", this.Scale));

            string fileName = System.IO.Path.Combine(folder, "boneData.xml");
            using (FileStream stream = System.IO.File.Create(fileName))
            {
                doc.Save(stream);
            }
        }

        public override int GetWriteDataLength()
        {
            return 68; // 4, 12, 16, 36
        }
    }
}
