namespace AoMEngineLibrary.Graphics.Grn
{
    using AoMEngineLibrary.Graphics.Grn.Nodes;
    using AoMEngineLibrary.Graphics.Model;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class GrnTexture : Texture, IEquatable<GrnTexture>
    {
        public Int32 DataExtensionIndex { get; set; }

        public GrnFile ParentFile { get; set; }
        public string Name
        {
            get
            {
                return this.ParentFile.GetDataExtensionObjectName(this.DataExtensionIndex);
            }
        }
        public string FileName
        {
            get
            {
                return this.ParentFile.GetDataExtensionFileName(this.DataExtensionIndex);
            }
        }

        public GrnTexture()
            : base()
        {
            this.DataExtensionIndex = 0;
        }
        public GrnTexture(GrnFile parentFile)
            : this()
        {
            this.ParentFile = parentFile;
        }

        internal void Read(GrnNode textureMap)
        {
            // -- Each TextureMap has width height, depth?, and dataExtRef
            GrnTextureMapImageNode mapImage =
                textureMap.FindNode<GrnTextureMapImageNode>(
                GrnNodeType.TextureMapImage);
            if (mapImage != null)
            {
                this.Width = mapImage.Width;
                this.Height = mapImage.Height;
            }

            this.DataExtensionIndex =
                textureMap.FindNode<GrnDataExtensionReferenceNode>(
                GrnNodeType.DataExtensionReference).DataExtensionIndex - 1;
        }
        public void Write(GrnNode texSecNode)
        {
            GrnNode texMapNode = new GrnNode(texSecNode, GrnNodeType.TextureMap);
            texSecNode.AppendChild(texMapNode);
            GrnNode texImSecNode = new GrnNode(texMapNode, GrnNodeType.TextureImageSection);
            texMapNode.AppendChild(texImSecNode);
            GrnTextureMapImageNode texMapImNode = new GrnTextureMapImageNode(texImSecNode);
            texMapImNode.Width = this.Width;
            texMapImNode.Height = this.Height;
            texImSecNode.AppendChild(texMapImNode);
            GrnDataExtensionReferenceNode refNode = new GrnDataExtensionReferenceNode(texMapNode);
            refNode.DataExtensionIndex = this.DataExtensionIndex + 1;
            texMapNode.AppendChild(refNode);
        }

        public bool Equals(GrnTexture tex)
        {
            if ((object)tex == null)
            {
                return false;
            }

            return this.Name == tex.Name &&
                this.FileName == tex.FileName &&
                this.Width == tex.Width &&
                this.Height == tex.Height;
        }
    }
}
