namespace AoMEngineLibrary.Graphics.Grn
{
    using AoMEngineLibrary.Graphics.Grn.Nodes;
    using AoMEngineLibrary.Graphics.Model;
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class GrnMaterial : Material
    {
        public GrnFile ParentFile { get; set; }
        public Int32 DataExtensionIndex { get; set; }
        public Int32 DiffuseTextureIndex { get; set; }

        public override string Name
        {
            get
            {
                foreach (KeyValuePair<string, string> dataExt in this.ParentFile.DataExtensions[this.DataExtensionIndex])
                {
                    if (dataExt.Key == "__ObjectName")
                    {
                        return dataExt.Value;
                    }
                }

                return "(unnamed)";
            }
        }
        public GrnTexture DiffuseTexture
        {
            get
            {
                return this.ParentFile.Textures[this.DiffuseTextureIndex];
            }
        }

        public GrnMaterial()
            : base()
        {
            this.DataExtensionIndex = 0;
            this.DiffuseTextureIndex = 0;
        }
        public GrnMaterial(GrnFile parentFile)
            : this()
        {
            this.ParentFile = parentFile;
        }

        internal void Read(GrnNode material)
        {
            // -- Each material has diffuseTex (0, textureIndex(+1), 1), and dataExtRef
            GrnMaterialSimpleDiffuseTextureNode matDiffuse = 
                material.FindNode<GrnMaterialSimpleDiffuseTextureNode>(
                GrnNodeType.MaterialSimpleDiffuseTexture);
            if (matDiffuse != null)
            {
                this.DiffuseTextureIndex = matDiffuse.TextureMapIndex - 1;
            }

            this.DataExtensionIndex = 
                material.FindNode<GrnDataExtensionReferenceNode>(
                GrnNodeType.DataExtensionReference).DataExtensionIndex - 1;
        }
        public void Write(GrnNode matSecNode)
        {
            GrnNode matNode = new GrnNode(matSecNode, GrnNodeType.Material);
            matSecNode.AppendChild(matNode);
            GrnMaterialSimpleDiffuseTextureNode matDiffNode =
                new GrnMaterialSimpleDiffuseTextureNode(matNode);
            matDiffNode.TextureMapIndex = this.DiffuseTextureIndex + 1;
            matNode.AppendChild(matDiffNode);
            GrnDataExtensionReferenceNode refNode = new GrnDataExtensionReferenceNode(matNode);
            refNode.DataExtensionIndex = this.DataExtensionIndex + 1;
            matNode.AppendChild(refNode);
        }
    }
}
