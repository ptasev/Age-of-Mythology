namespace AoMEngineLibrary.Graphics.Brg
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.IO;

    public class BrgFile
    {
        public string FileName { get; set; }

        public BrgHeader Header { get; set; }
        public BrgAsetHeader AsetHeader { get; private set; }

        public List<BrgMesh> Meshes { get; set; }
        public List<BrgMaterial> Materials { get; set; }

        public BrgAnimation Animation { get; set; }

        public BrgFile()
        {
            this.FileName = string.Empty;
            this.Header = new BrgHeader();
            this.AsetHeader = new BrgAsetHeader();

            this.Meshes = new List<BrgMesh>();
            this.Materials = new List<BrgMaterial>();

            this.Animation = new BrgAnimation();
        }
        public BrgFile(System.IO.FileStream fileStream)
            : this()
        {
            using (BrgBinaryReader reader = new BrgBinaryReader(fileStream))
            {
                this.FileName = fileStream.Name;
                this.Header = new BrgHeader(reader);
                if (this.Header.Magic != "BANG")
                {
                    throw new Exception("This is not a BRG file!");
                }
                this.AsetHeader = new BrgAsetHeader();

                int asetCount = 0;
                this.Meshes = new List<BrgMesh>(this.Header.NumMeshes);
                this.Materials = new List<BrgMaterial>();
                while (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    string magic = reader.ReadString(4);
                    if (magic == "ASET")
                    {
                        this.AsetHeader = new BrgAsetHeader(reader);
                        ++asetCount;
                    }
                    else if (magic == "MESI")
                    {
                        this.Meshes.Add(new BrgMesh(reader, this));
                    }
                    else if (magic == "MTRL")
                    {
                        BrgMaterial mat = new BrgMaterial(reader, this);
                        Materials.Add(mat);
                        if (!ContainsMaterialID(mat.Id))
                        {
                            //Materials.Add(mat);
                        }
                        else
                        {
                            //throw new Exception("Duplicate material ids!");
                        }
                    }
                    else
                    {
                        throw new Exception("The type tag " +/* magic +*/ " is not recognized!");
                    }
                }

                if (asetCount > 1)
                {
                    //throw new Exception("Multiple ASETs!");
                }

                if (Header.NumMeshes < Meshes.Count)
                {
                    throw new Exception("Inconsistent mesh count!");
                }

                if (Header.NumMaterials < Materials.Count)
                {
                    throw new Exception("Inconsistent material count!");
                }

                if (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    throw new Exception("The end of stream was not reached!");
                }

                this.Animation.Duration = this.Meshes[0].ExtendedHeader.AnimationLength;
                if (this.Meshes[0].Header.AnimationType.HasFlag(BrgMeshAnimType.NonUniform))
                {
                    for (int i = 0; i < this.Meshes.Count; ++i)
                    {
                        this.Animation.MeshKeys.Add(this.Meshes[0].NonUniformKeys[i] * this.Animation.Duration);
                    }
                }
                else if (this.Meshes.Count > 1)
                {
                    float divisor = this.Meshes.Count - 1;
                    for (int i = 0; i < this.Meshes.Count; ++i)
                    {
                        this.Animation.MeshKeys.Add((float)i / divisor * this.Animation.Duration);
                    }
                }
                else
                {
                    this.Animation.MeshKeys.Add(0);
                }
            }
        }


        public void Write(FileStream fileStream)
        {
            using (fileStream)
            {
                this.FileName = fileStream.Name;
                Write((Stream)fileStream);
            }
        }
        public void Write(Stream fileStream)
        {
            using (BrgBinaryWriter writer = new BrgBinaryWriter(fileStream, true))
            {
                this.Header.Write(writer);

                if (this.Header.NumMeshes > 1)
                {
                    updateAsetHeader();
                    writer.Write(1413829441); // magic "ASET"
                    this.AsetHeader.Write(writer);
                }

                for (int i = 0; i < this.Meshes.Count; i++)
                {
                    writer.Write(1230193997); // magic "MESI"
                    this.Meshes[i].Write(writer);
                }

                for (int i = 0; i < this.Materials.Count; i++)
                {
                    writer.Write(1280463949); // magic "MTRL"
                    this.Materials[i].Write(writer);
                }
            }
        }

        public bool ContainsMaterialID(int id)
        {
            for (int i = 0; i < Materials.Count; i++)
            {
                if (Materials[i].Id == id)
                {
                    return true;
                }
            }

            return false;
        }
        private void updateAsetHeader()
        {
            AsetHeader.numFrames = this.Animation.MeshKeys.Count;
            AsetHeader.frameStep = 1f / (float)AsetHeader.numFrames;
            AsetHeader.animTime = this.Animation.Duration;
            AsetHeader.frequency = 1f / (float)AsetHeader.animTime;
            AsetHeader.spf = AsetHeader.animTime / (float)AsetHeader.numFrames;
            AsetHeader.fps = (float)AsetHeader.numFrames / AsetHeader.animTime;
        }

        public void UpdateMeshSettings()
        {
            if (this.Meshes.Count == 0)
            {
                return;
            }

            BrgMesh mesh = this.Meshes[0];
            for (int i = 0; i < this.Meshes.Count; i++)
            {
                UpdateMeshSettings(i, mesh.Header.Flags, mesh.Header.Format, mesh.Header.AnimationType, mesh.Header.InterpolationType);
            }
        }
        public void UpdateMeshSettings(BrgMeshFlag flags, BrgMeshFormat format, BrgMeshAnimType animType, BrgMeshInterpolationType interpolationType)
        {
            if (this.Meshes.Count == 0)
            {
                return;
            }

            for (int i = 0; i < this.Meshes.Count; i++)
            {
                UpdateMeshSettings(i, flags, format, animType, interpolationType);
            }
        }
        public void UpdateMeshSettings(int meshIndex, BrgMeshFlag flags, BrgMeshFormat format, BrgMeshAnimType animType, BrgMeshInterpolationType interpolationType)
        {
            if (meshIndex > 0)
            {
                this.Meshes[meshIndex].Header.Flags = flags;
                this.Meshes[meshIndex].Header.Format = format;
                this.Meshes[meshIndex].Header.AnimationType = animType;
                this.Meshes[meshIndex].Header.InterpolationType = interpolationType;
                this.Meshes[meshIndex].Header.Flags |= BrgMeshFlag.SECONDARYMESH;
                this.Meshes[meshIndex].Header.AnimationType &= ~BrgMeshAnimType.NonUniform;
            }
            else
            {
                this.Meshes[meshIndex].Header.Flags = flags;
                this.Meshes[meshIndex].Header.Format = format;
                this.Meshes[meshIndex].Header.AnimationType = animType;
                this.Meshes[meshIndex].Header.InterpolationType = interpolationType;

                if (this.Meshes[meshIndex].Header.AnimationType == BrgMeshAnimType.NonUniform)
                {
                    this.Meshes[meshIndex].NonUniformKeys = new List<float>(this.Header.NumMeshes);
                    for (int i = 0; i < this.Header.NumMeshes; i++)
                    {
                        this.Meshes[meshIndex].NonUniformKeys.Add(this.Animation.MeshKeys[i] / this.Animation.Duration);
                    }
                }
            }
        }
    }
}
