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

                Mesh = new List<BrgMesh>(Header.numMeshes);
                for (int i = 0; i < Header.numMaterials; i++)
                {
                    Mesh.Add(new BrgMesh());
                    Mesh[i].ReadBr3(reader);
                }

                Material = new List<BrgMaterial>();
                for (int i = 0; i < Header.numMaterials; i++)
                {
                    Material.Add(new BrgMaterial());
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
                    writer.Write(mesh.numVertices);
                    writer.Write(mesh.numFaces);
                    writer.Write((Int16)mesh.flags);

                    foreach (Vector3<float> v in mesh.vertices)
                    {
                        writer.Write(-v.X);
                        writer.Write(-v.Z);
                        writer.Write(v.Y);
                    }
                    foreach (Vector3<float> v in mesh.normals)
                    {
                        writer.Write(-v.X);
                        writer.Write(-v.Z);
                        writer.Write(v.Y);
                    }

                    if (!mesh.flags.HasFlag(BrgMeshFlag.NOTFIRSTMESH) || mesh.flags.HasFlag(BrgMeshFlag.MOVINGTEX))
                    {
                        if (mesh.flags.HasFlag(BrgMeshFlag.TEXTURE))
                        {
                            foreach (Vector2<float> v in mesh.texVertices)
                            {
                                writer.Write(v.X);
                                writer.Write(v.Y);
                            }
                        }
                    }

                    if (!mesh.flags.HasFlag(BrgMeshFlag.NOTFIRSTMESH))
                    {
                        if (mesh.flags.HasFlag(BrgMeshFlag.MATERIALS))
                        {
                            foreach (Int16 id in mesh.faceMaterials)
                            {
                                writer.Write((int)id);
                            }
                        }

                        foreach (Vector3<Int16> v in mesh.faceVertices)
                        {
                            writer.Write((Int16)(v.Y + 1));
                            writer.Write((Int16)(v.X + 1));
                            writer.Write((Int16)(v.Z + 1));
                        }
                    }

                    if (mesh.flags.HasFlag(BrgMeshFlag.ATTACHPOINTS))
                    {
                        writer.Write((int)mesh.attachpoints.Count);

                        foreach (BrgAttachpoint att in mesh.attachpoints)
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

                            writer.Write(-att.position.X);
                            writer.Write(-att.position.Z);
                            writer.Write(att.position.Y);
                        }
                    }
                }

                foreach (BrgMaterial mat in Material)
                {
                    writer.Write(mat.id);
                    writer.Write((int)mat.flags);
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
        public Vector3<float>[] vertices;
        public Vector3<float>[] normals;

        public Vector2<float>[] texVertices;
        public Int16[] faceMaterials;
        public Vector3<Int16>[] faceVertices;
        Int16[] vertMaterials;

        float[] unknown09;
        Int16 checkSpace; //09a
        Int16 unknown09e;
        float unknown09b;
        int lenSpace; //09c
        int unknown09d;

        public Int16 numMatrix;
        Int16 numIndex;
        Int16 unknown10;
        public List<BrgAttachpoint> attachpoints;
        float[] unknown14;

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

            unknown09 = new float[12];
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
        public BrgMesh()
        {
            magic = 1230193997; //MESI
            meshFormat = 22;
            unknown01b = 12;
            numVertices = 0;
            numFaces = 0;
            unknown02 = 256;
            unknown03 = new float[9];
            unknown04 = new byte[6];
            unknown04[4] = 40;
            flags = 0;
            unknown07 = new float[3];
            meshX = 0f;
            meshY = 0f;
            meshZ = 0f;
            vertices = new Vector3<float>[numVertices];
            normals = new Vector3<float>[numVertices];
            texVertices = new Vector2<float>[numVertices];
            faceMaterials = new Int16[numFaces];
            faceVertices = new Vector3<Int16>[numFaces];
            vertMaterials = new Int16[numVertices];

            unknown09 = new float[12];
            checkSpace = 0;
            unknown09e = 0;
            unknown09b = 1;
            lenSpace = 0;
            unknown09d = 2;

            // Implement this later
            //if (unknown05 == 97 || unknown06 == 200 || unknown06 == 204 || unknown06 == 72 || (unknown06 == 76 && unknown05 != 98)) {
            //    byte unknown0a[4 * numVertices];
            //}

            numMatrix = 0;
            numIndex = 0;
            unknown10 = 1;
            attachpoints = new List<BrgAttachpoint>(numMatrix);
            unknown14 = new float[lenSpace];
        }

        public void Write(BrgBinaryWriter writer)
        {
            writer.Write(magic);

            writer.Write(meshFormat);
            writer.Write(unknown01b);
            writer.Write(numVertices);
            writer.Write(numFaces);
            writer.Write(unknown02);

            for (int i = 0; i < 9; i++)
            {
                writer.Write(unknown03[i]);
            }

            writer.Write(unknown04);

            writer.Write((Int16)flags);
            for (int i = 0; i < 3; i++)
            {
                writer.Write(unknown07[i]);
            }
            writer.Write(meshX);
            writer.Write(meshY);
            writer.Write(meshZ);

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

            for (int i = 0; i < 12; i++)
            {
                writer.WriteHalf(unknown09[i]);
            }
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
                    if (attachpoints[i].NameId > maxNameId)
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
                reader.ReadVector3(out vertices[i], false);
            }
            normals = new Vector3<float>[numVertices];
            for (int i = 0; i < numVertices; i++)
            {
                reader.ReadVector3(out normals[i], false);
            }

            if (!flags.HasFlag(BrgMeshFlag.NOTFIRSTMESH) || flags.HasFlag(BrgMeshFlag.MOVINGTEX))
            {
                if (flags.HasFlag(BrgMeshFlag.TEXTURE))
                {
                    texVertices = new Vector2<float>[numVertices];
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
                        faceMaterials[i] = (Int16)reader.ReadInt32();
                    }
                }

                faceVertices = new Vector3<Int16>[numFaces];
                for (int i = 0; i < numFaces; i++)
                {
                    reader.ReadVector3(out faceVertices[i]);
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

            if (flags.HasFlag(BrgMeshFlag.ATTACHPOINTS))
            {
                numMatrix = (Int16)reader.ReadInt32();

                attachpoints = new List<BrgAttachpoint>(numMatrix);
                for (int i = 0; i < attachpoints.Count; i++)
                {
                    attachpoints.Add(new BrgAttachpoint());
                    attachpoints[i].NameId = BrgAttachpoint.GetIdByName(reader.ReadString());

                    Vector3<float> x3, y3, z3;
                    reader.ReadVector3(out x3);
                    reader.ReadVector3(out y3);
                    reader.ReadVector3(out z3);
                    reader.ReadVector3(out attachpoints[i].position, false);

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
                    return i;
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
            alpha = 1;

            unknown05 = 1;

            sfx = new List<BrgMatSFX>();
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

            writer.WriteString(name, false);
            writer.Write(alpha);

            if (flags.HasFlag(BrgMatFlag.SOLIDCOLOR))
            {
                writer.Write(unknown05);
            }

            if (flags.HasFlag(BrgMatFlag.SFX))
            {
                writer.Write((byte)sfx.Count);
                for (int i = 0; i < sfx.Count; i++)
                {
                    writer.Write(sfx[i].Id);
                    writer.Write(sfx[i].Name);
                }
            }
        }

        public void ReadBr3(BrgBinaryReader reader)
        {
            id = reader.ReadInt32();
            flags = (BrgMatFlag)reader.ReadInt32();
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
        public void WriteString(string str, bool writeLength)
        {
            byte[] data = Encoding.UTF8.GetBytes(str);
            if (writeLength)
                this.Write((Int16)data.Length);
            this.Write(data);
        }
    }
}
