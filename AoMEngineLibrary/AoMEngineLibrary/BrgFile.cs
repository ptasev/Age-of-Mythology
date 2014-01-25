using System;
using System.Collections.Generic;
using System.Text;
using MiscUtil.Conversion;
using MiscUtil.IO;
using ManagedServices;
using MiscUtil;
using System.Drawing;
using System.IO;

namespace AoMEngineLibrary
{
    public struct Vector3<T>
    {
        public T X;
        public T Y;
        public T Z;

        public Vector3(T value)
        {
            this.X = value;
            this.Y = value;
            this.Z = value;
        }
        public Vector3(T x, T y, T z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public T LongestAxisLength()
        {
            if (Operator.LessThan<T>(X, Y))
            {
                if (Operator.LessThan<T>(Y, Z))
                {
                    return Z;
                }
                else
                {
                    return Y;
                }
            }
            else
            {
                if (Operator.LessThan<T>(X, Z))
                {
                    return Z;
                }
                else
                {
                    return X;
                }
            }
        }

        public static Vector3<T> operator -(Vector3<T> one, Vector3<T> two)
        {
            return new Vector3<T>(Operator.Subtract<T>(one.X, two.X), Operator.Subtract<T>(one.Y, two.Y), Operator.Subtract<T>(one.Z, two.Z));
        }
        public static Vector3<T> operator +(Vector3<T> one, Vector3<T> two)
        {
            return new Vector3<T>(Operator.Add<T>(one.X, two.X), Operator.Add<T>(one.Y, two.Y), Operator.Add<T>(one.Z, two.Z));
        }
        public static Vector3<T> operator /(Vector3<T> one, T two)
        {
            return new Vector3<T>(Operator.Divide<T>(one.X, two), Operator.Divide<T>(one.Y, two), Operator.Divide<T>(one.Z, two));
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
    public enum BrgMeshFormat : ushort
    {
        NOLOOPANIMATE = 0x0020, // don't animate Last-First frame
        FORMATNONE1 = 0x0010, // haven't seen used
        ANIMATED = 0x0008, // maybe means Animated
        HASMATERIAL = 0x0004, // uses materials
        ANIMATEDUV = 0x0002, // Animated UV
        ROTATE = 0x0001  // rotates with the player view
    };
    public enum BrgMeshProperty : uint
    {
        FOLLOWTERRAIN = 1,
        VARIABLEANIM = 256
    };
    [Flags]
    public enum BrgMatFlag1
    {
        SFX = 0x1C000000,
        GLOW = 0x00200000,
        MATNONE1 = 0x00800000,
        PLAYERCOLOR = 0x00040000,
        SOLIDCOLOR = 0x00020000,
        TITANGATE = 0x00008300,
        MATTEXTURE = 0x00000030
    };
    [Flags]
    public enum BrgMatFlag : uint
    {
        ILLUMREFLECTION = 0x10000000, // Don't use the VectorFloat specular?
        MATNONE10 = 0x08000000, // no idea
        REFLECTIONTEXTURE = 0x04000000, // use a reflection texture
        PLAYERCOLOR2 = 0x02000000, // darker player color?
        LOWPLAYERCOLOR2 = 0x01000000, // low player color overlay for faces
        DIFFUSETEXTURE = 0x00800000, // use texture, no idea
        USESELFILLUMCOLOR = 0x00400000, // black, stay with highlight
        WHITESELFILLUMCOLOR = 0x00200000, // white, stay with highlight
        MATNONE14 = 0x00100000, // white, except for highlight
        PLAYERCOLOR4 = 0x00080000, // fuller player color
        PLAYERCOLOR = 0x00040000, // default player color
        USESPECLVL = 0x00020000, // also use specular level var
        MATNONE16 = 0x00010000, // no idea
        MATTEXURE2 = 0x00008000, // use texture for something
        MATNONE18 = 0x00004000, // ground texture?
        MATNONE19 = 0x00002000, // smooth/ambient?
        LOWPLAYERCOLOROVERLAY = 0x00001000, // low player color overlay
        PLAYERCOLOROVERWHITE = 0x00000800, // high player color overlay
        MATNONE22 = 0x00000400, // 
        MATNONE23 = 0x00000200, // does nothing?
        MATNONE24 = 0x00000100, // 
        MATNONE25 = 0x00000030
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

    public static class Maxscript
    {
        public static bool Execute = true;
        public static bool OutputCommands = false;
        public static List<string> Output
        {
            get
            {
                return output;
            }
        }
        private static List<string> output = new List<string>();

        public enum QueryType { Integer, Float, String, Boolean, Color };
        public static Int32 QueryInteger(string command, params object[] args)
        {
            return (Int32)Query(command, QueryType.Integer, args);
        }
        public static float QueryFloat(string command, params object[] args)
        {
            return (float)Query(command, QueryType.Float, args);
        }
        public static Boolean QueryBoolean(string command, params object[] args)
        {
            return (Boolean)Query(command, QueryType.Boolean, args);
        }
        public static object Query(string command, QueryType qType, params object[] args)
        {
            string formatCommand = MaxscriptSDK.AssembleScript(command, args);
            //string formatCommand = String.Format(command, args);
            if (qType == QueryType.Integer)
            {
                return MaxscriptSDK.ExecuteIntMaxscriptQuery(formatCommand);
            }
            else if (qType == QueryType.Float)
            {
                return MaxscriptSDK.ExecuteFloatMaxscriptQuery(formatCommand);
            }
            else if (qType == QueryType.String)
            {
                return MaxscriptSDK.ExecuteStringMaxscriptQuery(formatCommand);
            }
            else if (qType == QueryType.Boolean)
            {
                return MaxscriptSDK.ExecuteBooleanMaxscriptQuery(formatCommand);
            }
            else
            {
                return MaxscriptSDK.ExecuteColorMaxscriptQuery(formatCommand);
            }
        }

        public static void CommentTitle(string name)
        {
            if (OutputCommands)
            {
                output.Add("--######################################## " + name + " ########################################");
            }
        }
        public static void AtTime(float time, string command, params object[] args)
        {
            Command("at time " + time + "s (" + command + ")", args);
        }
        public static void Animate(string command, params object[] args)
        {
            Command("with animate on (" + command + ")", args);
        }
        public static void AnimateAtTime(float time, string command, params object[] args)
        {
            Command("with animate on (at time " + time + "s (" + command + "))", args);
        }
        public static void Append(string name, object arg)
        {
            Command("append {0} {1}", name, arg);
        }
        public static string NewDummy(string varName, string name, string rotate, string position, string boxSize, string scale)
        {
            Command("{0} = dummy name:\"{1}\" rotation:({2} as quat) position:{3} boxsize:{4} scale:{5}", varName, name, rotate, position, boxSize, scale);
            return varName;
        }
        public static string SnapshotAsMesh(string varName, string varNode, float time)
        {
            Command("at time {0}s ({1} = snapshotAsMesh {2})", time, varName, varNode);
            return varName;
        }
        public static string NewMeshLiteral(string name, string vertArray, string normArray, string faceArray, string faceMatIdArray, string texVertArray)
        {
            return MaxscriptSDK.AssembleScript("mesh name:\"{0}\" vertices:{1} normals:{2} faces:{3} materialIDs:{4} tverts:{5}", name, vertArray, normArray, faceArray, faceMatIdArray, texVertArray);
        }
        public static string NewMesh(string name, string vertArray, string normArray, string faceArray, string faceMatIdArray, string texVertArray)
        {
            Command("{0} = mesh vertices:{1} normals:{2} faces:{3} materialIDs:{4} tverts:{5}", name, vertArray, normArray, faceArray, faceMatIdArray, texVertArray);
            return name;
        }

        public static string NewMatrix3(string name, string xVector, string yVector, string zVector, string posVector)
        {
            Command("{0} = matrix3 {1} {2} {3} {4}", name, xVector, yVector, zVector, posVector);
            return name;
        }
        #region Point3
        public static string NewPoint3<T>(string name, ref Vector3<T> vector)
        {
            Command("{0} = [{1}, {2}, {3}]", name, vector.X, vector.Y, vector.Z);
            return name;
        }
        public static string NewPoint3Literal<T>(T X, T Y, T Z)
        {
            return MaxscriptSDK.AssembleScript("[{0}, {1}, {2}]", X, Y, Z);
        }
        public static string NewPoint3<T>(string name, T X, T Y, T Z)
        {
            Command("{0} = [{1}, {2}, {3}]", name, X, Y, Z);
            return name;
        }
        #endregion
        public static string NewArray(string name)
        {
            Command("{0} = #()", name);
            return name;
        }
        public static void Interval(float begin, float end)
        {
            Command("animationRange = interval {0} {1}s", begin, end);
        }
        public static string Command(string command, params object[] args)
        {
            string formatCommand = MaxscriptSDK.AssembleScript(command, args);
            //string formatCommand = String.Format(command, args);
            if (Execute)
            {
                MaxscriptSDK.ExecuteMaxscriptCommand(formatCommand);
            }
            if (OutputCommands)
            {
                output.Add(formatCommand);
            }
            return formatCommand;
        }
    }

    public class BrgFile
    {
        public MaxPlugin ParentForm1;
        public MaxPluginForm ParentForm;

        public BrgHeader Header;
        public BrgAsetHeader AsetHeader;
        public List<BrgMesh> Mesh;
        public List<BrgMaterial> Material;

        public BrgFile(System.IO.Stream fileStream, MaxPluginForm form)
            : this(fileStream)
        {
            ParentForm = form;
        }
        public BrgFile(System.IO.Stream fileStream, MaxPlugin form)
            : this(fileStream)
        {
            ParentForm1 = form;
        }
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
                    BrgMaterial mat = new BrgMaterial(reader, this);
                    if (!ContainsMaterialID(mat.id))
                    {
                        Material.Add(mat);
                    }
                    //Material.Add(new BrgMaterial(reader, this));
                }

                if (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    throw new Exception("The end of stream was not reached!");
                }
            }
        }
        public BrgFile(MaxPluginForm form) 
        {
            ParentForm = form;
            Header.magic = 1196310850;
            Header.unknown03 = 1999922179;
            AsetHeader.magic = 1413829441;

            Mesh = new List<BrgMesh>();
            Material = new List<BrgMaterial>();
        }

        public void Write(System.IO.Stream fileStream)
        {
            using (BrgBinaryWriter writer = new BrgBinaryWriter(new LittleEndianBitConverter(), fileStream))
            {
                Header.numMeshes = Mesh.Count;
                Header.numMaterials = Material.Count;
                writer.WriteHeader(ref Header);

                if (Header.numMeshes > 1)
                {
                    updateAsetHeader();
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

        public void ExportToMax()
        {
            if (Mesh.Count > 1)
            {
                Maxscript.Command("frameRate = {0}", Math.Round(Mesh.Count / Mesh[0].animTime));
                Maxscript.Interval(0, Mesh[0].animTime);
            }
            else
            {
                Maxscript.Command("frameRate = 1");
                Maxscript.Interval(0, 1);
            }

            if (Mesh.Count > 0)
            {
                string mainObject = Mesh[0].ExportToMax();
                //string firstFrameObject = MaxHelper.SnapshotAsMesh("firstFrameObject", mainObject, 0);

                for (int i = 1; i < Mesh.Count; i++)
                {
                    Maxscript.CommentTitle("ANIMATE FRAME " + i);
                    float time = GetFrameTime(i);

                    for (int j = 0; j < Mesh[i].vertices.Length; j++)
                    {
                        //Vector3<float> movePos = Mesh[i].vertices[j] - Mesh[i - 1].vertices[j];
                        //string moveVector = MaxHelper.NewPoint3<float>("moveVector", -movePos.X, -movePos.Z, movePos.Y);
                        //string moveVector = MaxHelper.NewPoint3<float>("moveVector", -Mesh[i].vertices[j].X, -Mesh[i].vertices[j].Z, Mesh[i].vertices[j].Y);
                        Maxscript.AnimateAtTime(time, "meshOp.setVert {0} {1} {2}", mainObject, j + 1, Maxscript.NewPoint3Literal<float>(-Mesh[i].vertices[j].X, -Mesh[i].vertices[j].Z, Mesh[i].vertices[j].Y));

                        // When NOTFIRSTMESH (i aleady starts from 1)
                        if (Mesh[i].flags.HasFlag(BrgMeshFlag.MOVINGTEX))
                        {
                            if (Mesh[i].flags.HasFlag(BrgMeshFlag.TEXTURE))
                            {
                                //string tVertVector = MaxHelper.NewPoint3<float>("tVertVector", Mesh[i].texVertices[j].X, Mesh[i].texVertices[j].Y, 0);
                                //MaxHelper.Execute = false;
                                //string tVertCommand = MaxHelper.Command("setTVert {0}.mesh {1} {2}", mainObject, j + 1, tVertVector);
                                //MaxHelper.Execute = true;
                                //MaxHelper.AnimateAtTime(time, tVertCommand);
                                Maxscript.Animate("{0}.Unwrap_UVW.SetVertexPosition {1}s {2} {3}", mainObject, time, j + 1, Maxscript.NewPoint3Literal<float>(Mesh[i].texVertices[j].X, Mesh[i].texVertices[j].Y, 0));
                            }
                        }
                    }

                    for (int j = 0; j < Mesh[i].attachpoints.Count; j++)
                    {
                        Maxscript.Command("attachpoint = getNodeByName \"{0}\"", Mesh[i].attachpoints[j].GetMaxName(j));
                        Maxscript.Command("with animate on (at time {0}s (attachpoint.rotation = {1}))", time, Mesh[i].attachpoints[j].GetMaxTransform());
                        Maxscript.Command("with animate on (at time {0}s (attachpoint.position = {1}))", time, Mesh[i].attachpoints[j].GetMaxPosition());
                        Maxscript.Command("with animate on (at time {0}s (attachpoint.scale = {1}))", time, Mesh[i].attachpoints[j].GetMaxScale());
                    }
                }

                // Still can't figure out why it updates/overwrites normals ( geometry:false topology:false)
                Maxscript.Command("update {0}", mainObject);

                if (Material.Count > 0)
                {
                    Maxscript.CommentTitle("LOAD MATERIALS");
                    Maxscript.Command("matGroup = multimaterial numsubs:{0}", Material.Count);
                    for (int i = 0; i < Material.Count; i++)
                    {
                        Maxscript.Command("matGroup[{0}] = {1}", i + 1, Material[i].ExportToMax());
                        Maxscript.Command("matGroup.materialIDList[{0}] = {1}", i + 1, Material[i].id);
                    }
                    Maxscript.Command("{0}.material = matGroup", mainObject);
                }
            }
        }
        public void ImportFromMax(bool execute)
        {
            string mainObject = "mainObject";
            Maxscript.Command("{0} = selection[1]", mainObject);
            bool objectSelected = Maxscript.QueryBoolean("classOf {0} == Editable_mesh", mainObject);
            if (!objectSelected)
            {
                throw new Exception("No object selected!");
            }
            if (ParentForm.Flags.HasFlag(BrgMeshFlag.MOVINGTEX))
            {
                Header.numMeshes = Maxscript.QueryInteger("{0}.Unwrap_UVW[1].keys.count", mainObject);
            }
            else
            {
                Header.numMeshes = (int)Maxscript.Query("{0}.mesh[1].keys.count", Maxscript.QueryType.Integer, mainObject);
            }
            Header.numMaterials = (int)Maxscript.Query("{0}.material.materialList.count", Maxscript.QueryType.Integer, mainObject);
            //System.Windows.Forms.MessageBox.Show(Header.numMeshes + " " + Header.numMaterials);

            if (Header.numMaterials > 0)
            {
                Material = new List<BrgMaterial>(Header.numMaterials);
                for (int i = 0; i < Header.numMaterials; i++)
                {
                    Material.Add(new BrgMaterial(this));
                    Material[i].ImportFromMax(mainObject, i);
                }
            }

            if (Header.numMeshes <= 0)
            {
                Header.numMeshes = 1;
            }
            updateAsetHeader();

            Mesh = new List<BrgMesh>(Header.numMeshes);
            for (int i = 0; i < Header.numMeshes; i++)
            {
                Mesh.Add(new BrgMesh(this));
                Mesh[i].ImportFromMax(mainObject, i);//, flags, format, properties);
            }

            if (Mesh[0].properties.HasFlag(BrgMeshProperty.VARIABLEANIM))
            {
                Mesh[0].animTimeAdjust = new float[Mesh.Count];
                for (int i = 0; i < Mesh.Count; i++)
                {
                    Mesh[0].animTimeAdjust[i] = Maxscript.QueryFloat("{0}.mesh[1].keys[{1}].time as float", mainObject, i + 1) / 4800f / Mesh[0].animTime;
                }
            }
        }

        public float GetFrameTime(int meshIndex)
        {
            if (Mesh[0].properties.HasFlag(BrgMeshProperty.VARIABLEANIM))
            {
                //System.Windows.Forms.MessageBox.Show("t1");
                return Mesh[0].animTimeAdjust[meshIndex] * Mesh[0].animTime;
            }
            else if (Mesh.Count > 1)
            {
                //System.Windows.Forms.MessageBox.Show("t2");
                return (float)meshIndex / ((float)Mesh.Count - 1f) * Mesh[0].animTime;
            }
            else
            {
                //System.Windows.Forms.MessageBox.Show("t3");
                return 0;
            }
        }
        public bool ContainsMaterialID(int id)
        {
            for (int i = 0; i < Material.Count; i++)
            {
                if (Material[i].id == id)
                {
                    return true;
                }
            }

            return false;
        }
        private void updateAsetHeader()
        {
            AsetHeader.numFrames = Mesh.Count;
            AsetHeader.frameStep = 1f / (float)AsetHeader.numFrames;
            AsetHeader.animTime = Mesh[0].animTime;
            AsetHeader.frequency = 1f / (float)AsetHeader.animTime;
            AsetHeader.spf = AsetHeader.animTime / (float)AsetHeader.numFrames;
            AsetHeader.fps = (float)AsetHeader.numFrames / AsetHeader.animTime;
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
        //public Int16 numVertices;
        //public Int16 numFaces;
        public BrgMeshProperty properties;
        Vector3<float> centerPos;
        public float unknown03;
        public Vector3<float> position;
        Vector3<float> groundPos;
        public Int16 unknown04;
        public BrgMeshFlag flags;
        Vector3<float> boundingBoxMin;
        Vector3<float> boundingBoxMax;
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
        //public Int16 checkSpace; //09a
        public float unknown09e;
        public float animTimeMult;
        //public int lenSpace; //09c
        public int numMaterialsUsed;

        public Vector2<float>[] unknown0a;

        //public Int16 numMatrix;
        //Int16 numIndex;
        //public Int16 unknown10;
        public List<BrgAttachpoint> attachpoints;
        public float[] animTimeAdjust;

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
            Int16 numVertices = reader.ReadInt16();
            Int16 numFaces = reader.ReadInt16();
            properties = (BrgMeshProperty)reader.ReadInt32();

            reader.ReadVector3(out centerPos, true, false);
            unknown03 = reader.ReadSingle();
            reader.ReadVector3(out position, true, false);
            reader.ReadVector3(out groundPos, true, false);

            unknown04 = reader.ReadInt16();

            flags = (BrgMeshFlag)reader.ReadInt16();
            reader.ReadVector3(out boundingBoxMin, true);
            reader.ReadVector3(out boundingBoxMax, true);

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

            texVertices = new Vector2<float>[0];
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

            faceMaterials = new Int16[0];
            faceVertices = new Vector3<short>[0];
            vertMaterials = new Int16[0];
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
            reader.ReadInt16(); //09a
            unknown09e = reader.ReadHalf();
            animTimeMult = reader.ReadSingle();
            int lenSpace = reader.ReadInt32(); //09c
            numMaterialsUsed = reader.ReadInt32();

            unknown0a = new Vector2<float>[0];
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
                reader.ReadInt16();

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
            }
            else
            {
                attachpoints = new List<BrgAttachpoint>();
            }

            // Only seen on first mesh
            animTimeAdjust = new float[0];
            if (properties.HasFlag(BrgMeshProperty.VARIABLEANIM))
            {
                animTimeAdjust = new float[lenSpace];
                for (int i = 0; i < lenSpace; i++)
                {
                    animTimeAdjust[i] = reader.ReadSingle();
                }
            }
        }
        public BrgMesh(BrgFile file)
        {
            ParentFile = file;
            magic = 1230193997;
            version = 22;
            unknown04 = 40;

            vertices = new Vector3<float>[0];
            normals = new Vector3<float>[0];

            texVertices = new Vector2<float>[0];
            faceMaterials = new Int16[0];
            faceVertices = new Vector3<Int16>[0];
            vertMaterials = new Int16[0];

            unknown09Const = 191738312;
            animTimeMult = 1f;

            Vector2<float>[] unknown0a = new Vector2<float>[0];
            attachpoints = new List<BrgAttachpoint>();
            animTimeAdjust = new float[0];
        }

        public void Write(BrgBinaryWriter writer)
        {
            writer.Write(magic);

            writer.Write(version);
            writer.Write((UInt16)format);
            writer.Write((Int16)vertices.Length);
            writer.Write((Int16)faceVertices.Length);
            writer.Write((uint)properties);

            writer.WriteVector3(ref centerPos, true);
            writer.Write(boundingBoxMax.LongestAxisLength());//unknown03
            writer.WriteVector3(ref position, true);
            writer.WriteVector3(ref groundPos, true);
            writer.Write(unknown04);
            writer.Write((UInt16)flags);
            writer.WriteVector3(ref boundingBoxMin, true);
            writer.WriteVector3(ref boundingBoxMax, true);

            for (int i = 0; i < vertices.Length; i++)
            {
                writer.WriteVector3(ref vertices[i], true, true);
            }
            for (int i = 0; i < vertices.Length; i++)
            {
                writer.WriteVector3(ref normals[i], true, true);
            }

            if (!flags.HasFlag(BrgMeshFlag.NOTFIRSTMESH) || flags.HasFlag(BrgMeshFlag.MOVINGTEX))
            {
                if (flags.HasFlag(BrgMeshFlag.TEXTURE))
                {
                    for (int i = 0; i < texVertices.Length; i++)
                    {
                        writer.WriteVector2(ref texVertices[i], true);
                    }
                }
            }

            if (!flags.HasFlag(BrgMeshFlag.NOTFIRSTMESH))
            {
                if (flags.HasFlag(BrgMeshFlag.MATERIALS))
                {
                    for (int i = 0; i < faceMaterials.Length; i++)
                    {
                        writer.Write(faceMaterials[i]);
                    }
                }

                for (int i = 0; i < faceVertices.Length; i++)
                {
                    writer.WriteVector3(ref faceVertices[i]);
                }

                if (flags.HasFlag(BrgMeshFlag.MATERIALS))
                {
                    for (int i = 0; i < vertMaterials.Length; i++)
                    {
                        writer.Write(vertMaterials[i]);
                    }
                }
            }

            writer.Write(numIndex0);
            writer.Write(numMatrix0);
            writer.Write(unknown091);
            writer.Write(unknown09Unused);
            writer.Write(lastMaterialIndex);
            writer.Write(animTime);
            writer.Write(unknown09Const);
            writer.Write((Int16)0);
            writer.WriteHalf(unknown09e);
            writer.Write(animTimeMult);
            writer.Write(animTimeAdjust.Length);
            writer.Write(numMaterialsUsed);

            if (((flags.HasFlag(BrgMeshFlag.TRANSPCOLOR) || flags.HasFlag(BrgMeshFlag.CHANGINGCOL)) && !flags.HasFlag(BrgMeshFlag.NOTFIRSTMESH))
                || flags.HasFlag(BrgMeshFlag.VERTCOLOR))
            {
                for (int i = 0; i < unknown0a.Length; i++)
                {
                    writer.WriteVector2(ref unknown0a[i], true);
                }
            }

            if (flags.HasFlag(BrgMeshFlag.ATTACHPOINTS))
            {
                writer.Write((Int16)attachpoints.Count);

                List<int> nameId = new List<int>();
                int maxNameId = 0;
                for (int i = 0; i < attachpoints.Count; i++)
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
                int numIndex = (Int16)(maxNameId + 1);//(Int16)(55 - maxNameId);
                writer.Write((Int16)numIndex);
                writer.Write((Int16)1);


                for (int i = 0; i < attachpoints.Count; i++)
                {
                    writer.WriteVector3(ref attachpoints[i].x, true, true);
                }
                for (int i = 0; i < attachpoints.Count; i++)
                {
                    writer.WriteVector3(ref attachpoints[i].y, true, true);
                }
                for (int i = 0; i < attachpoints.Count; i++)
                {
                    writer.WriteVector3(ref attachpoints[i].z, true, true);
                }
                for (int i = 0; i < attachpoints.Count; i++)
                {
                    writer.WriteVector3(ref attachpoints[i].position, true, true);
                }
                for (int i = 0; i < attachpoints.Count; i++)
                {
                    writer.WriteVector3(ref attachpoints[i].unknown11a, true, true);
                }
                for (int i = 0; i < attachpoints.Count; i++)
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
                for (int i = 0; i < attachpoints.Count; i++)
                {
                    for (int j = 0; j < attachpoints.Count; j++)
                    {
                        if (nameId[i] == nameId2[j])
                        {
                            nameId2[j] = -1;
                            writer.Write((byte)j);
                            break;
                        }
                    }
                }
            }

            if (properties.HasFlag(BrgMeshProperty.VARIABLEANIM))
            {
                for (int i = 0; i < animTimeAdjust.Length; i++)
                {
                    writer.Write(animTimeAdjust[i]);
                }
            }
        }

        // Only used for first key frame/mesh
        public string ExportToMax()
        {
            string vertArray = Maxscript.NewArray("vertArray");
            string normArray = Maxscript.NewArray("normArray");
            string texVerts = Maxscript.NewArray("texVerts");

            string faceMats = Maxscript.NewArray("faceMats");
            string faceArray = Maxscript.NewArray("faceArray");

            Maxscript.CommentTitle("Load Vertices");
            foreach (Vector3<float> vert in vertices)
            {
                Maxscript.Append(vertArray, Maxscript.NewPoint3<float>("v", -vert.X, -vert.Z, vert.Y));
            }
            Maxscript.CommentTitle("Load Normals");
            foreach (Vector3<float> norm in normals)
            {
                Maxscript.Append(normArray, Maxscript.NewPoint3<float>("n", norm.X, norm.Z, -norm.Y));
            }
            if (!flags.HasFlag(BrgMeshFlag.NOTFIRSTMESH) || flags.HasFlag(BrgMeshFlag.MOVINGTEX))
            {
                if (flags.HasFlag(BrgMeshFlag.TEXTURE))
                {
                    Maxscript.CommentTitle("Load UVW");
                    foreach (Vector2<float> texVert in texVertices)
                    {
                        Maxscript.Append(texVerts, Maxscript.NewPoint3<float>("tV", texVert.X, texVert.Y, 0));
                    }
                }
            }

            if (!flags.HasFlag(BrgMeshFlag.NOTFIRSTMESH))
            {
                if (flags.HasFlag(BrgMeshFlag.MATERIALS))
                {
                    Maxscript.CommentTitle("Load Face Materials");
                    foreach (Int16 fMat in faceMaterials)
                    {
                        Maxscript.Append(faceMats, fMat.ToString());
                    }
                }

                Maxscript.CommentTitle("Load Faces");
                foreach (Vector3<Int16> face in faceVertices)
                {
                    Maxscript.Append(faceArray, Maxscript.NewPoint3<Int32>("fV", face.X + 1, face.Y + 1, face.Z + 1));
                }
            }

            if (flags.HasFlag(BrgMeshFlag.ATTACHPOINTS))
            {
                Maxscript.CommentTitle("Load Attachpoints");
                for (int i = 0; i < attachpoints.Count; i++)
                {
                    string boxSize = Maxscript.NewPoint3<Int32>("boxSize", 1, 1, 1);
                    string attachDummy = Maxscript.NewDummy("attachDummy", attachpoints[i].GetMaxName(i), attachpoints[i].GetMaxTransform(), attachpoints[i].GetMaxPosition(), boxSize, attachpoints[i].GetMaxScale());
                }
            }

            string mainObject = "mainObj";
            Maxscript.AnimateAtTime(ParentFile.GetFrameTime(0), Maxscript.NewMeshLiteral(mainObject, vertArray, normArray, faceArray, faceMats, texVerts));
            Maxscript.Command("{0} = getNodeByName \"{0}\"", mainObject);

            string groundPlanePos = Maxscript.NewPoint3<float>("groundPlanePos", -groundPos.X, -groundPos.Z, groundPos.Y);
            Maxscript.Command("plane name:\"ground\" pos:{0} length:10 width:10", groundPlanePos);

            Maxscript.CommentTitle("TVert Hack");
            Maxscript.Command("buildTVFaces {0}", mainObject);
            for (int i = 1; i <= faceVertices.Length; i++)
            {
                Maxscript.Command("setTVFace {0} {1} (getFace {0} {1})", mainObject, i);
            }

            if (flags.HasFlag(BrgMeshFlag.MOVINGTEX))
            {
                Maxscript.Command("select {0}", mainObject);
                Maxscript.Command("addModifier {0} (Unwrap_UVW())", mainObject);

                Maxscript.Command("select {0}.verts", mainObject);
                Maxscript.Animate("{0}.Unwrap_UVW.moveSelected [0,0,0]", mainObject);
            }

            return mainObject;
        }
        public void ImportFromMax(string mainObject, int meshIndex)
        {
            UpdateSettings(meshIndex);
            //flags = ParentFile.ParentForm.Flags;
            if (meshIndex > 0)
            {
                //flags |= BrgMeshFlag.NOTFIRSTMESH;
            }
            //format = ParentFile.ParentForm.Format;
            //properties = ParentFile.ParentForm.Properties;
            if (flags.HasFlag(BrgMeshFlag.NOTFIRSTMESH))
            {
                //properties = properties ^ BrgMeshProperty.VARIABLEANIM;
            }

            centerPos.X = -(float)Maxscript.Query("{0}.center.x", Maxscript.QueryType.Float, mainObject);
            centerPos.Z = -(float)Maxscript.Query("{0}.center.y", Maxscript.QueryType.Float, mainObject);
            centerPos.Y = (float)Maxscript.Query("{0}.center.z", Maxscript.QueryType.Float, mainObject);

            Maxscript.Command("grnd = getNodeByName \"ground\"");
            if (!Maxscript.QueryBoolean("grnd == undefined"))
            {
                groundPos.X = -(float)Maxscript.Query("grnd.position.x", Maxscript.QueryType.Float);
                groundPos.Z = -(float)Maxscript.Query("grnd.position.y", Maxscript.QueryType.Float);
                groundPos.Y = (float)Maxscript.Query("grnd.position.z", Maxscript.QueryType.Float);
            }

            Maxscript.Command("{0}BBMax = {0}.max", mainObject);
            Maxscript.Command("{0}BBMin = {0}.min", mainObject);
            Vector3<float> bBoxMax = new Vector3<float>(Maxscript.QueryFloat("{0}BBMax.X", mainObject), Maxscript.QueryFloat("{0}BBMax.Y", mainObject), Maxscript.QueryFloat("{0}BBMax.Z", mainObject));
            Vector3<float> bBoxMin = new Vector3<float>(Maxscript.QueryFloat("{0}BBMin.X", mainObject), Maxscript.QueryFloat("{0}BBMin.Y", mainObject), Maxscript.QueryFloat("{0}BBMin.Z", mainObject));
            Vector3<float> bBox = (bBoxMax - bBoxMin) / 2;
            boundingBoxMin.X = -bBox.X;
            boundingBoxMin.Z = -bBox.Y;
            boundingBoxMin.Y = -bBox.Z;
            boundingBoxMax.X = bBox.X;
            boundingBoxMax.Z = bBox.Y;
            boundingBoxMax.Y = bBox.Z;

            float time;
            if (ParentFile.Header.numMeshes == 1)
            {
                time = 0;
            }
            else if (flags.HasFlag(BrgMeshFlag.MOVINGTEX))
            {
                time = Maxscript.QueryFloat("{0}.Unwrap_UVW[1].keys[{1}].time as float", mainObject, meshIndex + 1) / 4800f;
            }
            else
            {
                time = (float)Maxscript.Query("{0}.mesh[1].keys[{1}].time as float", Maxscript.QueryType.Float, mainObject, meshIndex + 1) / 4800f;
            }
            string mainMesh = Maxscript.SnapshotAsMesh("mainMesh", mainObject, time);
            int numVertices = (int)Maxscript.Query("{0}.numverts", Maxscript.QueryType.Integer, mainMesh);
            int numFaces = (int)Maxscript.Query("{0}.numfaces", Maxscript.QueryType.Integer, mainMesh);
            string attachDummy = Maxscript.NewArray("attachDummy");
            Maxscript.AtTime(time, "{0} = $helpers/atpt??* as array", attachDummy);
            int numAttachpoints = (int)Maxscript.Query("{0}.count", Maxscript.QueryType.Integer, attachDummy);

            // Figure out the used vertices, and corresponding tex verts
            string vertMask = Maxscript.NewArray("vertMask");
            for (int i = 0; i < numVertices; i++)
            {
                Maxscript.Append(vertMask, 0);
            }
            for (int i = 0; i < numFaces; i++)
            {
                Maxscript.Command("face = getFace {0} {1}", mainMesh, i + 1);
                Maxscript.Command("tFace = getTVFace {0} {1}", mainMesh, i + 1);

                Maxscript.Command("{0}[face[1]] = tFace[1]", vertMask);
                Maxscript.Command("{0}[face[2]] = tFace[2]", vertMask);
                Maxscript.Command("{0}[face[3]] = tFace[3]", vertMask);
            }

            //System.Windows.Forms.MessageBox.Show("1");
            List<Vector3<float>> verticesList = new List<Vector3<float>>(numVertices);
            List<Vector3<float>> normalsList = new List<Vector3<float>>(numVertices);
            for (int i = 0; i < numVertices; i++)
            {
                //System.Windows.Forms.MessageBox.Show("1.1");
                bool export = (Boolean)Maxscript.Query("{0}[{1}] > 0", Maxscript.QueryType.Boolean, vertMask, i + 1);
                //System.Windows.Forms.MessageBox.Show("1.2");
                if (export)
                {
                    //System.Windows.Forms.MessageBox.Show("1.3");
                    Maxscript.Command("vertex = getVert {0} {1}", mainMesh, i + 1);
                    //System.Windows.Forms.MessageBox.Show("1.4");
                    verticesList.Add(new Vector3<float>(-Maxscript.QueryFloat("vertex.x"), Maxscript.QueryFloat("vertex.z"), -Maxscript.QueryFloat("vertex.y")));

                    //System.Windows.Forms.MessageBox.Show("1.5");
                    Maxscript.Command("normal = getNormal {0} {1}", mainMesh, i + 1);
                    //System.Windows.Forms.MessageBox.Show("1.6");
                    normalsList.Add(new Vector3<float>(Maxscript.QueryFloat("normal.x"), -Maxscript.QueryFloat("normal.z"), Maxscript.QueryFloat("normal.y")));
                    //System.Windows.Forms.MessageBox.Show("1.7");
                }
            }
            vertices = verticesList.ToArray();
            normals = normalsList.ToArray();

            //System.Windows.Forms.MessageBox.Show("2");
            if (!flags.HasFlag(BrgMeshFlag.NOTFIRSTMESH) || flags.HasFlag(BrgMeshFlag.MOVINGTEX))
            {
                if (flags.HasFlag(BrgMeshFlag.TEXTURE))
                {
                    List<Vector2<float>> texVerticesList = new List<Vector2<float>>(numVertices);
                    for (int i = 0; i < numVertices; i++)
                    {
                        bool export = (Boolean)Maxscript.Query("{0}[{1}] > 0", Maxscript.QueryType.Boolean, vertMask, i + 1);
                        if (export)
                        {
                            Maxscript.Command("tVert = getTVert {0} {1}[{2}]", mainMesh, vertMask, i + 1);
                            texVerticesList.Add(new Vector2<float>(Maxscript.QueryFloat("tVert.x"), Maxscript.QueryFloat("tVert.y")));
                        }
                    }
                    texVertices = texVerticesList.ToArray();
                }
            }

            //System.Windows.Forms.MessageBox.Show("3");
            HashSet<int> diffFaceMats = new HashSet<int>();
            if (!flags.HasFlag(BrgMeshFlag.NOTFIRSTMESH))
            {
                if (flags.HasFlag(BrgMeshFlag.MATERIALS))
                {
                    faceMaterials = new Int16[numFaces];
                    for (int i = 0; i < numFaces; i++)
                    {
                        faceMaterials[i] = (Int16)(Int32)Maxscript.Query("getFaceMatID {0} {1}", Maxscript.QueryType.Integer, mainMesh, i + 1);
                        diffFaceMats.Add(faceMaterials[i]);
                    }
                }

                //System.Windows.Forms.MessageBox.Show("3.1");
                faceVertices = new Vector3<Int16>[numFaces];
                for (int i = 0; i < faceVertices.Length; i++)
                {
                    Maxscript.Command("face = getFace {0} {1}", mainMesh, i + 1);
                    faceVertices[i].X = (Int16)((Int32)Maxscript.Query("face.x", Maxscript.QueryType.Integer) - 1);
                    faceVertices[i].Y = (Int16)((Int32)Maxscript.Query("face.y", Maxscript.QueryType.Integer) - 1);
                    faceVertices[i].Z = (Int16)((Int32)Maxscript.Query("face.z", Maxscript.QueryType.Integer) - 1);
                }

                //System.Windows.Forms.MessageBox.Show("3.2");
                if (flags.HasFlag(BrgMeshFlag.MATERIALS))
                {
                    vertMaterials = new Int16[numVertices];
                    for (int i = 0; i < faceVertices.Length; i++)
                    {
                        vertMaterials[faceVertices[i].X] = faceMaterials[i];
                        vertMaterials[faceVertices[i].Y] = faceMaterials[i];
                        vertMaterials[faceVertices[i].Z] = faceMaterials[i];
                    }
                }
            }

            //System.Windows.Forms.MessageBox.Show("4");
            //unknown091 = ParentFile.ParentForm.Unknown091;
            //lastMaterialIndex = ParentFile.ParentForm.LastMaterialIndex;
            if (meshIndex == 0)
            {
                lastMaterialIndex = diffFaceMats.Count - 1;
                numMaterialsUsed = diffFaceMats.Count;
            }
            animTime = (float)Maxscript.Query("animationRange.end.ticks / 4800 as float", Maxscript.QueryType.Float);
            //animTimeMult = ParentFile.ParentForm.TimeMult;

            //System.Windows.Forms.MessageBox.Show("5");
            if (flags.HasFlag(BrgMeshFlag.ATTACHPOINTS))
            {
                attachpoints = new List<BrgAttachpoint>(numAttachpoints);
                for (int i = 0; i < numAttachpoints; i++)
                {
                    attachpoints.Add(new BrgAttachpoint());
                    //System.Windows.Forms.MessageBox.Show("5.1");
                    attachpoints[i].NameId = BrgAttachpoint.GetIdByName(((string)Maxscript.Query("{0}[{1}].name", Maxscript.QueryType.String, attachDummy, i + 1)).Substring(7));
                    //System.Windows.Forms.MessageBox.Show("5.2");
                    Maxscript.AtTime(time, "{0}Transform = {0}[{1}].rotation as matrix3", attachDummy, i + 1);
                    Maxscript.AtTime(time, "{0}Position = {0}[{1}].position", attachDummy, i + 1);
                    Maxscript.AtTime(time, "{0}Scale = {0}[{1}].scale", attachDummy, i + 1);
                    //System.Windows.Forms.MessageBox.Show("5.3");
                    Maxscript.AtTime(time, "{0}BBMax = {0}[{1}].max", attachDummy, i + 1);
                    Maxscript.AtTime(time, "{0}BBMin = {0}[{1}].min", attachDummy, i + 1);
                    bBoxMax = new Vector3<float>(Maxscript.QueryFloat("{0}BBMax.X", attachDummy), Maxscript.QueryFloat("{0}BBMax.Y", attachDummy), Maxscript.QueryFloat("{0}BBMax.Z", attachDummy));
                    bBoxMin = new Vector3<float>(Maxscript.QueryFloat("{0}BBMin.X", attachDummy), Maxscript.QueryFloat("{0}BBMin.Y", attachDummy), Maxscript.QueryFloat("{0}BBMin.Z", attachDummy));
                    bBox = (bBoxMax - bBoxMin) / 2;
                    //System.Windows.Forms.MessageBox.Show("5.4");

                    attachpoints[i].x.X = (float)Maxscript.Query("{0}Transform[1].z", Maxscript.QueryType.Float, attachDummy);
                    attachpoints[i].x.Y = (float)Maxscript.Query("{0}Transform[3].z", Maxscript.QueryType.Float, attachDummy);
                    attachpoints[i].x.Z = (float)Maxscript.Query("{0}Transform[2].z", Maxscript.QueryType.Float, attachDummy);

                    attachpoints[i].y.X = (float)Maxscript.Query("{0}Transform[1].y", Maxscript.QueryType.Float, attachDummy);
                    attachpoints[i].y.Y = (float)Maxscript.Query("{0}Transform[3].y", Maxscript.QueryType.Float, attachDummy);
                    attachpoints[i].y.Z = (float)Maxscript.Query("{0}Transform[2].y", Maxscript.QueryType.Float, attachDummy);

                    attachpoints[i].z.X = (float)Maxscript.Query("{0}Transform[1].x", Maxscript.QueryType.Float, attachDummy);
                    attachpoints[i].z.Y = (float)Maxscript.Query("{0}Transform[3].x", Maxscript.QueryType.Float, attachDummy);
                    attachpoints[i].z.Z = (float)Maxscript.Query("{0}Transform[2].x", Maxscript.QueryType.Float, attachDummy);

                    attachpoints[i].position.X = -(float)Maxscript.Query("{0}Position.x", Maxscript.QueryType.Float, attachDummy);
                    attachpoints[i].position.Z = -(float)Maxscript.Query("{0}Position.y", Maxscript.QueryType.Float, attachDummy);
                    attachpoints[i].position.Y = (float)Maxscript.Query("{0}Position.z", Maxscript.QueryType.Float, attachDummy);

                    attachpoints[i].unknown11a.X = -bBox.X;
                    attachpoints[i].unknown11a.Z = -bBox.Y;
                    attachpoints[i].unknown11a.Y = -bBox.Z;
                    attachpoints[i].unknown11b.X = bBox.X;
                    attachpoints[i].unknown11b.Z = bBox.Y;
                    attachpoints[i].unknown11b.Y = bBox.Z;
                    //System.Windows.Forms.MessageBox.Show("5.5");
                }
            }
        }

        public void UpdateSettings(int meshIndex)
        {
            flags = ParentFile.ParentForm.Flags;
            format = ParentFile.ParentForm.Format;
            properties = ParentFile.ParentForm.Properties;
            unknown091 = ParentFile.ParentForm.Unknown091;
            animTimeMult = ParentFile.ParentForm.TimeMult;
            if (meshIndex > 0)
            {
                flags |= BrgMeshFlag.NOTFIRSTMESH;
                properties &= ~BrgMeshProperty.VARIABLEANIM;
            }
        }

        public void ReadBr3(BrgBinaryReader reader)
        {
            Int16 numVertices = reader.ReadInt16();
            Int16 numFaces = reader.ReadInt16();
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
            writer.Write(vertices.Length);
            writer.Write(faceVertices.Length);
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
            x = new Vector3<float>(0, 0, 1);
            y = new Vector3<float>(0, 1, 0);
            z = new Vector3<float>(-1, 0, 0);
            position = new Vector3<float>(1.5f);
            unknown11a = new Vector3<float>(-0.25f);
            unknown11b = new Vector3<float>(0.25f);
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

        public string GetMaxTransform()
        {
            //string xVector = Maxscript.NewPoint3<float>("xVector", this.x.X, this.y.X, this.z.X);
            //string yVector = Maxscript.NewPoint3<float>("yVector", this.x.Z, this.y.Z, this.z.Z);
            //string zVector = Maxscript.NewPoint3<float>("zVector", this.x.Y, this.y.Y, this.z.Y);
            //string xVector = Maxscript.NewPoint3<float>("xVector", this.x.X, this.x.Z, this.x.Y);
            //string yVector = Maxscript.NewPoint3<float>("yVector", this.z.X, this.z.Z, this.z.Y);
            //string zVector = Maxscript.NewPoint3<float>("zVector", this.y.X, this.y.Z, this.y.Y);

            string xVector = Maxscript.NewPoint3<float>("xVector", this.z.X, this.y.X, this.x.X);
            string yVector = Maxscript.NewPoint3<float>("yVector", this.z.Z, this.y.Z, this.x.Z);
            string zVector = Maxscript.NewPoint3<float>("zVector", this.z.Y, this.y.Y, this.x.Y);
            //string xVector = MaxHelper.NewPoint3<float>("xVector", -attachpoints[i].x.X, -attachpoints[i].z.X, attachpoints[i].y.X);
            //string yVector = MaxHelper.NewPoint3<float>("yVector", -attachpoints[i].x.Z, -attachpoints[i].z.Z, attachpoints[i].y.Z);
            //string zVector = MaxHelper.NewPoint3<float>("zVector", -attachpoints[i].x.Y, -attachpoints[i].z.Y, attachpoints[i].y.Y);
            //string posVector = MaxHelper.NewPoint3<float>("posVector", -this.position.X, -this.position.Z, this.position.Y);
            string posVector = Maxscript.NewPoint3<float>("rotPosVect", 0, 0, 0);
            return Maxscript.NewMatrix3("transformMatrix", xVector, yVector, zVector, posVector);
        }
        public string GetMaxPosition()
        {
            return Maxscript.NewPoint3<float>("posVector", -this.position.X, -this.position.Z, this.position.Y);
        }
        public string GetMaxBoxSize()
        {
            return Maxscript.NewPoint3<float>("posVector", 1, 1, 1);
        }
        public string GetMaxScale()
        {
            return Maxscript.NewPoint3<float>("boundingScale", (this.unknown11b.X - this.unknown11a.X), (this.unknown11b.Z - this.unknown11a.Z), (this.unknown11b.Y - this.unknown11a.Y));
        }
        public string GetMaxName(int number)
        {
            return String.Format("atpt{0:D2}.{1}", number, this.Name);
        }
    }

    public class BrgMaterial
    {
        public BrgFile ParentFile;
        public string EditorName
        {
            get
            {
                return "Mat ID: " + id;
            }
        }
        public Color DiffuseColor
        {
            get
            {
                return Color.FromArgb((int)Math.Round(diffuse.X * 255f), (int)Math.Round(diffuse.Y * 255f), (int)Math.Round(diffuse.Z * 255f));
            }
        }
        public Color AmbientColor
        {
            get
            {
                return Color.FromArgb((int)Math.Round(ambient.X * 255f), (int)Math.Round(ambient.Y * 255f), (int)Math.Round(ambient.Z * 255f));
            }
        }
        public Color SpecularColor
        {
            get
            {
                return Color.FromArgb((int)Math.Round(specular.X * 255f), (int)Math.Round(specular.Y * 255f), (int)Math.Round(specular.Z * 255f));
            }
        }
        public Color SelfIllumColor
        {
            get
            {
                return Color.FromArgb((int)Math.Round(selfIllum.X * 255f), (int)Math.Round(selfIllum.Y * 255f), (int)Math.Round(selfIllum.Z * 255f));
            }
        }

        int magic;
        public int id;
        public BrgMatFlag flags;
        public int unknown01b;
        //int nameLength;
        Vector3<float> diffuse; //unknown02 [36 bytes]
        Vector3<float> ambient;
        Vector3<float> specular;
        Vector3<float> selfIllum; //unknown03 [12 bytes]
        public string name;
        public string name2;
        public float specularLevel;
        public float alphaOpacity; //unknown04
        public List<BrgMatSFX> sfx;

        public BrgMaterial(BrgBinaryReader reader, BrgFile file)
        {
            ParentFile = file;
            magic = reader.ReadInt32();
            if (magic != EndianBitConverter.Little.ToInt32(Encoding.UTF8.GetBytes("MTRL"), 0))
            {
                throw new Exception("Incorrect material header!");
            }

            id = reader.ReadInt32();
            flags = (BrgMatFlag)reader.ReadInt32();
            unknown01b = reader.ReadInt32();
            int nameLength = reader.ReadInt32();
            reader.ReadVector3(out diffuse);
            reader.ReadVector3(out ambient);
            reader.ReadVector3(out specular);
            reader.ReadVector3(out selfIllum);

            name = reader.ReadString(nameLength);
            if (flags.HasFlag(BrgMatFlag.USESPECLVL))
            {
                specularLevel = reader.ReadSingle();
            }
            if (flags.HasFlag(BrgMatFlag.MATTEXURE2))
            {
                name2 = reader.ReadString(reader.ReadInt32());
            }
            alphaOpacity = reader.ReadSingle();

            if (flags.HasFlag(BrgMatFlag.REFLECTIONTEXTURE))
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
        public BrgMaterial(BrgFile file)
        {
            ParentFile = file;
            magic = 1280463949;
            id = 0;
            flags = 0;
            unknown01b = 0;
            //nameLength = 0;
            diffuse = new Vector3<float>();
            ambient = new Vector3<float>();
            specular = new Vector3<float>();
            selfIllum = new Vector3<float>();

            name = string.Empty;
            name2 = string.Empty;

            specularLevel = 1;

            alphaOpacity = 1;

            sfx = new List<BrgMatSFX>();
        }
        public BrgMaterial(BrgMaterial copy)
        {
            ParentFile = copy.ParentFile;
            magic = copy.magic;

            id = copy.id;
            flags = copy.flags;
            unknown01b = copy.unknown01b;
            //nameLength = copy.nameLength;
            diffuse = copy.diffuse;
            ambient = copy.ambient;
            specular = copy.specular;
            selfIllum = copy.selfIllum;

            name = copy.name;
            if (flags.HasFlag(BrgMatFlag.USESPECLVL))
            {
                specularLevel = copy.specularLevel;
            }
            if (flags.HasFlag(BrgMatFlag.MATTEXURE2))
            {
                name2 = copy.name2;
            }
            alphaOpacity = copy.alphaOpacity;

            if (flags.HasFlag(BrgMatFlag.REFLECTIONTEXTURE))
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

        public string ExportToMax()
        {
            Maxscript.Command("mat = StandardMaterial()");
            Maxscript.Command("mat.name = \"{0}\"", name);
            Maxscript.Command("mat.adLock = false");
            Maxscript.Command("mat.useSelfIllumColor = true");
            Maxscript.Command("mat.diffuse = color {0} {1} {2}", diffuse.X * 255f, diffuse.Y * 255f, diffuse.Z * 255f);
            Maxscript.Command("mat.ambient = color {0} {1} {2}", ambient.X * 255f, ambient.Y * 255f, ambient.Z * 255f);
            Maxscript.Command("mat.specular = color {0} {1} {2}", specular.X * 255f, specular.Y * 255f, specular.Z * 255f);
            Maxscript.Command("mat.selfIllumColor = color {0} {1} {2}", selfIllum.X * 255f, selfIllum.Y * 255f, selfIllum.Z * 255f);
            Maxscript.Command("mat.opacity = {0}", alphaOpacity * 100f);
            Maxscript.Command("mat.specularLevel = {0}", specularLevel);
            //MaxHelper.Command("print \"{0}\"", name);

            Maxscript.Command("tex = BitmapTexture()");
            Maxscript.Command("tex.name = \"{0}\"", name);
            if (flags.HasFlag(BrgMatFlag.MATNONE25))
            {
                if (flags.HasFlag(BrgMatFlag.REFLECTIONTEXTURE))
                {
                    Maxscript.Command("rTex = BitmapTexture()");
                    Maxscript.Command("rTex.name = \"{0}\"", Path.GetFileNameWithoutExtension(sfx[0].Name));
                    Maxscript.Command("rTex.filename = \"{0}\"", Path.GetFileNameWithoutExtension(sfx[0].Name) + ".tga");
                    Maxscript.Command("mat.reflectionMap = rTex");
                }
                if (flags.HasFlag(BrgMatFlag.MATTEXURE2))
                {
                    Maxscript.Command("aTex = BitmapTexture()");
                    Maxscript.Command("aTex.name = \"{0}\"", name2);
                    Maxscript.Command("aTex.filename = \"{0}\"", name2 + ".tga");
                    Maxscript.Command("mat.ambientMap = aTex");
                }
                if (flags.HasFlag(BrgMatFlag.WHITESELFILLUMCOLOR))
                {
                    Maxscript.Command("tex.filename = \"{0}\"", name + ".tga");
                    Maxscript.Command("mat.selfIllumMap = tex");
                }
                if (flags.HasFlag(BrgMatFlag.PLAYERCOLOR))
                {
                    Maxscript.Command("tex.filename = \"{0}\"", name + ".tga");
                    Maxscript.Command("mat.bumpMap = tex");
                }
            }
            if (flags.HasFlag(BrgMatFlag.DIFFUSETEXTURE))
            {
                //MaxHelper.Command("print {0}", name);
                Maxscript.Command("tex.filename = \"{0}\"", name + ".tga");
                Maxscript.Command("mat.diffusemap = tex");
            }

            return "mat";
        }
        public void ImportFromMax(string mainObject, int materialIndex)
        {
            id = (Int32)Maxscript.Query("{0}.material.materialIDList[{1}]", Maxscript.QueryType.Integer, mainObject, materialIndex + 1);
            Maxscript.Command("mat = {0}.material[{1}]", mainObject, id);

            diffuse.X = (float)Maxscript.Query("mat.diffuse.r", Maxscript.QueryType.Float) / 255f;
            diffuse.Y = (float)Maxscript.Query("mat.diffuse.g", Maxscript.QueryType.Float) / 255f;
            diffuse.Z = (float)Maxscript.Query("mat.diffuse.b", Maxscript.QueryType.Float) / 255f;
            ambient.X = (float)Maxscript.Query("mat.ambient.r", Maxscript.QueryType.Float) / 255f;
            ambient.Y = (float)Maxscript.Query("mat.ambient.g", Maxscript.QueryType.Float) / 255f;
            ambient.Z = (float)Maxscript.Query("mat.ambient.b", Maxscript.QueryType.Float) / 255f;
            specular.X = (float)Maxscript.Query("mat.specular.r", Maxscript.QueryType.Float) / 255f;
            specular.Y = (float)Maxscript.Query("mat.specular.g", Maxscript.QueryType.Float) / 255f;
            specular.Z = (float)Maxscript.Query("mat.specular.b", Maxscript.QueryType.Float) / 255f;
            selfIllum.X = (float)Maxscript.Query("mat.selfIllumColor.r", Maxscript.QueryType.Float) / 255f;
            selfIllum.Y = (float)Maxscript.Query("mat.selfIllumColor.g", Maxscript.QueryType.Float) / 255f;
            selfIllum.Z = (float)Maxscript.Query("mat.selfIllumColor.b", Maxscript.QueryType.Float) / 255f;
            alphaOpacity = (float)Maxscript.Query("mat.opacity", Maxscript.QueryType.Float) / 100f;
            specularLevel = (float)Maxscript.Query("mat.specularLevel", Maxscript.QueryType.Float);
            if (specularLevel > 0)
            {
                flags |= BrgMatFlag.USESPECLVL;
            }

            //flags |= BrgMatFlag.MATNONE1;
            if (Maxscript.QueryBoolean("(classof mat.reflectionMap) == BitmapTexture"))
            {
                flags |= BrgMatFlag.MATNONE25 | BrgMatFlag.REFLECTIONTEXTURE;
                BrgMatSFX sfxMap = new BrgMatSFX();
                sfxMap.Id = 30;
                sfxMap.Name = (string)Maxscript.Query("getFilenameFile(mat.reflectionMap.filename)", Maxscript.QueryType.String) + ".cub";
                sfx.Add(sfxMap);
            }
            if (Maxscript.QueryBoolean("(classof mat.ambientMap) == BitmapTexture"))
            {
                flags |= BrgMatFlag.MATNONE25 | BrgMatFlag.MATTEXURE2;
                name2 = (string)Maxscript.Query("getFilenameFile(mat.ambientMap.filename)", Maxscript.QueryType.String);
            }
            if (Maxscript.QueryBoolean("(classof mat.selfIllumMap) == BitmapTexture"))
            {
                flags |= BrgMatFlag.MATNONE25 | BrgMatFlag.DIFFUSETEXTURE | BrgMatFlag.WHITESELFILLUMCOLOR;
                name = (string)Maxscript.Query("getFilenameFile(mat.selfIllumMap.filename)", Maxscript.QueryType.String);
            }
            if (Maxscript.QueryBoolean("(classof mat.bumpMap) == BitmapTexture"))
            {
                flags |= BrgMatFlag.MATNONE25 | BrgMatFlag.DIFFUSETEXTURE | BrgMatFlag.PLAYERCOLOR;
                name = (string)Maxscript.Query("getFilenameFile(mat.bumpMap.filename)", Maxscript.QueryType.String);
            }
            if (Maxscript.QueryBoolean("(classof mat.diffusemap) == BitmapTexture"))
            {
                flags |= BrgMatFlag.DIFFUSETEXTURE;
                name = (string)Maxscript.Query("getFilenameFile(mat.diffusemap.filename)", Maxscript.QueryType.String);
                if (name.Length > 0)
                {
                    flags |= BrgMatFlag.MATNONE25;
                }
            }
        }

        public void Write(BrgBinaryWriter writer)
        {
            writer.Write(magic);
            writer.Write(id);
            writer.Write((int)flags);

            writer.Write(unknown01b);
            writer.Write(Encoding.UTF8.GetByteCount(name));

            writer.WriteVector3(ref diffuse);
            writer.WriteVector3(ref ambient);
            writer.WriteVector3(ref specular);
            writer.WriteVector3(ref selfIllum);

            writer.WriteString(name, 0);

            if (flags.HasFlag(BrgMatFlag.USESPECLVL))
            {
                writer.Write(specularLevel);
            }
            if (flags.HasFlag(BrgMatFlag.MATTEXURE2))
            {
                writer.WriteString(name2, 4);
            }

            writer.Write(alphaOpacity);

            if (flags.HasFlag(BrgMatFlag.REFLECTIONTEXTURE))
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
            //nameLength = Encoding.UTF8.GetByteCount(name);
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
