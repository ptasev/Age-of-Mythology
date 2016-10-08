using AoMEngineLibrary.Graphics.Grn.Nodes;
using AoMEngineLibrary.Graphics.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AoMEngineLibrary.Graphics.Grn
{
    public class GrnBone : IGrnObject
    {
        public GrnFile ParentFile { get; set; }
        public string Name
        {
            get
            {
                return this.ParentFile.GetDataExtensionObjectName(this.DataExtensionIndex);
            }
        }
        public Int32 ParentIndex { get; set; }
        public Int32 DataExtensionIndex { get; set; }
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public Matrix4x4 Scale { get; set; }

        public GrnBone()
        {
            this.ParentIndex = -1;
            this.Position = new Vector3(0f, 0f, 0f);
            this.Rotation = new Quaternion(0, 0, 0, 1);
            this.Scale = Matrix4x4.Identity;
        }
        public GrnBone(GrnFile parentFile)
            : this()
        {
            this.ParentFile = parentFile;
        }

        public void Read(GrnBoneNode boneNode)
        {
            this.ParentIndex = boneNode.ParentIndex;
            this.Position = boneNode.Position;
            this.Rotation = boneNode.Rotation;
            this.Scale = boneNode.Scale;
        }
        public void Write(GrnNode boneSecNode)
        {
            GrnBoneNode boneNode = new GrnBoneNode(boneSecNode);
            boneNode.ParentIndex = this.ParentIndex;
            boneNode.Position = this.Position;
            boneNode.Rotation = this.Rotation;
            boneNode.Scale = this.Scale;
            boneSecNode.AppendChild(boneNode);
        }
    }
}
