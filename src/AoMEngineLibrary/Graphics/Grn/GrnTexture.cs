using AoMEngineLibrary.Graphics.Grn.Nodes;
using System;
using System.IO;

namespace AoMEngineLibrary.Graphics.Grn
{
    public class GrnTexture : IGrnObject, IEquatable<GrnTexture>
    {
        public Int32 DataExtensionIndex { get; set; }

        public GrnFile ParentFile { get; set; }
        public string Name => ParentFile.GetDataExtensionObjectName(DataExtensionIndex);
        public string FileName => ParentFile.GetDataExtensionFileName(DataExtensionIndex);

        public int Width { get; set; }
        public int Height { get; set; }

        public GrnTexture(GrnFile parentFile)
        {
            ParentFile = parentFile;
            DataExtensionIndex = 0;
        }

        internal void Read(GrnNode textureMap)
        {
            // -- Each TextureMap has width height, depth?, and dataExtRef
            GrnTextureMapImageNode? mapImage =
                textureMap.FindNode<GrnTextureMapImageNode>(
                GrnNodeType.TextureMapImage);
            if (mapImage != null)
            {
                this.Width = mapImage.Width;
                this.Height = mapImage.Height;
            }

            GrnDataExtensionReferenceNode? dataRef = textureMap.FindNode<GrnDataExtensionReferenceNode>(
                GrnNodeType.DataExtensionReference);
            if (dataRef == null) throw new InvalidDataException("Texture map has no data extension reference node.");
            this.DataExtensionIndex = dataRef.DataExtensionIndex - 1;
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

        public bool Equals(GrnTexture? tex)
        {
            if (tex == null)
            {
                return false;
            }

            return Name == tex.Name && FileName == tex.FileName;
        }

        public override bool Equals(object? obj) => Equals(obj as GrnTexture);

        public override int GetHashCode() => HashCode.Combine(Name, FileName);
    }
}
