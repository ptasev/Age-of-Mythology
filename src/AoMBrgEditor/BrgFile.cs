using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*
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
        NONE1 =         0x8000,
        TRANSPCOLOR =   0x4000,
        NONE2 =         0x2000,
        NONE3 =         0x1000,
        MOVINGTEX =     0x0800,
        NOTFIRSTMESH =  0x0400,
        NONE4 =         0x0200,
        ATTACHPOINTS =  0x0100,
        NONE5 =         0x0080,
        MATERIALS =     0x0040,
        CHANGINGCOL =   0x0020,
        NONE7 =         0x0010,
        NONE8 =         0x0008,
        NONE9 =         0x0004,
        TEXTURE =       0x0002,
        VERTCOLOR =     0x0001
    };
    [Flags]
    public enum BrgMeshFormat : ushort
    {
        NOLOOPANIMATE   = 0x0020, // don't animate Last-First frame
        FORMATNONE1     = 0x0010, // haven't seen used
        ANIMATED        = 0x0008, // maybe means Animated
        HASMATERIAL     = 0x0004, // uses materials
        ANIMATEDUV      = 0x0002, // Animated UV
        ROTATE          = 0x0001  // rotates with the player view
    };
    public enum BrgMeshProperty : uint
    {
        FOLLOWGROUND = 1,
        VARIABLEANIM = 256
    };
    [Flags]
    public enum BrgMatFlag1
    {
        SFX =           0x1C000000,
        GLOW =          0x00200000,
        MATNONE1 =      0x00800000,
        PLAYERCOLOR =   0x00040000,
        SOLIDCOLOR =    0x00020000,
        TITANGATE =     0x00008300,
        MATTEXTURE =    0x00000030
    };

    [Flags]
    public enum BrgMatFlag : uint
    {
        SFX =               0x1C000000,
        NOREFLECT =         0x10000000, // Don't use the VectorFloat reflection
        MATNONE10 =         0x08000000, // no idea
        REFLECTTEX =        0x04000000, // use a reflection texture
        DARKPLAYERCOLOR =   0x02000000, // darker player color?
        MATNONE12 =         0x01000000, // low player color overlay for faces
        MATNONE1 =          0x00800000, // use texture, no idea
        MATNONE13 =         0x00400000, // black, stay with highlight
        GLOW =              0x00200000, // white, stay with highlight
        MATNONE14 =         0x00100000, // white, except for highlight
        MATNONE15 =         0x00080000, // fuller player color
        PLAYERCOLOR =       0x00040000, // default player color
        SOLIDCOLOR =        0x00020000, // also use specular level var (unknown05)
        MATNONE16 =         0x00010000, // no idea
        TITANGATE =         0x00008300, // 
        MATNONE17 =         0x00008000, // use texture for something
        MATNONE18 =         0x00004000, // ground texture?
        MATNONE19 =         0x00002000, // smooth/ambient?
        MATNONE20 =         0x00001000, // low player color overlay
        MATNONE21 =         0x00000800, // high player color overlay
        MATNONE22 =         0x00000400, // 
        MATNONE23 =         0x00000200, // does nothing?
        MATNONE24 =         0x00000100, // 
        MATTEXTURE =        0x00000030
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
                    //Mesh[i].Write(writer);
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
                    //Mesh.Add(new BrgMesh(Mesh[lastFrameIndex]));
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
        public Int16 version;
        public BrgMeshFormat format;
        public Int16 numVertices;
        public Int16 numFaces;
        public BrgMeshProperty properties;
        Vector3<float> hitboxPos;
        public float unknown03;
        public Vector3<float> unknown03Const;
        Vector3<float> groundPos;
        public Int16 unknown04;
        public BrgMeshFlag flags;
        Vector3<float> negativeMeshPos;
        Vector3<float> meshPos;
        public Vector3<float>[] vertices;
        public Vector3<float>[] normals;

        public Vector2<float>[] texVertices;
        public Int16[] faceMaterials;
        public Vector3<Int16>[] faceVertices;
        Int16[] vertMaterials;

        public Int16 numIndex0;
        public Int16 numMatrix0;
        public int unknown091;
        public int unknown09Unused;
        public int lastMaterialIndex;
        public float animTime;
        public int unknown09Const;
        public Int16 checkSpace; //09a
        public float unknown09e;
        public float animTimeMult;
        public int lenSpace; //09c
        public int numMaterialsUsed;

        public Vector2<float>[] unknown0a;

        //public Int16 numMatrix;
        //Int16 numIndex;
        public Int16 unknown10;
        public List<BrgAttachpoint> attachpoints;
        float[] animTimeAdjust;

        public BrgMesh(BrgBinaryReader reader, BrgFile file)
        {
            ParentFile = file;
            magic = reader.ReadInt32();
            if (magic != EndianBitConverter.Little.ToInt32(Encoding.UTF8.GetBytes("MESI"), 0))
            {
                throw new Exception("Improper mesh header!");
            }

            version = reader.ReadInt16();
            format = (BrgMeshFormat)reader.ReadInt16();
            numVertices = reader.ReadInt16();
            numFaces = reader.ReadInt16();
            properties = (BrgMeshProperty)reader.ReadInt32();

            reader.ReadVector3(out hitboxPos, true, false);
            unknown03 = reader.ReadSingle();
            reader.ReadVector3(out unknown03Const, true, false);
            reader.ReadVector3(out groundPos, true, false);

            unknown04 = reader.ReadInt16();

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

            numIndex0 = reader.ReadInt16();
            numMatrix0 = reader.ReadInt16();
            unknown091 = reader.ReadInt32();
            unknown09Unused = reader.ReadInt32();
            lastMaterialIndex = reader.ReadInt32();
            animTime = reader.ReadSingle();
            unknown09Const = reader.ReadInt32();
            checkSpace = reader.ReadInt16(); //09a
            unknown09e = reader.ReadHalf();

            if (checkSpace == 0)
            {
                animTimeMult = reader.ReadSingle();
                lenSpace = reader.ReadInt32(); //09c
                numMaterialsUsed = reader.ReadInt32();
            }

            if (((flags.HasFlag(BrgMeshFlag.TRANSPCOLOR) || flags.HasFlag(BrgMeshFlag.CHANGINGCOL)) && !flags.HasFlag(BrgMeshFlag.NOTFIRSTMESH))
                || flags.HasFlag(BrgMeshFlag.VERTCOLOR))
            {
                unknown0a = new Vector2<float>[numVertices];
                for (int i = 0; i < numVertices; i++)
                {
                    reader.ReadVector2(out unknown0a[i], true);
                }
            }

            if (flags.HasFlag(BrgMeshFlag.ATTACHPOINTS))
            {
                Int16 numMatrix = reader.ReadInt16();
                Int16 numIndex = reader.ReadInt16();
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
                    animTimeAdjust = new float[lenSpace];
                    for (int i = 0; i < lenSpace; i++)
                    {
                        animTimeAdjust[i] = reader.ReadSingle();
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
                Int16 numMatrix = (Int16)reader.ReadInt32();

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
        public int unknown01b;
        int nameLength;
        Vector3<float> color; //unknown02 [36 bytes]
        Vector3<float> specular;
        Vector3<float> reflection;
        Vector3<float> ambient; //unknown03 [12 bytes]
        public string name;
        string name2;
        float specularLevel;
        float alphaOpacity; //unknown04
        public List<BrgMatSFX> sfx;

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
                specularLevel = reader.ReadSingle();
            }
            if (flags.HasFlag(BrgMatFlag.TITANGATE))
            {
                name2 = reader.ReadString(reader.ReadInt32());
            }
            alphaOpacity = reader.ReadSingle();

            if (flags.HasFlag(BrgMatFlag.REFLECTTEX))
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

            specularLevel = 1;

            alphaOpacity = 1;


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
                specularLevel = copy.specularLevel;
            }
            if (flags.HasFlag(BrgMatFlag.TITANGATE))
            {
                name2 = copy.name2;
            }
            alphaOpacity = copy.alphaOpacity;

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
                writer.Write(specularLevel);
            }
            if (flags.HasFlag(BrgMatFlag.TITANGATE))
            {
                writer.WriteString(name2, 4);
            }

            writer.Write(alphaOpacity);

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
*/