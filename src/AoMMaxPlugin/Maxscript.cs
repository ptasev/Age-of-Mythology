namespace AoMMaxPlugin
{
    using ManagedServices;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Numerics;

    public static class Maxscript
    {
        private static CultureInfo cult = CultureInfo.InvariantCulture;
        public static bool Execute = true;
        public static bool OutputCommands = true;
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
        public static void Format(string message, string arguments, params object[] args)
        {
            Command("format \"" + message + "\" " + arguments, args);
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
        public static string NewBone(string varName, string name, string position, string rotation)
        {
            Command("{0} = bone name:\"{1}\" rotation:{3} position:{2}", varName, name, position, rotation);
            return varName;
        }
        public static string SnapshotAsMesh(string varName, string varNode)
        {
            Command("{0} = snapshotAsMesh {1}", varName, varNode);
            return varName;
        }
        public static string SnapshotAsMesh(string varName, string varNode, float time)
        {
            Command("{1} = at time {0}s (snapshotAsMesh {2})", time, varName, varNode);
            return varName;
        }
        public static string NewMeshLiteral(string name, string vertArray, string faceArray, string faceMatIdArray, string texVertArray)
        {
            return MaxscriptSDK.AssembleScript("mesh name:\"{0}\" vertices:{1} faces:{2} materialIDs:{3} tverts:{4}", name, vertArray, faceArray, faceMatIdArray, texVertArray);
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
        public static string QuatLiteral(Quaternion q)
        {
            return MaxscriptSDK.AssembleScript("quat {0} {1} {2} {3}", q.X, q.Y, q.Z, q.W);
        }
        #region Point3
        public static string Point3Literal<T>(T X, T Y, T Z)
        {
            return MaxscriptSDK.AssembleScript("[{0}, {1}, {2}]", X, Y, Z);
        }
        public static string Point3Literal(Vector3 v)
        {
            return MaxscriptSDK.AssembleScript("[{0}, {1}, {2}]", v.X, v.Y, v.Z);
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
}
