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
        public Int32 TextureDataExtensionIndex { get; set; }

        public string Name
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
        public string DiffuseMap
        {
            get
            {
                foreach (KeyValuePair<string, string> dataExt in this.ParentFile.DataExtensions[this.TextureDataExtensionIndex])
                {
                    if (dataExt.Key == "__ObjectName")
                    {
                        return dataExt.Value;
                    }
                }

                return "(unnamed)";
            }
        }
        public string DiffuseMapFileName
        {
            get
            {
                foreach (KeyValuePair<string, string> dataExt in this.ParentFile.DataExtensions[this.TextureDataExtensionIndex])
                {
                    if (dataExt.Key == "__FileName")
                    {
                        return dataExt.Value;
                    }
                }

                return "(unnamed)";
            }
        }

        public GrnMaterial()
            : base()
        {
            this.DataExtensionIndex = 0;
            this.TextureDataExtensionIndex = 0;
        }
        public GrnMaterial(GrnFile parentFile)
            : this()
        {
            this.ParentFile = parentFile;
        }

        internal void Read(GrnNode material, List<int> textureDataRefExts)
        {
            // -- Each material has diffuseTex (0, textureIndex(+1), 1), and dataExtRef
            GrnMaterialSimpleDiffuseTextureNode matDiffuse = 
                material.FindNode<GrnMaterialSimpleDiffuseTextureNode>(
                GrnNodeType.MaterialSimpleDiffuseTexture);
            if (matDiffuse != null)
            {
                this.TextureDataExtensionIndex = textureDataRefExts[matDiffuse.TextureMapIndex - 1];
            }

            this.DataExtensionIndex = 
                material.FindNode<GrnDataExtensionReferenceNode>(
                GrnNodeType.DataExtensionReference).DataExtensionIndex - 1;
        }
        public void Write(GrnNode matSecNode, OrderedDictionary textureMaps)
        {
            GrnNode matNode = new GrnNode(matSecNode, GrnNodeType.Material);
            matSecNode.AppendChild(matNode);
            GrnMaterialSimpleDiffuseTextureNode matDiffNode =
                new GrnMaterialSimpleDiffuseTextureNode(matNode);
            matDiffNode.TextureMapIndex = (int)textureMaps[(object)this.TextureDataExtensionIndex] + 1;
            matDiffNode.Unknown = matDiffNode.TextureMapIndex;
            matNode.AppendChild(matDiffNode);
            GrnDataExtensionReferenceNode refNode = new GrnDataExtensionReferenceNode(matNode);
            refNode.DataExtensionIndex = this.DataExtensionIndex + 1;
            matNode.AppendChild(refNode);
        }
    }
}
