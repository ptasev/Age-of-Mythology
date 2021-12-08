using AoMEngineLibrary.Extensions;
using System;
using System.Collections.Generic;
using System.IO;

namespace AoMEngineLibrary.Graphics.Brg
{
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
            FileName = string.Empty;
            Header = new BrgHeader();
            AsetHeader = new BrgAsetHeader();

            Meshes = new List<BrgMesh>();
            Materials = new List<BrgMaterial>();

            Animation = new BrgAnimation();
        }
        public BrgFile(FileStream fileStream)
            : this((Stream)fileStream)
        {
            FileName = fileStream.Name;
        }
        public BrgFile(Stream fileStream)
            : this()
        {
            using (var reader = new BrgBinaryReader(fileStream))
            {
                Header = new BrgHeader(reader);
                if (Header.Magic != BrgHeader.BangMagic)
                {
                    throw new FileFormatException("This is not a BRG file.");
                }

                AsetHeader = new BrgAsetHeader();
                Meshes = new List<BrgMesh>(Header.NumMeshes);
                Materials = new List<BrgMaterial>();
                Animation = new BrgAnimation();

                if (Header.NumMeshes > 1)
                {
                    ReadAnimation(reader);
                }
                else
                {
                    ReadMesh(reader);
                    Animation.MeshKeys.Add(0);
                }

                if (Header.NumMaterials > 0)
                {
                    ReadMaterials(reader);
                }

                if (Header.NumMeshes < Meshes.Count)
                {
                    throw new InvalidDataException("Inconsistent mesh count!");
                }

                if (Header.NumMaterials < Materials.Count)
                {
                    throw new InvalidDataException("Inconsistent material count!");
                }
            }
        }

        private void ReadAnimation(BrgBinaryReader reader)
        {
            var magic = reader.ReadStringOfLength(4);
            if (magic != "ASET")
            {
                throw new InvalidDataException($"This ({magic.Trim('\0')}) is not a valid mesh animation set chunk.");
            }

            AsetHeader = new BrgAsetHeader(reader);
            for (var i = 0; i < AsetHeader.NumFrames; ++i)
            {
                ReadMesh(reader);
            }

            // Determine animation duration
            if (Meshes[0].Header.Format.HasFlag(BrgMeshFormat.AnimationLength))
            {
                Animation.Duration = Meshes[0].ExtendedHeader.AnimationLength;
            }
            else if (AsetHeader.AnimTime < 0.000001f)
            {
                Animation.Duration = 1.0f;
            }
            else
            {
                Animation.Duration = AsetHeader.AnimTime;
            }

            // Compute animation keys
            if (Meshes[0].Header.AnimationType.HasFlag(BrgMeshAnimType.NonUniform))
            {
                for (var i = 0; i < Meshes.Count; ++i)
                {
                    Animation.MeshKeys.Add(Meshes[0].NonUniformKeys[i] * Animation.Duration);
                }
            }
            else if (Meshes.Count > 1)
            {
                float divisor = Meshes.Count - 1;
                for (var i = 0; i < Meshes.Count; ++i)
                {
                    Animation.MeshKeys.Add(i / divisor * Animation.Duration);
                }
            }
            else
            {
                Animation.MeshKeys.Add(0);
            }
        }

        private void ReadMesh(BrgBinaryReader reader)
        {
            // keep reading tags until we find a mesh tag, or an invalid tag
            while (true)
            {
                var magic = reader.ReadStringOfLength(4);
                if (magic == "MESI")
                {
                    Meshes.Add(new BrgMesh(reader));
                    break;
                }
                else if (magic == "ASET")
                {
                    // Read ASET data during mesh read, but ignore it
                    _ = new BrgAsetHeader(reader);
                }
                else if (magic == "AFRM")
                {
                    // seems to be some sort of legacy tag that AoM code skips
                }
                else
                {
                    throw new InvalidDataException($"This ({magic.Trim('\0')}) is not a valid mesh chunk.");
                }
            }
        }

        private void ReadMaterials(BrgBinaryReader reader)
        {
            for (var i = 0; i < Header.NumMaterials; ++i)
            {
                ReadMaterial(reader);
            }
        }

        private void ReadMaterial(BrgBinaryReader reader)
        {
            var magic = reader.ReadStringOfLength(4);
            if (magic != "MTRL")
            {
                throw new InvalidDataException($"This ({magic.Trim('\0')}) is not a valid material chunk.");
            }

            var mat = new BrgMaterial(reader);
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

        public void Write(FileStream fileStream)
        {
            using (fileStream)
            {
                FileName = fileStream.Name;
                Write((Stream)fileStream);
            }
        }
        public void Write(Stream stream)
        {
            using (var writer = new BrgBinaryWriter(stream, true))
            {
                Header.Write(writer);

                if (Header.NumMeshes > 1)
                {
                    updateAsetHeader();
                    writer.Write(1413829441); // magic "ASET"
                    AsetHeader.Write(writer);
                }

                for (var i = 0; i < Meshes.Count; i++)
                {
                    writer.Write(1230193997); // magic "MESI"
                    Meshes[i].Write(writer);
                }

                for (var i = 0; i < Materials.Count; i++)
                {
                    writer.Write(1280463949); // magic "MTRL"
                    Materials[i].Write(writer);
                }
            }
        }

        public bool ContainsMaterialID(int id)
        {
            for (var i = 0; i < Materials.Count; i++)
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
            AsetHeader.NumFrames = (uint)Animation.MeshKeys.Count;
            AsetHeader.InvFrames = 1f / AsetHeader.NumFrames;
            AsetHeader.AnimTime = Animation.Duration;
            AsetHeader.Frequency = 1f / AsetHeader.AnimTime;
            AsetHeader.Spf = AsetHeader.AnimTime / AsetHeader.NumFrames;
            AsetHeader.Fps = AsetHeader.NumFrames / AsetHeader.AnimTime;
        }

        public void UpdateMeshSettings()
        {
            if (Meshes.Count == 0)
            {
                return;
            }

            var mesh = Meshes[0];
            for (var i = 0; i < Meshes.Count; i++)
            {
                UpdateMeshSettings(i, mesh.Header.Flags, mesh.Header.Format, mesh.Header.AnimationType, mesh.Header.InterpolationType);
            }
        }
        public void UpdateMeshSettings(BrgMeshFlag flags, BrgMeshFormat format, BrgMeshAnimType animType, BrgMeshInterpolationType interpolationType)
        {
            if (Meshes.Count == 0)
            {
                return;
            }

            for (var i = 0; i < Meshes.Count; i++)
            {
                UpdateMeshSettings(i, flags, format, animType, interpolationType);
            }
        }
        public void UpdateMeshSettings(int meshIndex, BrgMeshFlag flags, BrgMeshFormat format, BrgMeshAnimType animType, BrgMeshInterpolationType interpolationType)
        {
            if (meshIndex > 0)
            {
                Meshes[meshIndex].Header.Flags = flags;
                Meshes[meshIndex].Header.Format = format;
                Meshes[meshIndex].Header.AnimationType = animType;
                Meshes[meshIndex].Header.InterpolationType = interpolationType;
                Meshes[meshIndex].Header.Flags |= BrgMeshFlag.SECONDARYMESH;
                Meshes[meshIndex].Header.AnimationType &= ~BrgMeshAnimType.NonUniform;
            }
            else
            {
                Meshes[meshIndex].Header.Flags = flags;
                Meshes[meshIndex].Header.Format = format;
                Meshes[meshIndex].Header.AnimationType = animType;
                Meshes[meshIndex].Header.InterpolationType = interpolationType;

                if (Meshes[meshIndex].Header.AnimationType == BrgMeshAnimType.NonUniform)
                {
                    Meshes[meshIndex].NonUniformKeys = new List<float>(Header.NumMeshes);
                    for (var i = 0; i < Header.NumMeshes; i++)
                    {
                        Meshes[meshIndex].NonUniformKeys.Add(Animation.MeshKeys[i] / Animation.Duration);
                    }
                }
            }
        }
    }
}
