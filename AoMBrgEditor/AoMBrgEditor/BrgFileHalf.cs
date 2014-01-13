using MiscUtil.Conversion;
using MiscUtil.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoMBrgEditor
{
    public struct Vector3<T>
    {
        public T X;
        public T Y;
        public T Z;

        public Vector3(T x, T y, T z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }
    }
    public struct Vector2<T>
    {
        public T X;
        public T Y;

        public Vector2(T x, T y)
        {
            this.X = x;
            this.Y = y;
        }
    }

    [Flags]
    public enum BrgMeshFlag : ushort
    {
        NONE1 = 0x8000,
        TRANSPCOLOR = 0x4000,
        NONE2 = 0x2000,
        NONE3 = 0x1000,
        MOVINGTEX = 0x0800,
        NOTFIRSTMESH = 0x0400,
        NONE4 = 0x0200,
        ATTACHPOINTS = 0x0100,
        NONE5 = 0x0080,
        MATERIALS = 0x0040,
        CHANGINGCOL = 0x0020,
        NONE7 = 0x0010,
        NONE8 = 0x0008,
        NONE9 = 0x0004,
        TEXTURE = 0x0002,
        VERTCOLOR = 0x0001
    };
    [Flags]
    public enum BrgMatFlag
    {
        SFX = 0x1C000000,
        GLOW = 0x00200000,
        MATNONE1 = 0x00800000,
        PLAYERCOLOR = 0x00040000,
        SOLIDCOLOR = 0x00020000,
        MATTEXTURE = 0x00000030
    };

    public struct BrgHeader
    {
        public int magic;
        public int unknown01;
        public int numMaterials;
        public int unknown02;
        public int numMeshes;
        public int space;
        public int unknown03;
    }

    public struct BrgAsetHeader
    {
        public int magic;
        public int numFrames;
        public float frameStep;
        public float animTime;
        public float frequency;
        public float spf;
        public float fps;
        public int space;
    }

    public class BrgFile
    {
        public BrgHeader Header;
        public BrgAsetHeader AsetHeader;
        public List<BrgMesh> Mesh;
        public List<BrgMaterial> Material;

        public BrgFile(System.IO.Stream fileStream)
        {
            using (BrgBinaryReader reader = new BrgBinaryReader(new LittleEndianBitConverter(), fileStream))
            {
                reader.ReadHeader(ref Header);
                if (Header.magic != EndianBitConverter.Little.ToInt32(Encoding.UTF8.GetBytes("BANG"), 0))
                {
                    throw new Exception("This is not a BRG file!");
                }

                if (Header.numMeshes > 1)
                {
                    reader.ReadAsetHeader(ref AsetHeader);
                    if (AsetHeader.magic != EndianBitConverter.Little.ToInt32(Encoding.UTF8.GetBytes("ASET"), 0))
                    {
                        throw new Exception("Improper ASET header!");
                    }
                    if (Header.numMeshes != AsetHeader.numFrames)
                    {
                        throw new Exception("Number of meshes does not match number of frames!");
                    }
                }

                Mesh = new List<BrgMesh>(Header.numMeshes);
                for (int i = 0; i < Header.numMeshes; i++)
                {
                    Mesh.Add(new BrgMesh(reader));
                }

                Material = new List<BrgMaterial>();
                for (int i = 0; i < Header.numMaterials; i++)
                {
                    Material.Add(new BrgMaterial(reader));
                }

                if (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    throw new Exception("The end of stream was not reached!");
                }
            }
        }

        public void WriteBr3(System.IO.Stream fileStream)
        {
            using (BrgBinaryWriter writer = new BrgBinaryWriter(new LittleEndianBitConverter(), fileStream))
            {
                writer.Write(860312130); // BRG3
                writer.Write(Header.numMeshes);

                if (Header.numMeshes > 1)
                {
                    writer.Write(AsetHeader.animTime);
                }

                foreach (BrgMesh mesh in Mesh)
                {
                    writer.Write(mesh.numVertices);
                    writer.Write(mesh.numFaces);
                    writer.Write((Int16)mesh.flags);

                    foreach (Vector3<Half> v in mesh.vertices)
                    {
                        writer.Write(-(float)v.X);
                        writer.Write(-(float)v.Z);
                        writer.Write((float)v.Y);
                        //writer.Write(-(float)v.X);
                        //writer.Write((float)v.Z);
                        //writer.Write(-(float)v.Y);
                    }
                    foreach (Vector3<Half> v in mesh.normals)
                    {
                        writer.Write(-(float)v.X);
                        writer.Write(-(float)v.Z);
                        writer.Write((float)v.Y);
                        //writer.Write(-(float)v.X);
                        //writer.Write((float)v.Z);
                        //writer.Write(-(float)v.Y);
                    }

                    if (!mesh.flags.HasFlag(BrgMeshFlag.NOTFIRSTMESH) || mesh.flags.HasFlag(BrgMeshFlag.MOVINGTEX))
                    {
                        if (mesh.flags.HasFlag(BrgMeshFlag.TEXTURE))
                        {
                            foreach (Vector2<Half> v in mesh.texVertices)
                            {
                                writer.Write((float)v.X);
                                writer.Write((float)v.Y);
                            }
                        }
                    }

                    if (!mesh.flags.HasFlag(BrgMeshFlag.NOTFIRSTMESH))
                    {
                        foreach (Vector3<Int16> v in mesh.faceVertices)
                        {
                            writer.Write((int)v.Y + 1);
                            writer.Write((int)v.X + 1);
                            writer.Write((int)v.Z + 1);
                        }
                    }
                }
            }
        }
    }

    public class BrgMesh
    {
        int magic;
        Int16 meshFormat;
        Int16 unknown01b;
        public Int16 numVertices;
        public Int16 numFaces;
        int unknown02;
        float[] unknown03;
        byte[] unknown04;
        public BrgMeshFlag flags;
        //byte unknown05 = flags;
        //byte unknown06 = flags >> 8;
        float[] unknown07;
        float meshX;
        float meshY;
        float meshZ;
        public Vector3<Half>[] vertices;
        public Vector3<Half>[] normals;

        public Vector2<Half>[] texVertices;
        Int16[] faceMaterials;
        public Vector3<Int16>[] faceVertices;
        Int16[] vertMaterials;

        Half[] unknown09;
        Int16 checkSpace; //09a
        Int16 unknown09e;
        float unknown09b;
        int lenSpace; //09c
        int unknown09d;

        Int16 numMatrix;
        Int16 numIndex;
        Int16 unknown10;
        Half[] unknown11;
        List<BrgAttachpoint> attachpoints;
        byte[] unknown14;

        public BrgMesh(BrgBinaryReader reader)
        {
            magic = reader.ReadInt32();
            if (magic != EndianBitConverter.Little.ToInt32(Encoding.UTF8.GetBytes("MESI"), 0))
            {
                throw new Exception("Improper mesh header!");
            }

            meshFormat = reader.ReadInt16();
            unknown01b = reader.ReadInt16();
            numVertices = reader.ReadInt16();
            numFaces = reader.ReadInt16();
            unknown02 = reader.ReadInt32();

            unknown03 = new float[9];
            for (int i = 0; i < 9; i++)
            {
                unknown03[i] = reader.ReadSingle();
            }

            unknown04 = new byte[6];
            unknown04 = reader.ReadBytes(6);

            flags = (BrgMeshFlag)reader.ReadInt16();
            unknown07 = new float[3];
            for (int i = 0; i < 3; i++)
            {
                unknown07[i] = reader.ReadSingle();
            }

            meshX = reader.ReadSingle();
            meshY = reader.ReadSingle();
            meshZ = reader.ReadSingle();

            vertices = new Vector3<Half>[numVertices];
            for (int i = 0; i < numVertices; i++)
            {
                reader.ReadVector3(out vertices[i]);
            }
            normals = new Vector3<Half>[numVertices];
            for (int i = 0; i < numVertices; i++)
            {
                reader.ReadVector3(out normals[i]);
            }

            if (!flags.HasFlag(BrgMeshFlag.NOTFIRSTMESH) || flags.HasFlag(BrgMeshFlag.MOVINGTEX))
            {
                if (flags.HasFlag(BrgMeshFlag.TEXTURE))
                {
                    texVertices = new Vector2<Half>[numVertices];
                    for (int i = 0; i < numVertices; i++)
                    {
                        reader.ReadVector2(out texVertices[i]);
                    }
                }
            }

            if (!flags.HasFlag(BrgMeshFlag.NOTFIRSTMESH))
            {
                if (flags.HasFlag(BrgMeshFlag.MATERIALS))
                {
                    faceMaterials = new Int16[numFaces];
                    for (int i = 0; i < numFaces; i++)
                    {
                        faceMaterials[i] = reader.ReadInt16();
                    }
                }

                faceVertices = new Vector3<Int16>[numFaces];
                for (int i = 0; i < numFaces; i++)
                {
                    reader.ReadVector3(out faceVertices[i]);
                }

                if (flags.HasFlag(BrgMeshFlag.MATERIALS))
                {
                    vertMaterials = new Int16[numVertices];
                    for (int i = 0; i < numVertices; i++)
                    {
                        vertMaterials[i] = reader.ReadInt16();
                    }
                }
            }

            unknown09 = new Half[12];
            for (int i = 0; i < 12; i++)
            {
                unknown09[i] = reader.ReadHalf();
            }
            checkSpace = reader.ReadInt16();
            unknown09e = reader.ReadInt16();

            if (checkSpace == 0)
            {
                unknown09b = reader.ReadSingle();
                lenSpace = reader.ReadInt32(); //09c
                unknown09d = reader.ReadInt32();
            }

            // Implement this later
            //if (unknown05 == 97 || unknown06 == 200 || unknown06 == 204 || unknown06 == 72 || (unknown06 == 76 && unknown05 != 98)) {
            //    byte unknown0a[4 * numVertices];
            //}

            if (flags.HasFlag(BrgMeshFlag.ATTACHPOINTS))
            {
                numMatrix = reader.ReadInt16();
                numIndex = reader.ReadInt16();
                unknown10 = reader.ReadInt16();

                BrgAttachpoint[] attpts = new BrgAttachpoint[numMatrix];
                for (int i = 0; i < numMatrix; i++)
                {
                    attpts[i] = new BrgAttachpoint(reader);
                }
                unknown11 = new Half[6 * numMatrix];
                for (int i = 0; i < 6 * numMatrix; i++)
                {
                    unknown11[i] = reader.ReadHalf();
                }

                List<int> nameId = new List<int>();
                for (int i = 0; i < numIndex; i++)
                {
                    int duplicate = reader.ReadInt32(); // have yet to find a model with duplicates
                    reader.ReadInt32(); // Skip the id (at least I think its an ID)
                    for (int j = 0; j < duplicate; j++)
                    {
                        nameId.Add(i);
                    }
                }

                attachpoints = new List<BrgAttachpoint>(nameId.Count);
                for (int i = 0; i < nameId.Count; i++)
                {
                    attachpoints.Add(new BrgAttachpoint(attpts[reader.ReadByte()]));
                    attachpoints[i].NameId = nameId[i];
                }

                if (checkSpace == 0 && lenSpace > 0)
                {
                    unknown14 = reader.ReadBytes(4 * lenSpace);
                }
            }
        }
    }

    public class BrgAttachpoint
    {
        public static string[] AttachpointNames = new string[55] { 
            "TARGETPOINT", "LAUNCHPOINT", "CORPSE", "DECAL", "FIRE", "GATHERPOINT", "RESERVED9", "RESERVED8", "RESERVED7", "RESERVED6", "RESERVED5", "RESERVED4", "RESERVED3", "RESERVED2", "RESERVED1", "RESERVED0", 
            "SMOKE9", "SMOKE8", "SMOKE7", "SMOKE6", "SMOKE5", "SMOKE4", "SMOKE3", "SMOKE2", "SMOKE1", "SMOKE0", "GARRISONFLAG", "HITPOINTBAR", "RIGHTFOREARM", "LEFTFOREARM", "RIGHTFOOT", "LEFTFOOT", 
            "RIGHTLEG", "LEFTLEG", "RIGHTTHIGH", "LEFTTHIGH", "PELVIS", "BACKABDOMEN", "FRONTABDOMEN", "BACKCHEST", "FRONTCHEST", "RIGHTSHOULDER", "LEFTSHOULDER", "NECK", "RIGHTEAR", "LEFTEAR", "CHIN", "FACE", 
            "FOREHEAD", "TOPOFHEAD", "RIGHTHAND", "LEFTHAND", "RESERVED", "SMOKEPOINT", "ATTACHPOINT"
         };

        public int NameId;
        Vector3<Half> x;
        Vector3<Half> y;
        Vector3<Half> z;
        Vector3<Half> position;

        public string Name
        {
            get
            {
                if (NameId >= 0 && NameId <= 55)
                {
                    return AttachpointNames[54 - NameId];
                }
                else
                {
                    throw new Exception("Invalid Attachpoint Name Id!");
                }
            }
        }

        public BrgAttachpoint(BrgBinaryReader reader)
        {
            NameId = -1;
            reader.ReadVector3(out x);
            reader.ReadVector3(out y);
            reader.ReadVector3(out z);
            reader.ReadVector3(out position);
        }
        public BrgAttachpoint(BrgAttachpoint prev)
        {
            NameId = prev.NameId;
            x = prev.x;
            y = prev.y;
            z = prev.z;
        }
    }

    public class BrgMaterial
    {
        int magic;
        int id;
        BrgMatFlag flags;
        int unknown01b;
        int nameLength;
        Vector3<float> color; //unknown02 [36 bytes]
        Vector3<float> specular;
        Vector3<float> reflection;
        Vector3<float> ambient; //unknown03 [12 bytes]
        string name;
        float alpha; //unknown04
        float unknown05;
        List<BrgMatSFX> sfx;

        public BrgMaterial(BrgBinaryReader reader)
        {
            magic = reader.ReadInt32();
            if (magic != EndianBitConverter.Little.ToInt32(Encoding.UTF8.GetBytes("MTRL"), 0))
            {
                throw new Exception("Incorrect material header!");
            }

            id = reader.ReadInt32();
            flags = (BrgMatFlag)reader.ReadInt32();
            unknown01b = reader.ReadInt32();
            nameLength = reader.ReadInt32();
            reader.ReadVector3(out color);
            reader.ReadVector3(out specular);
            reader.ReadVector3(out reflection);
            reader.ReadVector3(out ambient);

            name = reader.ReadString(nameLength);
            alpha = reader.ReadSingle();

            if (flags.HasFlag(BrgMatFlag.SOLIDCOLOR))
            {
                unknown05 = reader.ReadSingle();
            }

            if (flags.HasFlag(BrgMatFlag.SFX))
            {
                byte numSFX = reader.ReadByte();
                sfx = new List<BrgMatSFX>(numSFX);
                for (int i = 0; i < numSFX; i++)
                {
                    sfx.Add(reader.ReadMaterialSFX());
                }
            }
            else
            {
                sfx = new List<BrgMatSFX>();
            }
        }
    }

    public struct BrgMatSFX
    {
        public byte Id;
        public string Name;
    }

    public class BrgBinaryReader : EndianBinaryReader
    {
        public BrgBinaryReader(EndianBitConverter bitConvertor, System.IO.Stream stream)
            : base(bitConvertor, stream)
        {
        }

        public void ReadHeader(ref BrgHeader header)
        {
            header.magic = this.ReadInt32();
            header.unknown01 = this.ReadInt32();
            header.numMaterials = this.ReadInt32();
            header.unknown02 = this.ReadInt32();
            header.numMeshes = this.ReadInt32();
            header.space = this.ReadInt32();
            header.unknown03 = this.ReadInt32();
        }
        public void ReadAsetHeader(ref BrgAsetHeader header)
        {
            header.magic = this.ReadInt32();
            header.numFrames = this.ReadInt32();
            header.frameStep = this.ReadSingle();
            header.animTime = this.ReadSingle();
            header.frequency = this.ReadSingle();
            header.spf = this.ReadSingle();
            header.fps = this.ReadSingle();
            header.space = this.ReadInt32();
        }
        public BrgMatSFX ReadMaterialSFX()
        {
            BrgMatSFX sfx;

            sfx.Id = this.ReadByte();
            sfx.Name = this.ReadString(this.ReadInt16());

            return sfx;
        }

        #region ReadVector3
        public void ReadVector3(out Vector3<float> v)
        {
            v.X = this.ReadSingle();
            v.Y = this.ReadSingle();
            v.Z = this.ReadSingle();
        }
        public void ReadVector3(out Vector3<Int16> v)
        {
            v.X = this.ReadInt16();
            v.Y = this.ReadInt16();
            v.Z = this.ReadInt16();
        }
        public void ReadVector3(out Vector3<Half> v)
        {
            v.X = this.ReadHalf();
            v.Y = this.ReadHalf();
            v.Z = this.ReadHalf();
        }
        #endregion

        #region ReadVector2
        public void ReadVector2(out Vector2<float> v)
        {
            v.X = this.ReadSingle();
            v.Y = this.ReadSingle();
        }
        public void ReadVector2(out Vector2<Half> v)
        {
            v.X = this.ReadHalf();
            v.Y = this.ReadHalf();
        }
        #endregion

        public Half ReadHalf()
        {
            byte[] f = new byte[4];
            byte[] h = this.ReadBytes(2);
            f[2] = h[0];
            f[3] = h[1];
            return HalfHelper.SingleToHalf(EndianBitConverter.Little.ToSingle(f, 0));
            //return Half.ToHalf(this.ReadBytes(2), 0);
        }
        public string ReadString(int length)
        {
            return Encoding.UTF8.GetString(this.ReadBytes(length));
        }
    }

    public class BrgBinaryWriter : EndianBinaryWriter
    {
        public BrgBinaryWriter(EndianBitConverter bitConvertor, System.IO.Stream stream)
            : base(bitConvertor, stream)
        {
        }

        public void WritePSSGString(string str)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            this.Write(bytes.Length);
            this.Write(bytes);
        }
    }
}
