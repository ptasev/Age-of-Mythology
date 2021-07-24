using AoMEngineLibrary.Graphics.Grn.Nodes;
using System;
using System.IO;

namespace AoMEngineLibrary.Graphics.Grn
{
    public class GrnMaterial : IGrnObject, IEquatable<GrnMaterial>
    {
        public GrnFile ParentFile { get; set; }
        public int DataExtensionIndex { get; set; }
        public int DiffuseTextureIndex { get; set; }

        public string Name
        {
            get
            {
                foreach (var dataExt in ParentFile.DataExtensions[DataExtensionIndex])
                {
                    if (dataExt.Key == "__ObjectName")
                    {
                        return dataExt.Value;
                    }
                }

                return "(unnamed)";
            }
        }
        public GrnTexture DiffuseTexture => ParentFile.Textures[DiffuseTextureIndex];

        public GrnMaterial(GrnFile parentFile)
        {
            ParentFile = parentFile;
            DataExtensionIndex = 0;
            DiffuseTextureIndex = 0;
        }

        internal void Read(GrnNode material)
        {
            // -- Each material has diffuseTex (0, textureIndex(+1), 1), and dataExtRef
            var matDiffuse = 
                material.FindNode<GrnMaterialSimpleDiffuseTextureNode>(
                GrnNodeType.MaterialSimpleDiffuseTexture);
            if (matDiffuse != null)
            {
                DiffuseTextureIndex = matDiffuse.TextureMapIndex - 1;
            }

            var refNode = material.FindNode<GrnDataExtensionReferenceNode>(
                GrnNodeType.DataExtensionReference);
            if (refNode == null) throw new InvalidDataException("Material node has no data extension reference.");
            DataExtensionIndex = refNode.DataExtensionIndex - 1;
        }
        public void Write(GrnNode matSecNode)
        {
            var matNode = new GrnNode(matSecNode, GrnNodeType.Material);
            matSecNode.AppendChild(matNode);
            var matDiffNode = new GrnMaterialSimpleDiffuseTextureNode(matNode);
            matDiffNode.TextureMapIndex = DiffuseTextureIndex + 1;
            matNode.AppendChild(matDiffNode);
            var refNode = new GrnDataExtensionReferenceNode(matNode);
            refNode.DataExtensionIndex = this.DataExtensionIndex + 1;
            matNode.AppendChild(refNode);
        }

        public bool Equals(GrnMaterial? m)
        {
            // If parameter is null return false:
            if (m == null)
            {
                return false;
            }

            var ret = Name == m.Name &&
                DiffuseTextureIndex == m.DiffuseTextureIndex;

            // Return true if the fields match:
            return ret;
        }

        public override bool Equals(object? obj) => Equals(obj as GrnMaterial);

        public override int GetHashCode() => HashCode.Combine(Name, DiffuseTextureIndex);
    }
}
