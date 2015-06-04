namespace AoMEngineLibrary.Graphics.Model
{
    using AoMEngineLibrary.Graphics.Brg;
    using MiscUtil.Conversion;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public static class ModelFileReader
    {
        //public static ModelFile ReadBrg(Stream stream)
        //{
        //    ModelFile file = new ModelFile();

        //    using (BrgBinaryReader reader = new BrgBinaryReader(new LittleEndianBitConverter(), stream))
        //    {
        //        BrgHeader header = new BrgHeader(reader);
        //        if (header.Magic != "BANG")
        //        {
        //            throw new Exception("This is not a BRG file!");
        //        }

        //        int asetCount = 0;
        //        file.Meshes = new List<Mesh>(header.NumMeshes);
        //        file.Materials = new List<Material>();
        //        while (reader.BaseStream.Position < reader.BaseStream.Length)
        //        {
        //            string magic = reader.ReadString(4);
        //            if (magic == "ASET")
        //            {
        //                BrgAsetHeader AsetHeader = new BrgAsetHeader(reader);
        //                ++asetCount;
        //            }
        //            else if (magic == "MESI")
        //            {
        //                file.Meshes.Add(new BrgMesh(reader, this));
        //            }
        //            else if (magic == "MTRL")
        //            {
        //                BrgMaterial mat = new BrgMaterial(reader, this);
        //                if (!ContainsMaterialID(mat.id))
        //                {
        //                    file.Materials.Add(mat);
        //                }
        //                else
        //                {
        //                    //throw new Exception("Duplicate material ids!");
        //                }
        //            }
        //            else
        //            {
        //                throw new Exception("The type tag " +/* magic +*/ " is not recognized!");
        //            }
        //        }

        //        if (asetCount > 1)
        //        {
        //            //throw new Exception("Multiple ASETs!");
        //        }

        //        if (Header.NumMeshes < Meshes.Count)
        //        {
        //            throw new Exception("Inconsistent mesh count!");
        //        }

        //        if (Header.NumMaterials < Materials.Count)
        //        {
        //            throw new Exception("Inconsistent material count!");
        //        }

        //        if (reader.BaseStream.Position < reader.BaseStream.Length)
        //        {
        //            throw new Exception("The end of stream was not reached!");
        //        }
        //    }

        //    return file;
        //}
    }
}
