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
        SFX =           0x1C000000,
        GLOW =          0x00200000,
        MATNONE1 =      0x00800000,
        PLAYERCOLOR =   0x00040000,
        SOLIDCOLOR =    0x00020000,
        TITANGATE =     0x00008300,
        MATTEXTURE =    0x00000030
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
                    Mesh.Add(new BrgMesh(reader, this));
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

        public void Write(System.IO.Stream fileStream)
        {
            using (BrgBinaryWriter writer = new BrgBinaryWriter(new LittleEndianBitConverter(), fileStream))
            {
                writer.WriteHeader(ref Header);

                if (Header.numMeshes > 1)
                {
                    writer.WriteAsetHeader(ref AsetHeader);
                }

                for (int i = 0; i < Mesh.Count; i++)
                {
                    Mesh[i].Write(writer);
                }

                for (int i = 0; i < Material.Count; i++)
                {
                    Material[i].Write(writer);
                }
            }
        }

        public void ReadBr3(System.IO.Stream fileStream)
        {
            using (BrgBinaryReader reader = new BrgBinaryReader(new LittleEndianBitConverter(), fileStream))
            {
                reader.ReadInt32(); // BRG3

                Header.numMeshes = reader.ReadInt32();
                Header.numMaterials = reader.ReadInt32();

                if (Header.numMeshes > 1)
                {
                    AsetHeader.numFrames = Header.numMeshes;
                    AsetHeader.frameStep = 1f / (float)AsetHeader.numFrames;
                    AsetHeader.animTime = reader.ReadSingle();
                    AsetHeader.frequency = 1f / (float)AsetHeader.animTime;
                    AsetHeader.spf = AsetHeader.animTime / (float)AsetHeader.numFrames;
                    AsetHeader.fps = (float)AsetHeader.numFrames / AsetHeader.animTime;
                }

                // Copy the last mesh for every new frame
                int numNewFrames = Header.numMeshes - Mesh.Count;
                int lastFrameIndex = Mesh.Count - 1;
                for (int i = 0; i < numNewFrames; i++)
                {
                    Mesh.Add(new BrgMesh(Mesh[lastFrameIndex]));
                }
                // Load new data into mesh
                for (int i = 0; i < Header.numMeshes; i++)
                {
                    Mesh[i].ReadBr3(reader);
                }

                // Copy first material for the new ones
                int numNewMaterials = Header.numMaterials - Material.Count;
                for (int i = 0; i < numNewMaterials; i++)
                {
                    Material.Add(new BrgMaterial(Material[0]));
                }
                // Load new data into the materials
                for (int i = 0; i < Header.numMaterials; i++)
                {
                    Material[i].ReadBr3(reader);
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
                writer.Write(Header.numMaterials);

                if (Header.numMeshes > 1)
                {
                    writer.Write(AsetHeader.animTime);
                }

                foreach (BrgMesh mesh in Mesh)
                {
                    mesh.WriteBr3(writer);
                }

                foreach (BrgMaterial mat in Material)
                {
                    mat.WriteBr3(writer);
                }
            }
        }
    }

    public class BrgMesh
    {
        public BrgFile ParentFile;

        int magic;
        public Int16 meshFormat;
        public Int16 meshFormat2;
        public Int16 numVertices;
        public Int16 numFaces;
        public int unknown02;
        public float[] unknown03;
        public float groundZ;
        public byte[] unknown04;
        public BrgMeshFlag flags;
        Vector3<float> negativeMeshPos;
        Vector3<float> meshPos;
        public Vector3<float>[] vertices;
        public Vector3<float>[] normals;

        public Vector2<float>[] texVertices;
        public Int16[] faceMaterials;
        public Vector3<Int16>[] faceVertices;
        Int16[] vertMaterials;

        public int[] unknown09;
        public float animTime;
        public int unknown09Const;
        Int16 checkSpace; //09a
        public Int16 unknown09e;
        public float unknown09b;
        public int lenSpace; //09c
        public int unknown09d;

        public Int16 numMatrix;
        Int16 numIndex;
        public Int16 unknown10;
        public List<BrgAttachpoint> attachpoints;
        float[] unknown14;

        public BrgMesh(BrgBinaryReader reader, BrgFile file)
        {
            ParentFile = file;
            magic = reader.ReadInt32();
            if (magic != EndianBitConverter.Little.ToInt32(Encoding.UTF8.GetBytes("MESI"), 0))
            {
                throw new Exception("Improper mesh header!");
            }

            meshFormat = reader.ReadInt16();
            meshFormat2 = reader.ReadInt16();
            numVertices = reader.ReadInt16();
            numFaces = reader.ReadInt16();
            unknown02 = reader.ReadInt32();

            unknown03 = new float[8];
            for (int i = 0; i < 8; i++)
            {
                unknown03[i] = reader.ReadSingle();
            }
            groundZ = reader.ReadSingle();

            unknown04 = new byte[6];
            unknown04 = reader.ReadBytes(6);

            flags = (BrgMeshFlag)reader.ReadInt16();
            reader.ReadVector3(out negativeMeshPos, true);
            reader.ReadVector3(out meshPos, true);

            vertices = new Vector3<float>[numVertices];
            for (int i = 0; i < numVertices; i++)
            {
                reader.ReadVector3(out vertices[i], true, true);
            }
            normals = new Vector3<float>[numVertices];
            for (int i = 0; i < numVertices; i++)
            {
                reader.ReadVector3(out normals[i], true, true);
            }

            if (!flags.HasFlag(BrgMeshFlag.NOTFIRSTMESH) || flags.HasFlag(BrgMeshFlag.MOVINGTEX))
            {
                if (flags.HasFlag(BrgMeshFlag.TEXTURE))
                {
                    texVertices = new Vector2<float>[numVertices];
                    for (int i = 0; i < numVertices; i++)
                    {
                        reader.ReadVector2(out texVertices[i], true);
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

            unknown09 = new int[4];
            for (int i = 0; i < 4; i++)
            {
                unknown09[i] = reader.ReadInt32();
            }
            animTime = reader.ReadSingle();
            unknown09Const = reader.ReadInt32();
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
                    attpts[i] = new BrgAttachpoint();
                }
                for (int i = 0; i < numMatrix; i++)
                {
                    reader.ReadVector3(out attpts[i].x, true, true);
                } 
                for (int i = 0; i < numMatrix; i++)
                {
                    reader.ReadVector3(out attpts[i].y, true, true);
                } 
                for (int i = 0; i < numMatrix; i++)
                {
                    reader.ReadVector3(out attpts[i].z, true, true);
                }
                for (int i = 0; i < numMatrix; i++)
                {
                    reader.ReadVector3(out attpts[i].position, true, true);
                }
                for (int i = 0; i < numMatrix; i++)
                {
                    reader.ReadVector3(out attpts[i].unknown11a, true, true);
                }
                for (int i = 0; i < numMatrix; i++)
                {
                    reader.ReadVector3(out attpts[i].unknown11b, true, true);
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
                    unknown14 = new float[lenSpace];
                    for (int i = 0; i < lenSpace; i++)
                    {
                        unknown14[i] = reader.ReadSingle();
                    }
                }
            }
        }
        public BrgMesh(BrgFile file)
        {
            ParentFile = file;
            magic = 1230193997; //MESI
            meshFormat = 22;
            meshFormat2 = 12; // 4 -- (1 frame/unknown09[4] = 0), 1(5) -- Projectile?, 8(12) -- Animated
            numVertices = 0;
            numFaces = 0;
            unknown02 = 256; // 0, 256(something with lenSpace?), or 1
            unknown03 = new float[8];
            groundZ = 0;
            unknown04 = new byte[6];
            unknown04[4] = 40;
            unknown04[5] = 0;
            flags = 0;
            negativeMeshPos = new Vector3<float>();
            meshPos = new Vector3<float>();
            vertices = new Vector3<float>[numVertices];
            normals = new Vector3<float>[numVertices];
            texVertices = new Vector2<float>[numVertices];
            faceMaterials = new Int16[numFaces];
            faceVertices = new Vector3<Int16>[numFaces];
            vertMaterials = new Int16[numVertices];

            unknown09 = new int[4];
            unknown09[0] = 0; // always 0
            unknown09[1] = 0; // int, usually 0
            unknown09[2] = 0; // always 0
            unknown09[3] = 1; // something to do with 09d, int
            animTime = 0;
            unknown09Const = 191738312; // different for aomx models
            checkSpace = 0;
            unknown09e = 0; // always this
            unknown09b = 1; // always this
            lenSpace = 0;
            unknown09d = 2; // 1 + unknown09[3] --min = 1, max = 7

            // Implement this later
            //if (unknown05 == 97 || unknown06 == 200 || unknown06 == 204 || unknown06 == 72 || (unknown06 == 76 && unknown05 != 98)) {
            //    byte unknown0a[4 * numVertices];
            //}

            numMatrix = 0;
            numIndex = 0;
            unknown10 = 1; // always seems to be this
            attachpoints = new List<BrgAttachpoint>(numMatrix);
            unknown14 = new float[lenSpace];
        }
        public BrgMesh(BrgMesh copy)
        {
            ParentFile = copy.ParentFile;
            magic = copy.magic;

            meshFormat = copy.meshFormat;
            meshFormat2 = copy.meshFormat2;
            numVertices = copy.numVertices;
            numFaces = copy.numFaces;
            unknown02 = copy.unknown02;

            unknown03 = new float[copy.unknown03.Length];
            Array.Copy(copy.unknown03, unknown03, copy.unknown03.Length);
            groundZ = copy.groundZ;

            unknown04 = new byte[copy.unknown04.Length];
            Array.Copy(copy.unknown04, unknown04, copy.unknown04.Length);

            flags = copy.flags;
            negativeMeshPos = copy.negativeMeshPos;
            meshPos = copy.meshPos;

            vertices = new Vector3<float>[copy.vertices.Length];
            Array.Copy(copy.vertices, vertices, copy.vertices.Length);
            normals = new Vector3<float>[copy.normals.Length];
            Array.Copy(copy.normals, normals, copy.normals.Length);

            if (!flags.HasFlag(BrgMeshFlag.NOTFIRSTMESH) || flags.HasFlag(BrgMeshFlag.MOVINGTEX))
            {
                if (flags.HasFlag(BrgMeshFlag.TEXTURE))
                {
                    texVertices = new Vector2<float>[copy.texVertices.Length];
                    Array.Copy(copy.texVertices, texVertices, copy.texVertices.Length);
                }
            }

            if (!flags.HasFlag(BrgMeshFlag.NOTFIRSTMESH))
            {
                if (flags.HasFlag(BrgMeshFlag.MATERIALS))
                {
                    faceMaterials = new short[copy.faceMaterials.Length];
                    Array.Copy(copy.faceMaterials, faceMaterials, copy.faceMaterials.Length);
                }

                faceVertices = new Vector3<short>[copy.faceVertices.Length];
                Array.Copy(copy.faceVertices, faceVertices, copy.faceVertices.Length);

                if (flags.HasFlag(BrgMeshFlag.MATERIALS))
                {
                    vertMaterials = new short[copy.vertMaterials.Length];
                    Array.Copy(copy.vertMaterials, vertMaterials, copy.vertMaterials.Length);
                }
            }

            unknown09 = new int[copy.unknown09.Length];
            Array.Copy(copy.unknown09, unknown09, copy.unknown09.Length);
            animTime = copy.animTime;
            unknown09Const = copy.unknown09Const;
            checkSpace = copy.checkSpace;
            unknown09e = copy.unknown09e;

            if (checkSpace == 0)
            {
                unknown09b = copy.unknown09b;
                lenSpace = copy.lenSpace;
                unknown09d = copy.unknown09d;
            }

            // Implement this later
            //if (unknown05 == 97 || unknown06 == 200 || unknown06 == 204 || unknown06 == 72 || (unknown06 == 76 && unknown05 != 98)) {
            //    byte unknown0a[4 * numVertices];
            //}

            if (flags.HasFlag(BrgMeshFlag.ATTACHPOINTS))
            {
                numMatrix = copy.numMatrix;
                numIndex = copy.numIndex;
                unknown10 = copy.unknown10;

                attachpoints = new List<BrgAttachpoint>(copy.attachpoints.Count);
                for (int i = 0; i < copy.attachpoints.Count; i++)
                {
                    attachpoints.Add(new BrgAttachpoint(copy.attachpoints[i]));
                }

                if (checkSpace == 0 && lenSpace > 0)
                {
                    unknown14 = new float[copy.unknown14.Length];
                    Array.Copy(copy.unknown14, unknown14, copy.unknown14.Length);
                }
            }
        }

        public void Write(BrgBinaryWriter writer)
        {
            writer.Write(magic);

            writer.Write(meshFormat);
            writer.Write(meshFormat2);
            writer.Write(numVertices);
            writer.Write(numFaces);
            writer.Write(unknown02);

            for (int i = 0; i < 8; i++)
            {
                writer.Write(unknown03[i]);
            }
            writer.Write(groundZ);

            writer.Write(unknown04);

            writer.Write((Int16)flags);
            writer.WriteVector3(ref negativeMeshPos, true);
            writer.WriteVector3(ref meshPos, true);

            for (int i = 0; i < numVertices; i++)
            {
                writer.WriteVector3(ref vertices[i], true, true);
            }
            for (int i = 0; i < numVertices; i++)
            {
                writer.WriteVector3(ref normals[i], true, true);
            }

            if (!flags.HasFlag(BrgMeshFlag.NOTFIRSTMESH) || flags.HasFlag(BrgMeshFlag.MOVINGTEX))
            {
                if (flags.HasFlag(BrgMeshFlag.TEXTURE))
                {
                    for (int i = 0; i < numVertices; i++)
                    {
                        writer.WriteVector2(ref texVertices[i], true);
                    }
                }
            }

            if (!flags.HasFlag(BrgMeshFlag.NOTFIRSTMESH))
            {
                if (flags.HasFlag(BrgMeshFlag.MATERIALS))
                {
                    for (int i = 0; i < numFaces; i++)
                    {
                        writer.Write(faceMaterials[i]);
                    }
                }

                for (int i = 0; i < numFaces; i++)
                {
                    writer.WriteVector3(ref faceVertices[i]);
                }

                if (flags.HasFlag(BrgMeshFlag.MATERIALS))
                {
                    for (int i = 0; i < numVertices; i++)
                    {
                        writer.Write(vertMaterials[i]);
                    }
                }
            }

            for (int i = 0; i < 4; i++)
            {
                writer.Write(unknown09[i]);
            }
            writer.Write(animTime);
            writer.Write(unknown09Const);
            writer.Write(checkSpace);
            writer.Write(unknown09e);

            if (checkSpace == 0)
            {
                writer.Write(unknown09b);
                writer.Write(lenSpace);
                writer.Write(unknown09d);
            }

            // Implement this later
            //if (unknown05 == 97 || unknown06 == 200 || unknown06 == 204 || unknown06 == 72 || (unknown06 == 76 && unknown05 != 98)) {
            //    byte unknown0a[4 * numVertices];
            //}

            if (flags.HasFlag(BrgMeshFlag.ATTACHPOINTS))
            {
                numMatrix = (Int16)attachpoints.Count;
                writer.Write(numMatrix);

                List<int> nameId = new List<int>();
                int maxNameId = 0;
                for (int i = 0; i < numMatrix; i++)
                {
                    nameId.Add(attachpoints[i].NameId);
                    if (i == 0)
                    {
                        maxNameId = attachpoints[i].NameId;
                    }
                    else if (attachpoints[i].NameId > maxNameId)
                    {
                        maxNameId = attachpoints[i].NameId;
                    }
                }
                numIndex = (Int16)(55 - maxNameId);
                writer.Write(numIndex);
                writer.Write(unknown10);


                for (int i = 0; i < numMatrix; i++)
                {
                    writer.WriteVector3(ref attachpoints[i].x, true, true);
                }
                for (int i = 0; i < numMatrix; i++)
                {
                    writer.WriteVector3(ref attachpoints[i].y, true, true);
                }
                for (int i = 0; i < numMatrix; i++)
                {
                    writer.WriteVector3(ref attachpoints[i].z, true, true);
                }
                for (int i = 0; i < numMatrix; i++)
                {
                    writer.WriteVector3(ref attachpoints[i].position, true, true);
                }
                for (int i = 0; i < numMatrix; i++)
                {
                    writer.WriteVector3(ref attachpoints[i].unknown11a, true, true);
                }
                for (int i = 0; i < numMatrix; i++)
                {
                    writer.WriteVector3(ref attachpoints[i].unknown11b, true, true);
                }

                int[] dup = new int[numIndex];
                for (int i = 0; i < nameId.Count; i++)
                {
                    dup[nameId[i]] += 1;
                }
                int countId = 0;
                for (int i = 0; i < numIndex; i++)
                {
                    writer.Write(dup[i]);
                    if (dup[i] == 0)
                    {
                        writer.Write(0);
                    }
                    else
                    {
                        writer.Write(countId);
                    }
                    countId += dup[i];
                }

                List<int> nameId2 = new List<int>(nameId);
                nameId.Sort();
                for (int i = 0; i < numMatrix; i++)
                {
                    for (int j = 0; j < numMatrix; j++)
                    {
                        if (nameId[i] == nameId2[j])
                        {
                            nameId2[j] = -1;
                            writer.Write((byte)j);
                            break;
                        }
                    }
                }

                if (checkSpace == 0 && lenSpace > 0)
                {
                    for (int i = 0; i < lenSpace; i++)
                    {
                        writer.Write(unknown14[i]);
                    }
                }
            }
        }

        public void ReadBr3(BrgBinaryReader reader)
        {
            numVertices = reader.ReadInt16();
            numFaces = reader.ReadInt16();
            flags = (BrgMeshFlag)reader.ReadInt16();

            vertices = new Vector3<float>[numVertices];
            for (int i = 0; i < numVertices; i++)
            {
                reader.ReadVector3(out vertices[i], false, false);
            }
            normals = new Vector3<float>[numVertices];
            for (int i = 0; i < numVertices; i++)
            {
                reader.ReadVector3(out normals[i], false, false);
            }

            if (!flags.HasFlag(BrgMeshFlag.NOTFIRSTMESH) || flags.HasFlag(BrgMeshFlag.MOVINGTEX))
            {
                if (flags.HasFlag(BrgMeshFlag.TEXTURE))
                {
                    texVertices = new Vector2<float>[numVertices];
                    for (int i = 0; i < numVertices; i++)
                    {
                        reader.ReadVector2(out texVertices[i], false);
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
                        faceMaterials[i] = (Int16)reader.ReadInt32();
                    }
                }

                faceVertices = new Vector3<Int16>[numFaces];
                for (int i = 0; i < numFaces; i++)
                {
                    reader.ReadVector3(out faceVertices[i], true);
                    faceVertices[i].X -= 1;
                    faceVertices[i].Y -= 1;
                    faceVertices[i].Z -= 1;
                }

                if (flags.HasFlag(BrgMeshFlag.MATERIALS))
                {
                    vertMaterials = new Int16[numVertices];
                    for (int i = 0; i < numFaces; i++)
                    {
                        vertMaterials[faceVertices[i].X] = faceMaterials[i];
                        vertMaterials[faceVertices[i].Y] = faceMaterials[i];
                        vertMaterials[faceVertices[i].Z] = faceMaterials[i];
                    }
                }
            }

            animTime = ParentFile.AsetHeader.animTime;

            if (flags.HasFlag(BrgMeshFlag.ATTACHPOINTS))
            {
                numMatrix = (Int16)reader.ReadInt32();

                attachpoints = new List<BrgAttachpoint>(numMatrix);
                for (int i = 0; i < numMatrix; i++)
                {
                    attachpoints.Add(new BrgAttachpoint());
                    attachpoints[i].NameId = BrgAttachpoint.GetIdByName(reader.ReadString());

                    Vector3<float> x3, y3, z3;
                    reader.ReadVector3(out x3, true, false);
                    reader.ReadVector3(out y3, true, false);
                    reader.ReadVector3(out z3, true, false);
                    reader.ReadVector3(out attachpoints[i].position, false, false);

                    attachpoints[i].x.X = x3.Z;
                    attachpoints[i].x.Y = z3.Z;
                    attachpoints[i].x.Z = y3.Z;

                    attachpoints[i].y.X = x3.Y;
                    attachpoints[i].y.Y = z3.Y;
                    attachpoints[i].y.Z = y3.Y;

                    attachpoints[i].z.X = x3.X;
                    attachpoints[i].z.Y = z3.X;
                    attachpoints[i].z.Z = y3.X;
                }
            }
        }
        public void WriteBr3(BrgBinaryWriter writer)
        {
            writer.Write(numVertices);
            writer.Write(numFaces);
            writer.Write((Int16)flags);

            for (int i = 0; i < vertices.Length; i++)
            {
                writer.WriteVector3(ref vertices[i], false, false);
            }
            for (int i = 0; i < normals.Length; i++)
            {
                writer.WriteVector3(ref normals[i], false, false);
            }

            if (!flags.HasFlag(BrgMeshFlag.NOTFIRSTMESH) || flags.HasFlag(BrgMeshFlag.MOVINGTEX))
            {
                if (flags.HasFlag(BrgMeshFlag.TEXTURE))
                {
                    for (int i = 0; i < texVertices.Length; i++)
                    {
                        writer.WriteVector2(ref texVertices[i], false);
                    }
                }
            }

            if (!flags.HasFlag(BrgMeshFlag.NOTFIRSTMESH))
            {
                if (flags.HasFlag(BrgMeshFlag.MATERIALS))
                {
                    foreach (Int16 id in faceMaterials)
                    {
                        writer.Write((int)id);
                    }
                }

                foreach (Vector3<Int16> v in faceVertices)
                {
                    writer.Write((Int16)(v.X + 1));
                    writer.Write((Int16)(v.Y + 1));
                    writer.Write((Int16)(v.Z + 1));
                }
            }

            if (flags.HasFlag(BrgMeshFlag.ATTACHPOINTS))
            {
                writer.Write((int)attachpoints.Count);

                foreach (BrgAttachpoint att in attachpoints)
                {
                    writer.WriteString(att.Name);

                    writer.Write(att.z.X);
                    writer.Write(att.y.X);
                    writer.Write(att.x.X);

                    writer.Write(att.z.Z);
                    writer.Write(att.y.Z);
                    writer.Write(att.x.Z);

                    writer.Write(att.z.Y);
                    writer.Write(att.y.Y);
                    writer.Write(att.x.Y);

                    writer.WriteVector3(ref att.position, false, false);
                    //writer.Write(-att.position.X);
                    //writer.Write(-att.position.Z);
                    //writer.Write(att.position.Y);
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
        public Vector3<float> x;
        public Vector3<float> y;
        public Vector3<float> z;
        public Vector3<float> position;
        public Vector3<float> unknown11a;
        public Vector3<float> unknown11b;

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

        public BrgAttachpoint()
        {
            NameId = -1;
        }
        public BrgAttachpoint(BrgAttachpoint prev)
        {
            NameId = prev.NameId;
            x = prev.x;
            y = prev.y;
            z = prev.z;
            position = prev.position;
            unknown11a = prev.unknown11a;
            unknown11b = prev.unknown11b;
        }

        public static int GetIdByName(string name)
        {
            for (int i = 0; i < AttachpointNames.Length; i++)
            {
                if (AttachpointNames[i].Equals(name, StringComparison.Ordinal))
                {
                    return (54 - i);
                }
            }

            throw new Exception("Invalid Attachpoint Name Id!");
        }
    }

    public class BrgMaterial
    {
        int magic;
        public int id;
        public BrgMatFlag flags;
        int unknown01b;
        int nameLength;
        Vector3<float> color; //unknown02 [36 bytes]
        Vector3<float> specular;
        Vector3<float> reflection;
        Vector3<float> ambient; //unknown03 [12 bytes]
        string name;
        string name2;
        float unknown05;
        float alpha; //unknown04
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
            if (flags.HasFlag(BrgMatFlag.SOLIDCOLOR))
            {
                unknown05 = reader.ReadSingle();
            }
            if (flags.HasFlag(BrgMatFlag.TITANGATE))
            {
                name2 = reader.ReadString(reader.ReadInt32());
            }
            alpha = reader.ReadSingle();

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
        public BrgMaterial()
        {
            magic = 1280463949;
            id = 0;
            flags = 0;
            unknown01b = 0;
            nameLength = 0;
            color = new Vector3<float>();
            specular = new Vector3<float>();
            reflection = new Vector3<float>();
            ambient = new Vector3<float>();

            name = string.Empty;
            name2 = string.Empty;

            unknown05 = 1;

            alpha = 1;


            sfx = new List<BrgMatSFX>();
        }
        public BrgMaterial(BrgMaterial copy)
        {
            magic = copy.magic;

            id = copy.id;
            flags = copy.flags;
            unknown01b = copy.unknown01b;
            nameLength = copy.nameLength;
            color = copy.color;
            specular = copy.specular;
            reflection = copy.reflection;
            ambient = copy.ambient;

            name = copy.name;
            if (flags.HasFlag(BrgMatFlag.SOLIDCOLOR))
            {
                unknown05 = copy.unknown05;
            }
            if (flags.HasFlag(BrgMatFlag.TITANGATE))
            {
                name2 = copy.name2;
            }
            alpha = copy.alpha;

            if (flags.HasFlag(BrgMatFlag.SFX))
            {
                sfx = new List<BrgMatSFX>(copy.sfx.Count);
                for (int i = 0; i < copy.sfx.Count; i++)
                {
                    sfx.Add(copy.sfx[i]);
                }
            }
            else
            {
                sfx = new List<BrgMatSFX>();
            }
        }

        public void Write(BrgBinaryWriter writer)
        {
            writer.Write(magic);
            writer.Write(id);
            writer.Write((int)flags);

            writer.Write(unknown01b);
            writer.Write(nameLength);

            writer.WriteVector3(ref color);
            writer.WriteVector3(ref specular);
            writer.WriteVector3(ref reflection);
            writer.WriteVector3(ref ambient);

            writer.WriteString(name, 0);

            if (flags.HasFlag(BrgMatFlag.SOLIDCOLOR))
            {
                writer.Write(unknown05);
            }
            if (flags.HasFlag(BrgMatFlag.TITANGATE))
            {
                writer.WriteString(name2, 4);
            }

            writer.Write(alpha);

            if (flags.HasFlag(BrgMatFlag.SFX))
            {
                writer.Write((byte)sfx.Count);
                for (int i = 0; i < sfx.Count; i++)
                {
                    writer.Write(sfx[i].Id);
                    writer.WriteString(sfx[i].Name, 2);
                }
            }
        }

        public void ReadBr3(BrgBinaryReader reader)
        {
            id = reader.ReadInt32();
            flags = (BrgMatFlag)reader.ReadInt32();
            name = reader.ReadString((byte)0x0);
            nameLength = Encoding.UTF8.GetByteCount(name);
        }
        public void WriteBr3(BrgBinaryWriter writer)
        {
            writer.Write(id);
            writer.Write((int)flags);
            writer.WriteString(name);
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
        public void ReadVector3(out Vector3<float> v, bool isAom = true, bool isHalf = false)
        {
            if (isAom)
            {
                if (!isHalf)
                {
                    v.X = this.ReadSingle();
                    v.Y = this.ReadSingle();
                    v.Z = this.ReadSingle();
                }
                else
                {
                    v.X = this.ReadHalf();
                    v.Y = this.ReadHalf();
                    v.Z = this.ReadHalf();
                }
            }
            else
            {
                if (!isHalf)
                {
                    v.X = -this.ReadSingle();
                    v.Z = -this.ReadSingle();
                    v.Y = this.ReadSingle();
                }
                else
                {
                    v.X = -this.ReadHalf();
                    v.Z = -this.ReadHalf();
                    v.Y = this.ReadHalf();
                }
            }
        }
        public void ReadVector3(out Vector3<Int16> v, bool isAom = true)
        {
            if (isAom)
            {
                v.X = this.ReadInt16();
                v.Y = this.ReadInt16();
                v.Z = this.ReadInt16();
            }
            else
            {
                v.X = (Int16)(-this.ReadInt16());
                v.Z = (Int16)(-this.ReadInt16());
                v.Y = this.ReadInt16();
            }
        }
        #endregion

        #region ReadVector2
        public void ReadVector2(out Vector2<float> v, bool isHalf = false)
        {
            if (!isHalf)
            {
                v.X = this.ReadSingle();
                v.Y = this.ReadSingle();
            }
            else
            {
                v.X = this.ReadHalf();
                v.Y = this.ReadHalf();
            }
        }
        #endregion

        public float ReadHalf()
        {
            byte[] f = new byte[4];
            byte[] h = this.ReadBytes(2);
            f[2] = h[0];
            f[3] = h[1];
            return EndianBitConverter.Little.ToSingle(f, 0);
        }
        public string ReadString(byte terminator = 0x0)
        {
            string filename = "";
            List<byte> fnBytes = new List<byte>();
            byte filenameByte = this.ReadByte();
            while (filenameByte != terminator)
            {
                filename += (char)filenameByte;
                fnBytes.Add(filenameByte);
                filenameByte = this.ReadByte();
            }
            return Encoding.UTF8.GetString(fnBytes.ToArray());
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

        public void WriteHeader(ref BrgHeader header)
        {
            this.Write(header.magic);
            this.Write(header.unknown01);
            this.Write(header.numMaterials);
            this.Write(header.unknown02);
            this.Write(header.numMeshes);
            this.Write(header.space);
            this.Write(header.unknown03);
        }
        public void WriteAsetHeader(ref BrgAsetHeader header)
        {
            this.Write(header.magic);
            this.Write(header.numFrames);
            this.Write(header.frameStep);
            this.Write(header.animTime);
            this.Write(header.frequency);
            this.Write(header.spf);
            this.Write(header.fps);
            this.Write(header.space);
        }

        #region Vector3
        public void WriteVector3(ref Vector3<float> v, bool isAom = true, bool isHalf = false)
        {
            if (isAom)
            {
                if (!isHalf)
                {
                    this.Write(v.X);
                    this.Write(v.Y);
                    this.Write(v.Z);
                }
                else
                {
                    this.WriteHalf(v.X);
                    this.WriteHalf(v.Y);
                    this.WriteHalf(v.Z);
                }
            }
            else
            {
                if (!isHalf)
                {
                    this.Write(-v.X);
                    this.Write(-v.Z);
                    this.Write(v.Y);
                }
                else
                {
                    this.WriteHalf(-v.X);
                    this.WriteHalf(-v.Z);
                    this.WriteHalf(v.Y);
                }
            }
        }
        public void WriteVector3(ref Vector3<Int16> v, bool isAom = true)
        {
            if (isAom)
            {
                this.Write(v.X);
                this.Write(v.Y);
                this.Write(v.Z);
            }
            else
            {
                this.Write((Int16)(-v.X));
                this.Write((Int16)(-v.Z));
                this.Write(v.Y);
            }
        }
        #endregion
        #region Vector2
        public void WriteVector2(ref Vector2<float> v, bool isHalf = false)
        {
            if (!isHalf)
            {
                this.Write(v.X);
                this.Write(v.Y);
            }
            else
            {
                this.WriteHalf(v.X);
                this.WriteHalf(v.Y);
            }
        }
        #endregion

        public void WriteHalf(float half)
        {
            byte[] f = EndianBitConverter.Little.GetBytes(half);
            this.Write(f[2]);
            this.Write(f[3]);
        }
        public void WriteString(string str)
        {
            byte[] data = Encoding.UTF8.GetBytes(str);
            this.Write(data);
            this.Write((byte)0x0);
        }
        public void WriteString(string str, int lengthSize)
        {
            byte[] data = Encoding.UTF8.GetBytes(str);
            if (lengthSize == 2)
            {
                this.Write((Int16)data.Length);
            }
            else if (lengthSize == 4)
            {
                this.Write(data.Length);
            }
            this.Write(data);
        }
    }
}
