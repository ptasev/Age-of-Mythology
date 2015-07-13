using AoMEngineLibrary.Graphics.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoMEngineLibrary.Graphics.Grn
{
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

        internal void Read(GrnBinaryReader reader, List<int> textureDataRefExts, GrnNode material, uint directoryOffset)
        {
            // -- Each material has diffuseTex (0, textureIndex(+1), 1), and dataExtRef
            GrnNode matDiffuse = material.FindNode(GrnNodeType.MaterialSimpleDiffuseTexture);
            if (matDiffuse != null)
            {
                reader.Seek((int)(matDiffuse.Offset + directoryOffset), SeekOrigin.Begin);
                reader.ReadInt32();
                this.TextureDataExtensionIndex = textureDataRefExts[reader.ReadInt32() - 1];
                // skip last one
            }

            GrnNode dataExtRef = material.FindNode(GrnNodeType.DataExtensionReference);
            if (dataExtRef != null)
            {
                reader.Seek((int)(dataExtRef.Offset + directoryOffset), SeekOrigin.Begin);
                this.DataExtensionIndex = reader.ReadInt32() - 1;
            }
        }
    }
}
