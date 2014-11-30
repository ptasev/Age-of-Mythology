using System;
using System.Collections.Generic;
using System.Text;
using MiscUtil.Conversion;
using MiscUtil.IO;
using ManagedServices;
using MiscUtil;
using System.Drawing;
using System.IO;
using System.Globalization;
using System.Linq;
using System.Collections;
using Wintellect.PowerCollections;

namespace AoMEngineLibrary
{
    public struct VertexColor
    {
        public byte R;
        public byte G;
        public byte B;
        public byte A;
    }
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

        public static Vector3<T> operator -(Vector3<T> one)
        {
            return new Vector3<T>(Operator.Negate<T>(one.X), Operator.Negate<T>(one.Y), Operator.Negate<T>(one.Z));
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

        public static Vector3<T> CrossProduct(Vector3<T> v1, Vector3<T> v2)
        {
            return
            (
               new Vector3<T>
               (
                  Operator.Subtract<T>(Operator.Multiply<T>(v1.Y, v2.Z), Operator.Multiply<T>(v1.Z, v2.Y)),
                  Operator.Subtract<T>(Operator.Multiply<T>(v1.Z, v2.X), Operator.Multiply<T>(v1.X, v2.Z)),
                  Operator.Subtract<T>(Operator.Multiply<T>(v1.X, v2.Y), Operator.Multiply<T>(v1.Y, v2.X))
               )
            );
        }
        public static T DotProduct(Vector3<T> v1, Vector3<T> v2)
        {
            return
            (
                Operator.Add<T>(Operator.Add<T>(Operator.Multiply<T>(v1.X, v2.X), Operator.Multiply<T>(v1.Y, v2.Y)), Operator.Multiply<T>(v1.Z, v2.Z))
            );
        }
        public static Vector3<T> CalcNormalOfFace(Vector3<T>[] pPositions, Vector3<T>[] pNormals)
        {
            Vector3<T> p0 = pPositions[1] - pPositions[0];
            Vector3<T> p1 = pPositions[2] - pPositions[0];
            Vector3<T> faceNormal = Vector3<T>.CrossProduct(p0, p1);

            Vector3<T> vertexNormal = pNormals[0];
            float dot = Operator.Convert<T, float>(Vector3<T>.DotProduct(faceNormal, vertexNormal));

            return (dot < 0.0f) ? -faceNormal : faceNormal;
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
        ANIMVERTCOLORALPHA = 0x0001, // animated vertex color alpha
        TEXCOORDSA = 0x0002, // Mesh has first set of tex coords
        MULTTEXCOORDS = 0x0004, // mesh has texture coords sets 2..n
        ANIMATEDMESH = 0x0008, // Deprecated - Not used after revision 0x0008
        RESERVED = 0x0010, // ?
        COLORCHANNEL = 0x0020, // Mesh has a vertex color channel
        MATERIAL = 0x0040, // Mesh has material data
        BUMPMAPINFO = 0x0080, // Mesh has bump/normal map info
        ATTACHPOINTS = 0x0100, // Mesh contains dummy objects
        NOZBUFFER = 0x0200, // mesh should not be rendered with z-testing
        SECONDARYMESH = 0x0400, // Secondary Mesh 2..n
        ANIMTEXCOORDS = 0x0800, // Mesh contains animated tex coords
        PARTICLESYSTEM = 0x1000, // Mesh is a Particle System
        PARTICLEPOINTS = 0x2000, // Mesh vertices are treated as particle points with radii
        COLORALPHACHANNEL = 0x4000, // Vertex color channel is treated as alpha channel
        ANIMVERTCOLORSNAP = 0x8000 // Animated vertex colors snap between keyframes
    };
    [Flags]
    public enum BrgMeshFormat : ushort
    {
        BILLBOARD = 0x0001, // rotates with the player view
        ANIMTEXCOORDSNAP = 0x0002, // Animated UV/animated texture coords snap between keyframes
        HASFACENORMALS = 0x0004, // has face normals
        ANIMATED = 0x0008, // animation length included in extended header
        KEYFRAMESNAP = 0x0010, // keyframe snap, not smooth
        NOLOOPANIMATE = 0x0020, // don't animate Last-First frame
        MFRESRVED0 = 0x0040, // ?
        FACEGROUPMAP = 0x0080, // Mesh has face group list
        STRIPPED = 0x0100  // Mesh data is stripped
    };
    public enum BrgMeshAnimType : byte
    {
        KEYFRAME    = 0x0000, // keyframe based animation
        NONUNIFORM  = 0x0001, // Non-uniform animation
        SKINBONE    = 0x0002 // Skinned Animation
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
        GROUNDTEXTUREOVERLAY = 0x00004000, // ground texture?
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
        public int unknown01;
        public int numMaterials;
        public int unknown02;
        public int numMeshes;
        public int space;
        public int unknown03;
    }

    public struct BrgAsetHeader
    {
        public int numFrames;
        public float frameStep;
        public float animTime;
        public float frequency;
        public float spf;
        public float fps;
        public int space;
    }

    public struct BrgMeshHeader
    {
        public Int16 version;
        public BrgMeshFormat format;
        public Int16 numVertices;
        public Int16 numFaces;
        public byte interpolationType;
        public BrgMeshAnimType properties;
        public Int16 userDataEntryCount;
        public Vector3<float> centerPos;
        public float centerRadius;
        public Vector3<float> position;
        public Vector3<float> groundPos;
        public Int16 extendedHeaderSize;
        public BrgMeshFlag flags;
        public Vector3<float> boundingBoxMin;
        public Vector3<float> boundingBoxMax;
    }

    public struct BrgMeshExtendedHeader
    {
        public Int16 numIndex;
        public Int16 numMatrix;
        public Int16 nameLength; // unknown091
        public Int16 pointMaterial;
        public float pointRadius; // unknown09Unused
        public byte materialCount; // lastMaterialIndex
        public byte shadowNameLength0;
        public byte shadowNameLength1;
        public byte shadowNameLength2;
        public float animTime;
        public int materialLibraryTimestamp; // unknown09Const
        //public Int16 checkSpace; //09a
        public float unknown09e;
        public float exportedScaleFactor; // animTimeMult
        public int nonUniformKeyCount; //09c
        public int uniqueMaterialCount; // numMaterialsUsed
    }

    public struct BrgUserDataEntry
    {
        public int dataNameLength;
        public int dataType;
        public object data;
        public string dataName;
    }

    public static class Maxscript
    {
        private static CultureInfo cult = CultureInfo.InvariantCulture;
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
        public static string QueryString(string command, params object[] args)
        {
            return (string)Query(command, QueryType.String, args);
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
            Command("at time " + time.ToString(cult) + "s (" + command + ")", args);
        }
        public static void SetVarAtTime(float time, string varName, string command, params object[] args)
        {
            Command(varName + " = at time " + time.ToString(cult) + "s (" + command + ")", args);
        }
        public static void Animate(string command, params object[] args)
        {
            Command("with animate on (" + command + ")", args);
        }
        public static void AnimateAtTime(float time, string command, params object[] args)
        {
            Command("with animate on (at time " + time.ToString(cult) + "s (" + command + "))", args);
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
            Command("{1} = at time {0}s (snapshotAsMesh {2})", time, varName, varNode);
            return varName;
        }
        public static string NewMeshLiteral(string name, string vertArray, string normArray, string faceArray, string faceMatIdArray, string texVertArray)
        {
            //return MaxscriptSDK.AssembleScript("mesh name:\"{0}\" vertices:{1} faces:{2} materialIDs:{3} tverts:{4}", name, vertArray, faceArray, faceMatIdArray, texVertArray);
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
        public static string NewBitArray(string varName)
        {
            Command("{0} = #{{}}", varName);
            return varName;
        }
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
        public string FileName;
        public MaxPluginForm ParentForm;

        public BrgHeader Header;
        public BrgAsetHeader AsetHeader;
        public List<BrgMesh> Mesh;
        public List<BrgMaterial> Material;

        public BrgFile(System.IO.FileStream fileStream, MaxPluginForm form)
            : this(fileStream)
        {
            ParentForm = form;
        }
        public BrgFile(System.IO.FileStream fileStream)
        {
            using (BrgBinaryReader reader = new BrgBinaryReader(new LittleEndianBitConverter(), fileStream))
            {
                FileName = fileStream.Name;
                string magic = reader.ReadString(4);
                if (magic != "BANG")
                {
                    throw new Exception("This is not a BRG file!");
                }
                reader.ReadHeader(ref Header);

                int asetCount = 0;
                Mesh = new List<BrgMesh>(Header.numMeshes);
                Material = new List<BrgMaterial>();
                while (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    magic = reader.ReadString(4);
                    if (magic == "ASET")
                    {
                        reader.ReadAsetHeader(ref AsetHeader);
                        ++asetCount;
                    }
                    else if (magic == "MESI")
                    {
                        Mesh.Add(new BrgMesh(reader, this));
                    }
                    else if (magic == "MTRL")
                    {
                        BrgMaterial mat = new BrgMaterial(reader, this);
                        if (!ContainsMaterialID(mat.id))
                        {
                            Material.Add(mat);
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
                    throw new Exception("Multiple ASETs!");
                }

                if (Header.numMeshes < Mesh.Count)
                {
                    throw new Exception("Inconsistent mesh count!");
                }

                if (Header.numMaterials < Material.Count)
                {
                    throw new Exception("Inconsistent material count!");
                }

                if (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    throw new Exception("The end of stream was not reached!");
                }
            }
        }
        public BrgFile(MaxPluginForm form) 
        {
            FileName = string.Empty;
            ParentForm = form;
            Header.unknown03 = 1999922179;

            Mesh = new List<BrgMesh>();
            Material = new List<BrgMaterial>();
        }

        public void Write(System.IO.FileStream fileStream)
        {
            using (BrgBinaryWriter writer = new BrgBinaryWriter(new LittleEndianBitConverter(), fileStream))
            {
                FileName = fileStream.Name;
                writer.Write(1196310850); // magic "BANG"

                Header.numMeshes = Mesh.Count;
                Header.numMaterials = Material.Count;
                Header.unknown03 = 1999922179;
                writer.WriteHeader(ref Header);

                if (Header.numMeshes > 1)
                {
                    updateAsetHeader();
                    writer.Write(1413829441); // magic "ASET"
                    writer.WriteAsetHeader(ref AsetHeader);
                }

                for (int i = 0; i < Mesh.Count; i++)
                {
                    writer.Write(1230193997); // magic "MESI"
                    Mesh[i].Write(writer);
                }

                for (int i = 0; i < Material.Count; i++)
                {
                    writer.Write(1280463949); // magic "MTRL"
                    Material[i].Write(writer);
                }
            }
        }

        public void ExportToMax()
        {
            if (Mesh.Count > 1)
            {
                Maxscript.Command("frameRate = {0}", Math.Round(Mesh.Count / Mesh[0].extendedHeader.animTime));
                Maxscript.Interval(0, Mesh[0].extendedHeader.animTime);
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
                        Maxscript.AnimateAtTime(time, "meshOp.setVert {0} {1} {2}", mainObject, j + 1, Maxscript.NewPoint3Literal<float>(-Mesh[i].vertices[j].X, -Mesh[i].vertices[j].Z, Mesh[i].vertices[j].Y));
                        Maxscript.AnimateAtTime(time, "setNormal {0} {1} {2}", mainObject, j + 1, Maxscript.NewPoint3Literal<float>(-Mesh[i].normals[j].X, -Mesh[i].normals[j].Z, Mesh[i].normals[j].Y));

                        // When NOTFIRSTMESH (i aleady starts from 1)
                        if (Mesh[i].header.flags.HasFlag(BrgMeshFlag.ANIMTEXCOORDS))
                        {
                            if (Mesh[i].header.flags.HasFlag(BrgMeshFlag.TEXCOORDSA))
                            {
                                Maxscript.Animate("{0}.Unwrap_UVW.SetVertexPosition {1}s {2} {3}", mainObject, time, j + 1, Maxscript.NewPoint3Literal<float>(Mesh[i].texVertices[j].X, Mesh[i].texVertices[j].Y, 0));
                            }
                        }
                    }

                    foreach (BrgAttachpoint att in Mesh[i].Attachpoint)
                    {
                        Maxscript.Command("attachpoint = getNodeByName \"{0}\"", att.GetMaxName());
                        Maxscript.AnimateAtTime(time, "attachpoint.rotation = {0}", att.GetMaxTransform());
                        Maxscript.AnimateAtTime(time, "attachpoint.position = {0}", att.GetMaxPosition());
                        Maxscript.AnimateAtTime(time, "attachpoint.scale = {0}", att.GetMaxScale());
                    }

                    if (((Mesh[i].header.flags.HasFlag(BrgMeshFlag.COLORALPHACHANNEL) || Mesh[i].header.flags.HasFlag(BrgMeshFlag.COLORCHANNEL)) && !Mesh[i].header.flags.HasFlag(BrgMeshFlag.SECONDARYMESH))
                        || Mesh[i].header.flags.HasFlag(BrgMeshFlag.ANIMVERTCOLORALPHA))
                    {
                        for (int j = 0; j < Mesh[j].vertexColors.Length; j++)
                        {
                            if (Mesh[i].header.flags.HasFlag(BrgMeshFlag.COLORALPHACHANNEL))
                            {
                                Maxscript.AnimateAtTime(time, "meshop.setVertAlpha {0} -2 {1} {2}", mainObject, j + 1, Mesh[i].vertexColors[j].A);
                            }
                            else
                            {
                                Maxscript.AnimateAtTime(time, "meshop.setVertColor {0} 0 {1} (color {2} {3} {4})", mainObject, j + 1, Mesh[i].vertexColors[j].R, Mesh[i].vertexColors[j].G, Mesh[i].vertexColors[j].B);
                            }
                        }
                    }
                }

                // Still can't figure out why it updates/overwrites normals ( geometry:false topology:false)
                // Seems like it was fixed in 3ds Max 2015 with setNormal command
                Maxscript.Command("update {0} geometry:false topology:false normals:false", mainObject);
                Maxscript.Command("select {0}", mainObject);
                Maxscript.Command("max zoomext sel all");

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

            HashSet<float> keys = new HashSet<float>();
            if (ParentForm.Flags.HasFlag(BrgMeshFlag.ANIMTEXCOORDS))
            {
                int numTexKeys = Maxscript.QueryInteger("{0}.Unwrap_UVW[1].keys.count", mainObject);
                for (int i = 0; i < numTexKeys; i++)
                {
                    keys.Add(Maxscript.QueryFloat("{0}.Unwrap_UVW[1].keys[{1}].time as float", mainObject, i + 1) / 4800f);
                }
            }
            if (Maxscript.QueryBoolean("{0}.modifiers[#skin] != undefined", mainObject))
            {
                //System.Windows.Forms.MessageBox.Show("b1");
                Maxscript.Command("max modify mode");
                int numBones = Maxscript.QueryInteger("skinops.getnumberbones {0}.modifiers[#skin]", mainObject);
                for (int i = 0; i < numBones; i++)
                {
                    string boneName = Maxscript.QueryString("skinops.getbonename {0}.modifiers[#skin] {1} 0", mainObject, i + 1);
                    Maxscript.Command("boneNode = getNodeByName \"{0}\"", boneName);
                    int numBoneKeys = Maxscript.QueryInteger("boneNode.position.controller.keys.count");
                    //System.Windows.Forms.MessageBox.Show("b1.1");
                    for (int j = 0; j < numBoneKeys; j++)
                    {
                        keys.Add(Maxscript.QueryFloat("boneNode.position.controller.keys[{0}].time as float", j + 1) / 4800f);
                    }
                    numBoneKeys = Maxscript.QueryInteger("boneNode.rotation.controller.keys.count");
                    //System.Windows.Forms.MessageBox.Show("b1.2 " + numKeys);
                    for (int j = 0; j < numBoneKeys; j++)
                    {
                        //System.Windows.Forms.MessageBox.Show("b1.2.1 " + String.Format("boneNode.rotation.controller.keys[{0}] as float", j + 1));
                        keys.Add(Maxscript.QueryFloat("boneNode.rotation.controller.keys[{0}].time as float", j + 1) / 4800f);
                    }
                    numBoneKeys = Maxscript.QueryInteger("boneNode.scale.controller.keys.count");
                    //System.Windows.Forms.MessageBox.Show("b1.3");
                    for (int j = 0; j < numBoneKeys; j++)
                    {
                        //System.Windows.Forms.MessageBox.Show("b1.3.1");
                        keys.Add(Maxscript.QueryFloat("boneNode.scale.controller.keys[{0}].time as float", j + 1) / 4800f);
                    }
                    numBoneKeys = Maxscript.QueryInteger("boneNode.controller.keys.count");
                    //System.Windows.Forms.MessageBox.Show("b1.4");
                    for (int j = 0; j < numBoneKeys; j++)
                    {
                        //System.Windows.Forms.MessageBox.Show("b1.4.1");
                        keys.Add(Maxscript.QueryFloat("boneNode.controller.keys[{0}].time as float", j + 1) / 4800f);
                    }
                }
            }
            int numKeys = (int)Maxscript.Query("{0}.baseobject.mesh[1].keys.count", Maxscript.QueryType.Integer, mainObject);
            //System.Windows.Forms.MessageBox.Show(numKeys + " ");
            for (int i = 0; i < numKeys; i++)
            {
                keys.Add(Maxscript.QueryFloat("{0}.baseobject.mesh[1].keys[{1}].time as float", mainObject, i + 1) / 4800f);
            }
            Header.numMeshes = keys.Count;
            if (Header.numMeshes <= 0)
            {
                Header.numMeshes = 1;
                keys.Add(0);
            }
            List<float> keyTime = new List<float>(keys);
            keyTime.Sort();

            //fixTverts(mainObject);

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

            Mesh = new List<BrgMesh>(Header.numMeshes);
            for (int i = 0; i < Header.numMeshes; i++)
            {
                Mesh.Add(new BrgMesh(this));
                Mesh[i].ImportFromMax(mainObject, keyTime[i], i);
            }

            if (Mesh[0].header.properties.HasFlag(BrgMeshAnimType.NONUNIFORM))
            {
                Mesh[0].animTimeAdjust = new float[Mesh.Count];
                for (int i = 0; i < Mesh.Count; i++)
                {
                    Mesh[0].animTimeAdjust[i] = keyTime[i] / Mesh[0].extendedHeader.animTime;
                }
            }
            updateAsetHeader();

            //Maxscript.Command("delete {0}", mainObject);
        }
        private void fixTverts(string mainObject)
        {
            // Figure out the used vertices, and corresponding tex verts
            int numVertices = (int)Maxscript.Query("{0}.numverts", Maxscript.QueryType.Integer, mainObject);
            int numFaces = (int)Maxscript.Query("{0}.numfaces", Maxscript.QueryType.Integer, mainObject);
            List<int> vertexMask = new List<int>(numVertices);
            HashSet<int> verticesToBreak = new HashSet<int>();
            for (int i = 0; i < numVertices; i++)
            {
                vertexMask.Add(0);
            }
            for (int i = 0; i < numFaces; i++)
            {
                Maxscript.Command("face = getFace {0} {1}", mainObject, i + 1);
                Maxscript.Command("tFace = getTVFace {0} {1}", mainObject, i + 1);
                int vert1 = Maxscript.QueryInteger("face[1]") - 1;
                int vert2 = Maxscript.QueryInteger("face[2]") - 1;
                int vert3 = Maxscript.QueryInteger("face[3]") - 1;
                int tVert1 = Maxscript.QueryInteger("tFace[1]");
                int tVert2 = Maxscript.QueryInteger("tFace[2]");
                int tVert3 = Maxscript.QueryInteger("tFace[3]");
                if (vertexMask[vert1] > 0 && vertexMask[vert1] != tVert1)
                {
                    verticesToBreak.Add(vert1 + 1);
                }
                if (vertexMask[vert2] > 0 && vertexMask[vert2] != tVert2)
                {
                    verticesToBreak.Add(vert2 + 1);
                }
                if (vertexMask[vert3] > 0 && vertexMask[vert3] != tVert3)
                {
                    verticesToBreak.Add(vert3 + 1);
                }

                vertexMask[vert1] = tVert1;
                vertexMask[vert2] = tVert2;
                vertexMask[vert3] = tVert3;
            }
            if (verticesToBreak.Count > 0)
            {
                System.Windows.Forms.MessageBox.Show("dd");
                string vertsToBreak = Maxscript.NewBitArray("vertsToBreak");
                foreach (int i in verticesToBreak)
                {
                    Maxscript.Command("{0}[{1}] = true", vertsToBreak, i);
                    //Maxscript.Append(vertsToBreak, i);
                }
                Maxscript.Command("meshop.breakVerts {0} {1}", mainObject, vertsToBreak);
                //Maxscript.Command("print {0}", vertsToBreak);
            }
        }

        public float GetFrameTime(int meshIndex)
        {
            if (Mesh[0].header.properties.HasFlag(BrgMeshAnimType.NONUNIFORM))
            {
                //System.Windows.Forms.MessageBox.Show("t1");
                return Mesh[0].animTimeAdjust[meshIndex] * Mesh[0].extendedHeader.animTime;
            }
            else if (Mesh.Count > 1)
            {
                //System.Windows.Forms.MessageBox.Show("t2");
                return (float)meshIndex / ((float)Mesh.Count - 1f) * Mesh[0].extendedHeader.animTime;
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
            AsetHeader.animTime = Mesh[0].extendedHeader.animTime;
            AsetHeader.frequency = 1f / (float)AsetHeader.animTime;
            AsetHeader.spf = AsetHeader.animTime / (float)AsetHeader.numFrames;
            AsetHeader.fps = (float)AsetHeader.numFrames / AsetHeader.animTime;
        }
    }

    public class BrgMesh
    {
        public BrgFile ParentFile;

        public BrgMeshHeader header;
        public Vector3<float>[] vertices;
        public Vector3<float>[] normals;

        public Vector2<float>[] texVertices;
        public Int16[] faceMaterials;
        public Vector3<Int16>[] faceVertices;
        private Int16[] vertMaterials;

        public BrgMeshExtendedHeader extendedHeader;
        public BrgUserDataEntry[] UserDataEntries;
        float[] particleData;

        public VertexColor[] vertexColors; // unknown0a

        //public Int16 numMatrix;
        //Int16 numIndex;
        //public Int16 unknown10;
        public BrgAttachpointCollection Attachpoint;
        public float[] animTimeAdjust;

        public BrgMesh(BrgBinaryReader reader, BrgFile file)
        {
            ParentFile = file;

            reader.ReadMeshHeader(ref header);

            vertices = new Vector3<float>[0];
            normals = new Vector3<float>[0];
            texVertices = new Vector2<float>[0];
            faceMaterials = new Int16[0];
            faceVertices = new Vector3<short>[0];
            vertMaterials = new Int16[0];
            if (!header.flags.HasFlag(BrgMeshFlag.PARTICLEPOINTS))
            {
                vertices = new Vector3<float>[header.numVertices];
                for (int i = 0; i < header.numVertices; i++)
                {
                    reader.ReadVector3(out vertices[i], true, header.version == 22);
                }
                normals = new Vector3<float>[header.numVertices];
                for (int i = 0; i < header.numVertices; i++)
                {
                    if (header.version >= 13 && header.version <= 17)
                    {
                        reader.ReadInt16(); // No idea what this is
                    }
                    else // v == 18, 19 or 22
                    {
                        reader.ReadVector3(out normals[i], true, header.version == 22);
                    }
                }

                if (!header.flags.HasFlag(BrgMeshFlag.SECONDARYMESH) || header.flags.HasFlag(BrgMeshFlag.ANIMTEXCOORDS) || header.flags.HasFlag(BrgMeshFlag.PARTICLESYSTEM))
                {
                    if (header.flags.HasFlag(BrgMeshFlag.TEXCOORDSA))
                    {
                        texVertices = new Vector2<float>[header.numVertices];
                        for (int i = 0; i < header.numVertices; i++)
                        {
                            reader.ReadVector2(out texVertices[i], header.version == 22);
                        }
                    }
                }

                if (!header.flags.HasFlag(BrgMeshFlag.SECONDARYMESH) || header.flags.HasFlag(BrgMeshFlag.PARTICLESYSTEM))
                {
                    if (header.flags.HasFlag(BrgMeshFlag.MATERIAL))
                    {
                        faceMaterials = new Int16[header.numFaces];
                        for (int i = 0; i < header.numFaces; i++)
                        {
                            faceMaterials[i] = reader.ReadInt16();
                        }
                    }

                    faceVertices = new Vector3<Int16>[header.numFaces];
                    for (int i = 0; i < header.numFaces; i++)
                    {
                        reader.ReadVector3(out faceVertices[i]);
                    }

                    if (header.flags.HasFlag(BrgMeshFlag.MATERIAL))
                    {
                        vertMaterials = new Int16[header.numVertices];
                        for (int i = 0; i < header.numVertices; i++)
                        {
                            vertMaterials[i] = reader.ReadInt16();
                        }
                    }
                }
            }

            UserDataEntries = new BrgUserDataEntry[header.userDataEntryCount];
            if (!header.flags.HasFlag(BrgMeshFlag.PARTICLEPOINTS))
            {
                for (int i = 0; i < header.userDataEntryCount; i++)
                {
                    UserDataEntries[i] = reader.ReadUserDataEntry(false);
                }
            }

            reader.ReadMeshExtendedHeader(ref extendedHeader, header.extendedHeaderSize);

            particleData = new float[0];
            if (header.flags.HasFlag(BrgMeshFlag.PARTICLEPOINTS))
            {
                particleData = new float[4 * header.numVertices];
                for (int i = 0; i < particleData.Length; i++)
                {
                    particleData[i] = reader.ReadSingle();
                }
                for (int i = 0; i < header.userDataEntryCount; i++)
                {
                    UserDataEntries[i] = reader.ReadUserDataEntry(true);
                }
            }

            if (header.version == 13)
            {
                reader.ReadBytes(extendedHeader.nameLength);
            }

            if (header.version >= 13 && header.version <= 18)
            {
                header.flags |= BrgMeshFlag.ATTACHPOINTS;
            }
            if (header.flags.HasFlag(BrgMeshFlag.ATTACHPOINTS))
            {
                Int16 numMatrix = extendedHeader.numMatrix;
                Int16 numIndex = extendedHeader.numIndex;
                if (header.version == 19 || header.version == 22)
                {
                    numMatrix = reader.ReadInt16();
                    numIndex = reader.ReadInt16();
                    reader.ReadInt16();
                }

                BrgAttachpoint[] attpts = new BrgAttachpoint[numMatrix];
                for (int i = 0; i < numMatrix; i++)
                {
                    attpts[i] = new BrgAttachpoint();
                }
                for (int i = 0; i < numMatrix; i++)
                {
                    reader.ReadVector3(out attpts[i].x, true, header.version == 22);
                }
                for (int i = 0; i < numMatrix; i++)
                {
                    reader.ReadVector3(out attpts[i].y, true, header.version == 22);
                }
                for (int i = 0; i < numMatrix; i++)
                {
                    reader.ReadVector3(out attpts[i].z, true, header.version == 22);
                }
                if (header.version == 19 || header.version == 22)
                {
                    for (int i = 0; i < numMatrix; i++)
                    {
                        reader.ReadVector3(out attpts[i].position, true, header.version == 22);
                    }
                }
                for (int i = 0; i < numMatrix; i++)
                {
                    reader.ReadVector3(out attpts[i].unknown11a, true, header.version == 22);
                }
                for (int i = 0; i < numMatrix; i++)
                {
                    reader.ReadVector3(out attpts[i].unknown11b, true, header.version == 22);
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

                Attachpoint = new BrgAttachpointCollection();
                for (int i = 0; i < nameId.Count; i++)
                {
                    Attachpoint.Add(new BrgAttachpoint(attpts[reader.ReadByte()]));
                    Attachpoint[i].NameId = nameId[i];
                    //attpts[reader.ReadByte()].NameId = nameId[i];
                }
                //attachpoints = new List<BrgAttachpoint>(attpts);
            }
            else
            {
                Attachpoint = new BrgAttachpointCollection();
            }

            vertexColors = new VertexColor[0];
            if (((header.flags.HasFlag(BrgMeshFlag.COLORALPHACHANNEL) || header.flags.HasFlag(BrgMeshFlag.COLORCHANNEL)) && !header.flags.HasFlag(BrgMeshFlag.SECONDARYMESH))
                || header.flags.HasFlag(BrgMeshFlag.ANIMVERTCOLORALPHA))
            {
                vertexColors = new VertexColor[header.numVertices];
                for (int i = 0; i < header.numVertices; i++)
                {
                    reader.ReadVertexColor(out vertexColors[i]);
                }
            }

            // Only seen on first mesh
            animTimeAdjust = new float[0];
            if (header.properties.HasFlag(BrgMeshAnimType.NONUNIFORM))
            {
                animTimeAdjust = new float[extendedHeader.nonUniformKeyCount];
                for (int i = 0; i < extendedHeader.nonUniformKeyCount; i++)
                {
                    animTimeAdjust[i] = reader.ReadSingle();
                }
            }

            if (header.version >= 14 && header.version <= 19)
            {
                // Face Normals??
                Vector3<float>[] legacy = new Vector3<float>[header.numFaces];
                for (int i = 0; i < header.numFaces; i++)
                {
                    reader.ReadVector3(out legacy[i]);
                }
            }

            if (!header.flags.HasFlag(BrgMeshFlag.SECONDARYMESH) && header.version != 22)
            {
                reader.ReadBytes(extendedHeader.shadowNameLength0 + extendedHeader.shadowNameLength1);
            }
        }
        public BrgMesh(BrgFile file)
        {
            ParentFile = file;
            header.version = 22;
            header.extendedHeaderSize = 40;

            vertices = new Vector3<float>[0];
            normals = new Vector3<float>[0];

            texVertices = new Vector2<float>[0];
            faceMaterials = new Int16[0];
            faceVertices = new Vector3<Int16>[0];
            vertMaterials = new Int16[0];

            extendedHeader.materialLibraryTimestamp = 191738312;
            extendedHeader.exportedScaleFactor = 1f;

            UserDataEntries = new BrgUserDataEntry[0];
            particleData = new float[0];

            Vector2<float>[] unknown0a = new Vector2<float>[0];
            Attachpoint = new BrgAttachpointCollection();
            animTimeAdjust = new float[0];
        }

        public void Write(BrgBinaryWriter writer)
        {
            header.version = 22;
            if (header.flags.HasFlag(BrgMeshFlag.PARTICLEPOINTS))
            {
                header.numVertices = (Int16)(particleData.Length / 4);
            }
            else
            {
                header.numVertices = (Int16)vertices.Length;
            }
            header.numFaces = (Int16)faceVertices.Length;
            header.userDataEntryCount = (Int16)UserDataEntries.Length;
            header.centerRadius = header.boundingBoxMax.LongestAxisLength();
            header.extendedHeaderSize = 40;
            writer.WriteMeshHeader(ref header);

            if (!header.flags.HasFlag(BrgMeshFlag.PARTICLEPOINTS))
            {
                for (int i = 0; i < vertices.Length; i++)
                {
                    writer.WriteVector3(ref vertices[i], true, true);
                }
                for (int i = 0; i < vertices.Length; i++)
                {
                    writer.WriteVector3(ref normals[i], true, true);
                }

                if (!header.flags.HasFlag(BrgMeshFlag.SECONDARYMESH) || header.flags.HasFlag(BrgMeshFlag.ANIMTEXCOORDS) || header.flags.HasFlag(BrgMeshFlag.PARTICLESYSTEM))
                {
                    if (header.flags.HasFlag(BrgMeshFlag.TEXCOORDSA))
                    {
                        for (int i = 0; i < texVertices.Length; i++)
                        {
                            writer.WriteVector2(ref texVertices[i], true);
                        }
                    }
                }

                if (!header.flags.HasFlag(BrgMeshFlag.SECONDARYMESH) || header.flags.HasFlag(BrgMeshFlag.PARTICLESYSTEM))
                {
                    if (header.flags.HasFlag(BrgMeshFlag.MATERIAL))
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

                    if (header.flags.HasFlag(BrgMeshFlag.MATERIAL))
                    {
                        for (int i = 0; i < vertMaterials.Length; i++)
                        {
                            writer.Write(vertMaterials[i]);
                        }
                    }
                }
            }

            if (!header.flags.HasFlag(BrgMeshFlag.PARTICLEPOINTS))
            {
                for (int i = 0; i < UserDataEntries.Length; i++)
                {
                    writer.WriteUserDataEntry(ref UserDataEntries[i], false);
                }
            }

            extendedHeader.nonUniformKeyCount = animTimeAdjust.Length;
            writer.WriteMeshExtendedHeader(ref extendedHeader);

            if (header.flags.HasFlag(BrgMeshFlag.PARTICLEPOINTS))
            {
                for (int i = 0; i < particleData.Length; i++)
                {
                    writer.Write(particleData[i]);
                }
                for (int i = 0; i < UserDataEntries.Length; i++)
                {
                    writer.WriteUserDataEntry(ref UserDataEntries[i], true);
                }
            }

            if (header.flags.HasFlag(BrgMeshFlag.ATTACHPOINTS))
            {
                writer.Write((Int16)Attachpoint.Count);

                List<int> nameId = new List<int>();
                int maxNameId = -1;
                foreach (BrgAttachpoint att in Attachpoint)
                {
                    nameId.Add(att.NameId);
                    if (att.NameId > maxNameId)
                    {
                        maxNameId = att.NameId;
                    }
                }
                Int16 numIndex = (Int16)(maxNameId + 1);//(Int16)(55 - maxNameId);
                writer.Write((Int16)numIndex);
                writer.Write((Int16)1);


                foreach (BrgAttachpoint att in Attachpoint)
                {
                    writer.WriteVector3(ref att.x, true, true);
                }
                foreach (BrgAttachpoint att in Attachpoint)
                {
                    writer.WriteVector3(ref att.y, true, true);
                }
                foreach (BrgAttachpoint att in Attachpoint)
                {
                    writer.WriteVector3(ref att.z, true, true);
                }
                foreach (BrgAttachpoint att in Attachpoint)
                {
                    writer.WriteVector3(ref att.position, true, true);
                }
                foreach (BrgAttachpoint att in Attachpoint)
                {
                    writer.WriteVector3(ref att.unknown11a, true, true);
                }
                foreach (BrgAttachpoint att in Attachpoint)
                {
                    writer.WriteVector3(ref att.unknown11b, true, true);
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
                for (int i = 0; i < Attachpoint.Count; i++)
                {
                    for (int j = 0; j < Attachpoint.Count; j++)
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

            if (((header.flags.HasFlag(BrgMeshFlag.COLORALPHACHANNEL) || header.flags.HasFlag(BrgMeshFlag.COLORCHANNEL)) && !header.flags.HasFlag(BrgMeshFlag.SECONDARYMESH))
                || header.flags.HasFlag(BrgMeshFlag.ANIMVERTCOLORALPHA))
            {
                for (int i = 0; i < vertexColors.Length; i++)
                {
                    writer.WriteVertexColor(ref vertexColors[i]);
                }
            }

            if (header.properties.HasFlag(BrgMeshAnimType.NONUNIFORM))
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
                Maxscript.Append(normArray, Maxscript.NewPoint3<float>("n", -norm.X, -norm.Z, norm.Y));
            }
            if (!header.flags.HasFlag(BrgMeshFlag.SECONDARYMESH) || header.flags.HasFlag(BrgMeshFlag.ANIMTEXCOORDS))
            {
                if (header.flags.HasFlag(BrgMeshFlag.TEXCOORDSA))
                {
                    Maxscript.CommentTitle("Load UVW");
                    foreach (Vector2<float> texVert in texVertices)
                    {
                        Maxscript.Append(texVerts, Maxscript.NewPoint3<float>("tV", texVert.X, texVert.Y, 0));
                    }
                }
            }

            if (!header.flags.HasFlag(BrgMeshFlag.SECONDARYMESH))
            {
                if (header.flags.HasFlag(BrgMeshFlag.MATERIAL))
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
                    Maxscript.Append(faceArray, Maxscript.NewPoint3<Int32>("fV", face.X + 1, face.Z + 1, face.Y + 1));
                }
            }

            if (header.flags.HasFlag(BrgMeshFlag.ATTACHPOINTS))
            {
                Maxscript.CommentTitle("Load Attachpoints");
                foreach (BrgAttachpoint att in Attachpoint)
                {
                    string attachDummy = Maxscript.NewDummy("attachDummy", att.GetMaxName(), att.GetMaxTransform(), att.GetMaxPosition(), att.GetMaxBoxSize(), att.GetMaxScale());
                }
            }

            string mainObject = "mainObj";
            Maxscript.AnimateAtTime(ParentFile.GetFrameTime(0), Maxscript.NewMeshLiteral(mainObject, vertArray, normArray, faceArray, faceMats, texVerts));
            Maxscript.Command("{0} = getNodeByName \"{0}\"", mainObject);

            if (((header.flags.HasFlag(BrgMeshFlag.COLORALPHACHANNEL) || header.flags.HasFlag(BrgMeshFlag.COLORCHANNEL)) && !header.flags.HasFlag(BrgMeshFlag.SECONDARYMESH))
                || header.flags.HasFlag(BrgMeshFlag.ANIMVERTCOLORALPHA))
            {
                //Maxscript.Command("{0}.showVertexColors = true", mainObject);
                for (int i = 0; i < vertexColors.Length; i++)
                {
                    if (header.flags.HasFlag(BrgMeshFlag.COLORALPHACHANNEL))
                    {
                        //Maxscript.Command("meshop.supportVAlphas {0}", mainObject);
                        Maxscript.Command("meshop.setVertAlpha {0} -2 {1} {2}", mainObject, i + 1, vertexColors[i].A);
                    }
                    else
                    {
                        Maxscript.Command("meshop.setVertColor {0} 0 {1} (color {2} {3} {4})", mainObject, i + 1, vertexColors[i].R, vertexColors[i].G, vertexColors[i].B);
                    }
                }
            }

            string groundPlanePos = Maxscript.NewPoint3<float>("groundPlanePos", -header.groundPos.X, -header.groundPos.Z, header.groundPos.Y);
            Maxscript.Command("plane name:\"ground\" pos:{0} length:10 width:10", groundPlanePos);

            Maxscript.CommentTitle("TVert Hack");
            Maxscript.Command("buildTVFaces {0}", mainObject);
            for (int i = 1; i <= faceVertices.Length; i++)
            {
                Maxscript.Command("setTVFace {0} {1} (getFace {0} {1})", mainObject, i);
            }

            if (header.flags.HasFlag(BrgMeshFlag.ANIMTEXCOORDS))
            {
                Maxscript.Command("select {0}", mainObject);
                Maxscript.Command("addModifier {0} (Unwrap_UVW())", mainObject);

                Maxscript.Command("select {0}.verts", mainObject);
                Maxscript.Animate("{0}.Unwrap_UVW.moveSelected [0,0,0]", mainObject);
            }

            return mainObject;
        }
        public void ImportFromMax(string mainObject, float time, int meshIndex)
        {
            UpdateSettings(meshIndex);

            header.centerPos.X = -(float)Maxscript.Query("{0}.center.x", Maxscript.QueryType.Float, mainObject);
            header.centerPos.Z = -(float)Maxscript.Query("{0}.center.y", Maxscript.QueryType.Float, mainObject);
            header.centerPos.Y = (float)Maxscript.Query("{0}.center.z", Maxscript.QueryType.Float, mainObject);

            Maxscript.Command("grnd = getNodeByName \"ground\"");
            if (!Maxscript.QueryBoolean("grnd == undefined"))
            {
                header.groundPos.X = -(float)Maxscript.Query("grnd.position.x", Maxscript.QueryType.Float);
                header.groundPos.Z = -(float)Maxscript.Query("grnd.position.y", Maxscript.QueryType.Float);
                header.groundPos.Y = (float)Maxscript.Query("grnd.position.z", Maxscript.QueryType.Float);
            }

            Maxscript.Command("{0}BBMax = {0}.max", mainObject);
            Maxscript.Command("{0}BBMin = {0}.min", mainObject);
            Vector3<float> bBoxMax = new Vector3<float>(Maxscript.QueryFloat("{0}BBMax.X", mainObject), Maxscript.QueryFloat("{0}BBMax.Y", mainObject), Maxscript.QueryFloat("{0}BBMax.Z", mainObject));
            Vector3<float> bBoxMin = new Vector3<float>(Maxscript.QueryFloat("{0}BBMin.X", mainObject), Maxscript.QueryFloat("{0}BBMin.Y", mainObject), Maxscript.QueryFloat("{0}BBMin.Z", mainObject));
            Vector3<float> bBox = (bBoxMax - bBoxMin) / 2;
            header.boundingBoxMin.X = -bBox.X;
            header.boundingBoxMin.Z = -bBox.Y;
            header.boundingBoxMin.Y = -bBox.Z;
            header.boundingBoxMax.X = bBox.X;
            header.boundingBoxMax.Z = bBox.Y;
            header.boundingBoxMax.Y = bBox.Z;

            string mainMesh = Maxscript.SnapshotAsMesh("mainMesh", mainObject, time);
            int numVertices = (int)Maxscript.Query("{0}.numverts", Maxscript.QueryType.Integer, mainMesh);
            int numFaces = (int)Maxscript.Query("{0}.numfaces", Maxscript.QueryType.Integer, mainMesh);
            string attachDummy = Maxscript.NewArray("attachDummy");
            Maxscript.SetVarAtTime(time, attachDummy, "$helpers/atpt??* as array");
            int numAttachpoints = (int)Maxscript.Query("{0}.count", Maxscript.QueryType.Integer, attachDummy);

            // Figure out the used vertices, and corresponding tex verts
            setUpTVerts(mainMesh, numVertices, numFaces);
            numVertices = Maxscript.QueryInteger("{0}.numverts", mainMesh);
            numFaces = Maxscript.QueryInteger("{0}.numfaces", mainMesh);
            //System.Windows.Forms.MessageBox.Show(numVertices + " " + numFaces);
            List<int> vertexMask = new List<int>(numVertices);
            for (int i = 0; i < numVertices; i++)
            {
                vertexMask.Add(0);
            }
            for (int i = 0; i < numFaces; i++)
            {
                Maxscript.Command("face = getFace {0} {1}", mainMesh, i + 1);
                Maxscript.Command("tFace = getTVFace {0} {1}", mainMesh, i + 1);
                int vert1 = Maxscript.QueryInteger("face[1]") - 1;
                int vert2 = Maxscript.QueryInteger("face[2]") - 1;
                int vert3 = Maxscript.QueryInteger("face[3]") - 1;
                int tVert1 = Maxscript.QueryInteger("tFace[1]");
                int tVert2 = Maxscript.QueryInteger("tFace[2]");
                int tVert3 = Maxscript.QueryInteger("tFace[3]");

                vertexMask[vert1] = tVert1;
                vertexMask[vert2] = tVert2;
                vertexMask[vert3] = tVert3;
            }

            //System.Windows.Forms.MessageBox.Show("1 " + numVertices);
            List<Vector3<float>> verticesList = new List<Vector3<float>>(numVertices);
            List<Vector3<float>> normalsList = new List<Vector3<float>>(numVertices);
            Dictionary<int, Int16> newVertMap = new Dictionary<int, Int16>(numVertices);
            for (int i = 0; i < numVertices; i++)
            {
                //System.Windows.Forms.MessageBox.Show("1.1");
                //System.Windows.Forms.MessageBox.Show("1.2");
                if (vertexMask[i] > 0)
                {
                    newVertMap.Add(i, (Int16)verticesList.Count);
                    //System.Windows.Forms.MessageBox.Show("1.3");
                    Maxscript.Command("vertex = getVert {0} {1}", mainMesh, i + 1);
                    //System.Windows.Forms.MessageBox.Show("1.4");
                    verticesList.Add(new Vector3<float>(-Maxscript.QueryFloat("vertex.x"), Maxscript.QueryFloat("vertex.z"), -Maxscript.QueryFloat("vertex.y")));

                    //System.Windows.Forms.MessageBox.Show("1.5");
                    // Snapshot of Mesh recalculates normals, so do it based on time from the real object
                    Maxscript.SetVarAtTime(time, "normal", "normalize (getNormal {0} {1})", mainObject, i + 1);
                    //System.Windows.Forms.MessageBox.Show("1.6");
                    normalsList.Add(new Vector3<float>(-Maxscript.QueryFloat("normal.x"), Maxscript.QueryFloat("normal.z"), -Maxscript.QueryFloat("normal.y")));
                    //System.Windows.Forms.MessageBox.Show("1.7");
                }
            }
            vertices = verticesList.ToArray();
            normals = normalsList.ToArray();

            //System.Windows.Forms.MessageBox.Show("2");
            if (!header.flags.HasFlag(BrgMeshFlag.SECONDARYMESH) || header.flags.HasFlag(BrgMeshFlag.ANIMTEXCOORDS))
            {
                if (header.flags.HasFlag(BrgMeshFlag.TEXCOORDSA))
                {
                    List<Vector2<float>> texVerticesList = new List<Vector2<float>>(numVertices);
                    for (int i = 0; i < numVertices; i++)
                    {
                        if (vertexMask[i] > 0)
                        {
                            Maxscript.Command("tVert = getTVert {0} {1}", mainMesh, vertexMask[i]);
                            texVerticesList.Add(new Vector2<float>(Maxscript.QueryFloat("tVert.x"), Maxscript.QueryFloat("tVert.y")));
                        }
                    }
                    texVertices = texVerticesList.ToArray();
                }
            }

            //System.Windows.Forms.MessageBox.Show("3");
            HashSet<int> diffFaceMats = new HashSet<int>();
            if (!header.flags.HasFlag(BrgMeshFlag.SECONDARYMESH))
            {
                if (header.flags.HasFlag(BrgMeshFlag.MATERIAL))
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
                    faceVertices[i].X = newVertMap[((Int32)Maxscript.Query("face.x", Maxscript.QueryType.Integer) - 1)];
                    faceVertices[i].Y = newVertMap[((Int32)Maxscript.Query("face.z", Maxscript.QueryType.Integer) - 1)];
                    faceVertices[i].Z = newVertMap[((Int32)Maxscript.Query("face.y", Maxscript.QueryType.Integer) - 1)];
                }

                //System.Windows.Forms.MessageBox.Show("3.2");
                if (header.flags.HasFlag(BrgMeshFlag.MATERIAL))
                {
                    vertMaterials = new Int16[vertices.Length];
                    for (int i = 0; i < faceVertices.Length; i++)
                    {
                        vertMaterials[faceVertices[i].X] = faceMaterials[i];
                        vertMaterials[faceVertices[i].Y] = faceMaterials[i];
                        vertMaterials[faceVertices[i].Z] = faceMaterials[i];
                    }
                }
            }

            //System.Windows.Forms.MessageBox.Show("4");
            if (meshIndex == 0 && diffFaceMats.Count > 0)
            {
                extendedHeader.materialCount = (byte)(diffFaceMats.Count - 1);
                extendedHeader.uniqueMaterialCount = diffFaceMats.Count;
            }
            extendedHeader.animTime = (float)Maxscript.Query("animationRange.end.ticks / 4800 as float", Maxscript.QueryType.Float);

            //System.Windows.Forms.MessageBox.Show("5 " + numAttachpoints);
            if (header.flags.HasFlag(BrgMeshFlag.ATTACHPOINTS))
            {
                Attachpoint = new BrgAttachpointCollection();
                for (int i = 0; i < numAttachpoints; i++)
                {
                    int index = Convert.ToInt32((Maxscript.QueryString("{0}[{1}].name", attachDummy, i + 1)).Substring(4, 2));
                    index = Attachpoint.Add(index);
                    //System.Windows.Forms.MessageBox.Show("5.1");
                    Attachpoint[index].NameId = BrgAttachpoint.GetIdByName(((string)Maxscript.Query("{0}[{1}].name", Maxscript.QueryType.String, attachDummy, i + 1)).Substring(7));
                    Maxscript.Command("{0}[{1}].name = \"{2}\"", attachDummy, i + 1, Attachpoint[index].GetMaxName());
                    //System.Windows.Forms.MessageBox.Show("5.2");
                    Maxscript.SetVarAtTime(time, "{0}Transform", "{0}[{1}].rotation as matrix3", attachDummy, i + 1);
                    Maxscript.SetVarAtTime(time, "{0}Position", "{0}[{1}].position", attachDummy, i + 1);
                    Maxscript.SetVarAtTime(time, "{0}Scale", "{0}[{1}].scale", attachDummy, i + 1);
                    //System.Windows.Forms.MessageBox.Show("5.3");
                    Vector3<float> scale = new Vector3<float>(Maxscript.QueryFloat("{0}Scale.X", attachDummy), Maxscript.QueryFloat("{0}Scale.Y", attachDummy), Maxscript.QueryFloat("{0}Scale.Z", attachDummy));
                    bBox = scale / 2;
                    //System.Windows.Forms.MessageBox.Show("5.4");

                    Attachpoint[index].x.X = -(float)Maxscript.Query("{0}Transform[1].z", Maxscript.QueryType.Float, attachDummy);
                    Attachpoint[index].x.Y = (float)Maxscript.Query("{0}Transform[3].z", Maxscript.QueryType.Float, attachDummy);
                    Attachpoint[index].x.Z = -(float)Maxscript.Query("{0}Transform[2].z", Maxscript.QueryType.Float, attachDummy);

                    Attachpoint[index].y.X = -(float)Maxscript.Query("{0}Transform[1].y", Maxscript.QueryType.Float, attachDummy);
                    Attachpoint[index].y.Y = (float)Maxscript.Query("{0}Transform[3].y", Maxscript.QueryType.Float, attachDummy);
                    Attachpoint[index].y.Z = -(float)Maxscript.Query("{0}Transform[2].y", Maxscript.QueryType.Float, attachDummy);

                    Attachpoint[index].z.X = -(float)Maxscript.Query("{0}Transform[1].x", Maxscript.QueryType.Float, attachDummy);
                    Attachpoint[index].z.Y = (float)Maxscript.Query("{0}Transform[3].x", Maxscript.QueryType.Float, attachDummy);
                    Attachpoint[index].z.Z = -(float)Maxscript.Query("{0}Transform[2].x", Maxscript.QueryType.Float, attachDummy);

                    Attachpoint[index].position.X = -(float)Maxscript.Query("{0}Position.x", Maxscript.QueryType.Float, attachDummy);
                    Attachpoint[index].position.Z = -(float)Maxscript.Query("{0}Position.y", Maxscript.QueryType.Float, attachDummy);
                    Attachpoint[index].position.Y = (float)Maxscript.Query("{0}Position.z", Maxscript.QueryType.Float, attachDummy);
                    //System.Windows.Forms.MessageBox.Show("5.5");

                    Attachpoint[index].unknown11a.X = -bBox.X;
                    Attachpoint[index].unknown11a.Z = -bBox.Y;
                    Attachpoint[index].unknown11a.Y = -bBox.Z;
                    Attachpoint[index].unknown11b.X = bBox.X;
                    Attachpoint[index].unknown11b.Z = bBox.Y;
                    Attachpoint[index].unknown11b.Y = bBox.Z;
                }
                //System.Windows.Forms.MessageBox.Show("# Atpts: " + Attachpoint.Count);
            }
        }
        private void setUpTVerts(string mainMesh, int numVertices, int numFaces)
        {
            // Reduce Duplicate TVerts
            //string uniqueTVerts = Maxscript.NewArray("uTVerts");
            //string uTVIndex = Maxscript.NewBitArray("uTVIndex");
            //Maxscript.Command("{0}.count = meshop.getNumTverts {1}", uTVIndex, mainMesh);
            //Maxscript.Command("for t = 1 to (meshop.getNumTverts {0}) do ( if(appendIfUnique {1} (getTVert {0} t)) do (append {2} t))", mainMesh, uniqueTVerts, uTVIndex);
            //Maxscript.Command("for t = 1 to (meshop.getNumTverts {0}) do (appendIfUnique {1} (getTVert {0} t))", mainMesh, uniqueTVerts);
            /*Maxscript.Command("meshop.setNumTverts {0} {1}.count", mainMesh, uniqueTVerts);
            Maxscript.Command("for t = 1 to (meshop.getNumTverts {0}) do (setTVert {0} t {1}[t]", mainMesh, uniqueTVerts);
            Maxscript.Command("buildTVFaces {0}", mainMesh);
            for (int i = 1; i <= faceVertices.Length; i++)
            {
                Maxscript.Command("setTVFace {0} {1} (getFace {0} {1})", mainMesh, i);
            }*/
            // Figure out the used vertices, and corresponding tex verts
            OrderedSet<int>[] vertUsage = new OrderedSet<int>[numVertices];
            OrderedSet<int>[] faceUsage = new OrderedSet<int>[numVertices];
            for (int i = 0; i < vertUsage.Length; i++)
            {
                vertUsage[i] = new OrderedSet<int>();
            }
            for (int i = 0; i < faceUsage.Length; i++)
            {
                faceUsage[i] = new OrderedSet<int>();
            }

            for (int i = 0; i < numFaces; i++)
            {
                Maxscript.Command("face = getFace {0} {1}", mainMesh, i + 1);
                Maxscript.Command("tFace = meshop.getMapFace {0} 1 {1}", mainMesh, i + 1);
                int vert1 = Maxscript.QueryInteger("face[1]") - 1;
                int vert2 = Maxscript.QueryInteger("face[2]") - 1;
                int vert3 = Maxscript.QueryInteger("face[3]") - 1;
                int tVert1 = Maxscript.QueryInteger("tFace[1]");
                int tVert2 = Maxscript.QueryInteger("tFace[2]");
                int tVert3 = Maxscript.QueryInteger("tFace[3]");
                try
                {
                    vertUsage[vert1].Add(tVert1);
                    vertUsage[vert2].Add(tVert2);
                    vertUsage[vert3].Add(tVert3);
                    faceUsage[vert1].Add(i + 1);
                    faceUsage[vert2].Add(i + 1);
                    faceUsage[vert3].Add(i + 1);
                }
                catch { throw new Exception(vertUsage.Length + " " + faceUsage.Length + " " + i + " " + vert1 + " " + vert2 + " " + vert3); }
            }

            for (int i = 0; i < vertUsage.Length; i++)
            {
                if (vertUsage[i].Count > 1)
                {
                    Maxscript.Command("vertPos = getVert {0} {1}", mainMesh, i + 1);
                    for (int mapV = 1; mapV < vertUsage[i].Count; mapV++)
                    {
                        int mapVert = vertUsage[i][mapV];
                        //if (!Maxscript.QueryBoolean("{0}[{1}] == true", uTVIndex, mapVert))
                          //  continue;
                        int newIndex = Maxscript.QueryInteger("{0}.numverts", mainMesh) + 1;
                        Maxscript.Command("setNumVerts {0} {1} true", mainMesh, newIndex);
                        Maxscript.Command("setVert {0} {1} {2}", mainMesh, newIndex, "vertPos");
                        for (int f = 0; f < faceUsage[i].Count; f++)
                        {
                            int face = faceUsage[i][f];
                            Maxscript.Command("theFaceDef = getFace {0} {1}", mainMesh, face);
                            Maxscript.Command("theMapFaceDef = meshOp.getMapFace {0} 1 {1}", mainMesh, face);
                            if (Maxscript.QueryInteger("theMapFaceDef.x") == mapVert && Maxscript.QueryInteger("theFaceDef.x") == i + 1)
                            {
                                Maxscript.Command("theFaceDef.x = {0}", newIndex);
                                Maxscript.Command("setFace {0} {1} theFaceDef", mainMesh, face);
                            }
                            if (Maxscript.QueryInteger("theMapFaceDef.y") == mapVert && Maxscript.QueryInteger("theFaceDef.y") == i + 1)
                            {
                                Maxscript.Command("theFaceDef.y = {0}", newIndex);
                                Maxscript.Command("setFace {0} {1} theFaceDef", mainMesh, face);
                            }
                            if (Maxscript.QueryInteger("theMapFaceDef.z") == mapVert && Maxscript.QueryInteger("theFaceDef.z") == i + 1)
                            {
                                Maxscript.Command("theFaceDef.z = {0}", newIndex);
                                Maxscript.Command("setFace {0} {1} theFaceDef", mainMesh, face);
                            }
                        }
                    }
                }
            }
        }

        public void UpdateSettings(int meshIndex)
        {
            header.flags = ParentFile.ParentForm.Flags;
            header.format = ParentFile.ParentForm.Format;
            header.properties = ParentFile.ParentForm.Properties;
            header.interpolationType = ParentFile.ParentForm.InterpolationType;
            extendedHeader.exportedScaleFactor = ParentFile.ParentForm.TimeMult;
            if (meshIndex > 0)
            {
                header.flags |= BrgMeshFlag.SECONDARYMESH;
                header.properties &= ~BrgMeshAnimType.NONUNIFORM;
            }
        }

        public class BrgAttachpointCollection : IEnumerable
        {
            private BrgAttachpoint[] attachpoint;
            public readonly static int Capacity = 100;
            public int Count;

            internal BrgAttachpointCollection()
            {
                attachpoint = new BrgAttachpoint[Capacity];
            }

            public void Add()
            {
                Add(Count, new BrgAttachpoint());
            }
            public int Add(int index)
            {
                return Add(index, new BrgAttachpoint());
            }
            public void Add(BrgAttachpoint att)
            {
                Add(Count, att);
            }
            public int Add(int index, BrgAttachpoint att)
            {
                if (Count > 100)
                {
                    throw new Exception("Reached max attachpoint capacity!");
                }
                att.Index = index;
                if (attachpoint[att.Index] != null)
                {
                    att.Index = 0;
                    while (attachpoint[att.Index] != null)
                    {
                        att.Index++;
                    }
                }
                attachpoint[att.Index] = att;
                Count++;
                return att.Index;
            }
            public void Remove(int index)
            {
                attachpoint[index] = null;
                Count--;
            }
            public BrgAttachpoint this[int index]
            {
                get
                {
                    return attachpoint[index];
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
            public IEnumerator GetEnumerator()
            {
                foreach (BrgAttachpoint att in attachpoint)
                {
                    if (att != null)
                        yield return att;
                }
                //return attachpoint.GetEnumerator();
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

        public int Index;
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
                if (NameId >= 0 && NameId <= 54)
                {
                    return AttachpointNames[54 - NameId];
                }
                else
                {
                    //return string.Empty;
                    throw new Exception("Invalid Attachpoint Name Id!");
                }
            }
        }
        public string MaxName
        {
            get
            {
                return GetMaxName();
            }
        }

        public BrgAttachpoint()
        {
            Index = -1;
            NameId = -1;
            x = new Vector3<float>(0, 1, 0);
            y = new Vector3<float>(0, 0, -1);
            z = new Vector3<float>(-1, 0, 0);
            position = new Vector3<float>(0f);
            unknown11a = new Vector3<float>(-0.25f);
            unknown11b = new Vector3<float>(0.25f);
        }
        public BrgAttachpoint(BrgAttachpoint prev)
        {
            Index = prev.Index;
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

            //return -1;
            throw new Exception("Invalid Attachpoint Name Id!");
        }

        public string GetMaxTransform()
        {
            //string xVector = Maxscript.NewPoint3<float>("xVector", this.z.X, this.y.X, this.x.X); // original
            //string yVector = Maxscript.NewPoint3<float>("yVector", this.z.Z, this.y.Z, this.x.Z);
            //string zVector = Maxscript.NewPoint3<float>("zVector", this.z.Y, this.y.Y, this.x.Y);

            string xVector = Maxscript.NewPoint3<float>("xVector", -this.z.X, -this.y.X, -this.x.X);
            string yVector = Maxscript.NewPoint3<float>("yVector", -this.z.Z, -this.y.Z, -this.x.Z);
            string zVector = Maxscript.NewPoint3<float>("zVector", this.z.Y, this.y.Y, this.x.Y);
            string posVector = Maxscript.NewPoint3<float>("rotPosVect", 0, 0, 0);
            return Maxscript.NewMatrix3("transformMatrix", xVector, yVector, zVector, posVector);
        }
        public string GetMaxPosition()
        {
            return Maxscript.NewPoint3<float>("posVector", -this.position.X, -this.position.Z, this.position.Y);
        }
        public string GetMaxBoxSize()
        {
            return Maxscript.NewPoint3<float>("boxSize", 1, 1, 1);
        }
        public string GetMaxScale()
        {
            return Maxscript.NewPoint3<float>("boundingScale", (this.unknown11b.X - this.unknown11a.X), (this.unknown11b.Z - this.unknown11a.Z), (this.unknown11b.Y - this.unknown11a.Y));
        }
        public string GetMaxName()
        {
            return String.Format("atpt{0:D2}.{1}", Index, this.Name);
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
            if (true)
            {
                alphaOpacity = reader.ReadSingle();
            }

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
                    Maxscript.Command("pcCompTex = CompositeTextureMap()");

                    Maxscript.Command("pcTex = BitmapTexture()");
                    Maxscript.Command("pcTex.name = \"{0}\"", name);
                    Maxscript.Command("pcTex.filename = \"{0}\"", name + ".tga");

                    Maxscript.Command("pcTex2 = BitmapTexture()");
                    Maxscript.Command("pcTex2.name = \"{0}\"", name);
                    Maxscript.Command("pcTex2.filename = \"{0}\"", name + ".tga");
                    Maxscript.Command("pcTex2.monoOutput = 1");

                    Maxscript.Command("pcCheck = Checker()");
                    Maxscript.Command("pcCheck.Color1 = color 0 0 255");
                    Maxscript.Command("pcCheck.Color2 = color 0 0 255");

                    Maxscript.Command("pcCompTex.mapList[1] = pcTex");
                    Maxscript.Command("pcCompTex.mapList[2] = pcCheck");
                    Maxscript.Command("pcCompTex.mask[2] = pcTex2");

                    Maxscript.Command("mat.diffusemap = pcCompTex");
                }
            }
            if (flags.HasFlag(BrgMatFlag.DIFFUSETEXTURE) && !flags.HasFlag(BrgMatFlag.PLAYERCOLOR))
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
            else if (Maxscript.QueryBoolean("(classof mat.diffusemap) == CompositeTextureMap") && Maxscript.QueryBoolean("(classof mat.diffusemap.mapList[1]) == BitmapTexture"))
            {
                flags |= BrgMatFlag.MATNONE25 | BrgMatFlag.DIFFUSETEXTURE | BrgMatFlag.PLAYERCOLOR;
                name = (string)Maxscript.Query("getFilenameFile(mat.diffusemap.mapList[1].filename)", Maxscript.QueryType.String);
            }
        }

        public void Write(BrgBinaryWriter writer)
        {
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

        public void WriteExternal(FileStream fileStream)
        {
            using (BrgBinaryWriter writer = new BrgBinaryWriter(new LittleEndianBitConverter(), fileStream))
            {
                writer.Write(1280463949); // MTRL
                writer.Write(Encoding.UTF8.GetByteCount(name));

                writer.Write(new byte[20]);

                writer.WriteVector3(ref diffuse);
                writer.WriteVector3(ref ambient);
                writer.WriteVector3(ref specular);
                writer.WriteVector3(ref selfIllum);
                writer.Write(specularLevel);
                writer.Write(alphaOpacity);

                writer.Write(-1);
                writer.Write(16777216);
                writer.Write(65793);
                writer.Write(10);
                writer.Write(new byte[16]);

                if (flags.HasFlag(BrgMatFlag.PLAYERCOLOR))
                {
                    writer.Write(4);
                }
                else
                {
                    writer.Write(0);
                }

                writer.Write(0);

                if (flags.HasFlag(BrgMatFlag.REFLECTIONTEXTURE))
                {
                    writer.Write(1275068416);
                    writer.Write(12);
                    writer.Write(0);
                    writer.Write(1);
                }
                else
                {
                    writer.Write(new byte[16]);
                }

                writer.Write(new byte[32]);
                writer.Write(-1);
                writer.Write(-1);
                writer.Write(-1);
                writer.Write(-1);
                writer.Write(-1);
                writer.Write(-1);
                writer.Write(new byte[16]);

                writer.WriteString(name);
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
            header.unknown01 = this.ReadInt32();
            header.numMaterials = this.ReadInt32();
            header.unknown02 = this.ReadInt32();
            header.numMeshes = this.ReadInt32();
            header.space = this.ReadInt32();
            header.unknown03 = this.ReadInt32();
        }
        public void ReadAsetHeader(ref BrgAsetHeader header)
        {
            header.numFrames = this.ReadInt32();
            header.frameStep = this.ReadSingle();
            header.animTime = this.ReadSingle();
            header.frequency = this.ReadSingle();
            header.spf = this.ReadSingle();
            header.fps = this.ReadSingle();
            header.space = this.ReadInt32();
        }
        public void ReadMeshHeader(ref BrgMeshHeader header)
        {
            header.version = this.ReadInt16();
            header.format = (BrgMeshFormat)this.ReadInt16();
            header.numVertices = this.ReadInt16();
            header.numFaces = this.ReadInt16();
            header.interpolationType = this.ReadByte();
            header.properties = (BrgMeshAnimType)this.ReadByte();
            header.userDataEntryCount = this.ReadInt16();
            this.ReadVector3(out header.centerPos, true, false);
            header.centerRadius = this.ReadSingle();
            this.ReadVector3(out header.position, true, false);
            this.ReadVector3(out header.groundPos, true, false);
            header.extendedHeaderSize = this.ReadInt16();
            header.flags = (BrgMeshFlag)this.ReadInt16();
            this.ReadVector3(out header.boundingBoxMin, true);
            this.ReadVector3(out header.boundingBoxMax, true);
        }
        public void ReadMeshExtendedHeader(ref BrgMeshExtendedHeader header, int extendedHeaderSize)
        {
            header.numIndex = this.ReadInt16();
            header.numMatrix = this.ReadInt16();
            header.nameLength = this.ReadInt16();
            if (extendedHeaderSize > 6)
            {
                header.pointMaterial = this.ReadInt16();
                header.pointRadius = this.ReadSingle();
            }
            if (extendedHeaderSize > 12)
            {
                header.materialCount = this.ReadByte();
                header.shadowNameLength0 = this.ReadByte();
                header.shadowNameLength1 = this.ReadByte();
                header.shadowNameLength2 = this.ReadByte();
            }
            if (extendedHeaderSize > 16)
            {
                header.animTime = this.ReadSingle();
            }
            if (extendedHeaderSize > 20)
            {
                header.materialLibraryTimestamp = this.ReadInt32();
            }
            if (extendedHeaderSize > 24)
            {
                //this.ReadInt16(); //09a checkSpace
                header.unknown09e = this.ReadSingle();
            }
            if (extendedHeaderSize > 28)
            {
                header.exportedScaleFactor = this.ReadSingle();
            }
            if (extendedHeaderSize > 32)
            {
                header.nonUniformKeyCount = this.ReadInt32(); //09c
            }
            if (extendedHeaderSize > 36)
            {
                header.uniqueMaterialCount = this.ReadInt32();
            }

            //animTime = 0f;
            //materialLibraryTimestamp = 0;
            //unknown09e = 0f;
            //exportedScaleFactor = 1f;
            //lenSpace = 0;
            //uniqueMaterialCount = 0;
        }
        public BrgUserDataEntry ReadUserDataEntry(bool isParticle)
        {
            BrgUserDataEntry dataEntry;

            dataEntry.dataNameLength = this.ReadInt32();
            dataEntry.dataType = this.ReadInt32();
            switch (dataEntry.dataType)
            {
                case 1:
                    dataEntry.data = this.ReadInt32();
                    dataEntry.dataName = this.ReadString(dataEntry.dataNameLength + (int)dataEntry.data);
                    break;
                case 2:
                    dataEntry.data = this.ReadInt32();
                    dataEntry.dataName = this.ReadString(dataEntry.dataNameLength);
                    break;
                case 3:
                    dataEntry.data = this.ReadSingle();
                    dataEntry.dataName = this.ReadString(dataEntry.dataNameLength);
                    break;
                default:
                    dataEntry.data = this.ReadInt32();
                    dataEntry.dataName = this.ReadString(dataEntry.dataNameLength);
                    break;
            }

            return dataEntry;
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
        public void ReadVertexColor(out VertexColor color)
        {
            color.R = this.ReadByte();
            color.G = this.ReadByte();
            color.B = this.ReadByte();
            color.A = this.ReadByte();
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
            this.Write(header.unknown01);
            this.Write(header.numMaterials);
            this.Write(header.unknown02);
            this.Write(header.numMeshes);
            this.Write(header.space);
            this.Write(header.unknown03);
        }
        public void WriteAsetHeader(ref BrgAsetHeader header)
        {
            this.Write(header.numFrames);
            this.Write(header.frameStep);
            this.Write(header.animTime);
            this.Write(header.frequency);
            this.Write(header.spf);
            this.Write(header.fps);
            this.Write(header.space);
        }
        public void WriteMeshHeader(ref BrgMeshHeader header)
        {
            this.Write(header.version);
            this.Write((UInt16)header.format);
            this.Write(header.numVertices);
            this.Write(header.numFaces);
            this.Write((uint)header.properties);
            this.WriteVector3(ref header.centerPos, true);
            this.Write(header.centerRadius);//unknown03
            this.WriteVector3(ref header.position, true);
            this.WriteVector3(ref header.groundPos, true);
            this.Write(header.extendedHeaderSize);
            this.Write((UInt16)header.flags);
            this.WriteVector3(ref header.boundingBoxMin, true);
            this.WriteVector3(ref header.boundingBoxMax, true);
        }
        public void WriteMeshExtendedHeader(ref BrgMeshExtendedHeader extendedHeader)
        {
            this.Write(extendedHeader.numIndex);//numIndex0);
            this.Write(extendedHeader.numMatrix);//numMatrix0);
            this.Write(extendedHeader.nameLength);
            this.Write(extendedHeader.pointMaterial);
            this.Write(extendedHeader.pointRadius);
            this.Write(extendedHeader.materialCount);
            this.Write(extendedHeader.shadowNameLength0);
            this.Write(extendedHeader.shadowNameLength1);
            this.Write(extendedHeader.shadowNameLength2);
            this.Write(extendedHeader.animTime);
            this.Write(extendedHeader.materialLibraryTimestamp);
            //writer.Write((Int16)0);
            this.Write(extendedHeader.unknown09e);
            this.Write(extendedHeader.exportedScaleFactor);
            this.Write(extendedHeader.nonUniformKeyCount);
            this.Write(extendedHeader.uniqueMaterialCount);
        }
        public void WriteUserDataEntry(ref BrgUserDataEntry dataEntry, bool isParticle)
        {
            this.Write(dataEntry.dataNameLength);
            this.Write(dataEntry.dataType);
            switch (dataEntry.dataType)
            {
                case 1:
                    this.Write((Int32)dataEntry.data);
                    this.WriteString(dataEntry.dataName, 0);
                    break;
                case 2:
                    this.Write((Int32)dataEntry.data);
                    this.WriteString(dataEntry.dataName, 0);
                    break;
                case 3:
                    this.Write((Single)dataEntry.data);
                    this.WriteString(dataEntry.dataName, 0);
                    break;
                default:
                    this.Write((Int32)dataEntry.data);
                    this.WriteString(dataEntry.dataName, 0);
                    break;
            }
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
        public void WriteVertexColor(ref VertexColor vertexColor)
        {
            this.Write(vertexColor.R);
            this.Write(vertexColor.G);
            this.Write(vertexColor.B);
            this.Write(vertexColor.A);
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
