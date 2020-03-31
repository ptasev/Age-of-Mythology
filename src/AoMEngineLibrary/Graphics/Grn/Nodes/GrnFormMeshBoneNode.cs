namespace AoMEngineLibrary.Graphics.Grn.Nodes
{
    using AoMEngineLibrary.Graphics.Model;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Numerics;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    public class GrnFormMeshBoneNode : GrnNode
    {
        public Int32 BoneIndex
        {
            get;
            set;
        }

        public Single Unknown
        {
            get;
            set;
        }

        public Vector3 OBBMin
        {
            get;
            set;
        }

        public Vector3 OBBMax
        {
            get;
            set;
        }

        public GrnFormMeshBoneNode(GrnNode parentNode)
            : base(parentNode, GrnNodeType.FormMeshBone)
        {
        }

        public override void ReadData(GrnBinaryReader reader, int directoryOffset)
        {
            reader.BaseStream.Seek((int)(this.Offset + directoryOffset), SeekOrigin.Begin);
            this.BoneIndex = reader.ReadInt32();
            this.Unknown = reader.ReadSingle();
            this.OBBMin = reader.ReadVector3D();
            this.OBBMax = reader.ReadVector3D();
        }

        public override void WriteData(GrnBinaryWriter writer)
        {
            writer.Write(this.BoneIndex);
            writer.Write(this.Unknown);
            writer.Write(this.OBBMin);
            writer.Write(this.OBBMax);
        }

        public override void CreateFolderFile(string folder)
        {
            XDocument doc = new XDocument();
            XElement root = new XElement("fMeshBone");
            doc.Add(root);

            root.Add(new XElement("boneIndex", this.BoneIndex));
            root.Add(new XElement("unknown", this.Unknown));
            root.Add(new XElement("obbMin", this.OBBMin));
            root.Add(new XElement("obbMax", this.OBBMax));

            string fileName = System.IO.Path.Combine(folder, "fMeshBone.xml");
            using (FileStream stream = System.IO.File.Create(fileName))
            {
                doc.Save(stream);
            }
        }

        public override int GetWriteDataLength()
        {
            return 32;
        }
    }
}
