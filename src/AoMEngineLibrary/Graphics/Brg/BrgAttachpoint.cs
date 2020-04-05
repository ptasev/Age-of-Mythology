namespace AoMEngineLibrary.Graphics.Brg
{
    using AoMEngineLibrary.Graphics.Model;
    using Extensions;
    using Newtonsoft.Json;
    using System;
    using System.Numerics;

    public class BrgAttachpoint
    {
        public static string[] AttachpointNames = new string[55] { 
            "targetpoint", "launchpoint", "corpse", "decal", "fire", "gatherpoint", "reserved9", "reserved8", "reserved7", "reserved6", "reserved5", "reserved4", "reserved3", "reserved2", "reserved1", "reserved0", 
            "smoke9", "smoke8", "smoke7", "smoke6", "smoke5", "smoke4", "smoke3", "smoke2", "smoke1", "smoke0", "garrisonflag", "hitpointbar", "rightforearm", "leftforearm", "rightfoot", "leftfoot", 
            "rightleg", "leftleg", "rightthigh", "leftthigh", "pelvis", "backabdomen", "frontabdomen", "backchest", "frontchest", "rightshoulder", "leftshoulder", "neck", "rightear", "leftear", "chin", "face", 
            "forehead", "topofhead", "righthand", "lefthand", "reserved", "smokepoint", "attachpoint"
            //"TARGETPOINT", "LAUNCHPOINT", "CORPSE", "DECAL", "FIRE", "GATHERPOINT", "RESERVED9", "RESERVED8", "RESERVED7", "RESERVED6", "RESERVED5", "RESERVED4", "RESERVED3", "RESERVED2", "RESERVED1", "RESERVED0", 
            //"SMOKE9", "SMOKE8", "SMOKE7", "SMOKE6", "SMOKE5", "SMOKE4", "SMOKE3", "SMOKE2", "SMOKE1", "SMOKE0", "GARRISONFLAG", "HITPOINTBAR", "RIGHTFOREARM", "LEFTFOREARM", "RIGHTFOOT", "LEFTFOOT", 
            //"RIGHTLEG", "LEFTLEG", "RIGHTTHIGH", "LEFTTHIGH", "PELVIS", "BACKABDOMEN", "FRONTABDOMEN", "BACKCHEST", "FRONTCHEST", "RIGHTSHOULDER", "LEFTSHOULDER", "NECK", "RIGHTEAR", "LEFTEAR", "CHIN", "FACE", 
            //"FOREHEAD", "TOPOFHEAD", "RIGHTHAND", "LEFTHAND", "RESERVED", "SMOKEPOINT", "ATTACHPOINT"
         };

        public int Index;
        public int NameId;
        /// <summary>
        /// The Y Axis pointing up in a LH coordinate system. (Green axis in AoM editor)
        /// </summary>
        public Vector3 Up;
        /// <summary>
        /// The Z axis pointing forward in a LH coordinate system. (Red axis in AoM editor)
        /// </summary>
        public Vector3 Forward;
        /// <summary>
        /// The X axis pointing to the right in a LH coordinate system. (Blue axis in AoM editor)
        /// </summary>
        public Vector3 Right;
        public Vector3 Position;
        public Vector3 BoundingBoxMin;
        public Vector3 BoundingBoxMax;

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
                    throw new Exception("Invalid Attachpoint Name Id " + NameId + "!");
                }
            }
        }

        public BrgAttachpoint()
        {
            Index = -1;
            NameId = -1;
            Up = new Vector3(0, 1, 0);
            Forward = new Vector3(0, 0, -1);
            Right = new Vector3(-1, 0, 0);
            Position = new Vector3(0f);
            BoundingBoxMin = new Vector3(-0.25f);
            BoundingBoxMax = new Vector3(0.25f);
        }
        public BrgAttachpoint(BrgAttachpoint prev)
        {
            Index = prev.Index;
            NameId = prev.NameId;
            Up = prev.Up;
            Forward = prev.Forward;
            Right = prev.Right;
            Position = prev.Position;
            BoundingBoxMin = prev.BoundingBoxMin;
            BoundingBoxMax = prev.BoundingBoxMax;
        }

        public static int GetIdByName(string name)
        {
            int ret;
            if (!BrgAttachpoint.TryGetIdByName(name, out ret))
            {
                throw new Exception("Invalid Attachpoint Name " + name + "!");
            }
            return ret;
        }
        public static bool TryGetIdByName(string name, out int nId)
        {
            if (name.Equals("righthandtag", StringComparison.InvariantCultureIgnoreCase))
            {
                nId = 4;
                return true;
            }

            if (name.Equals("lefthandtag", StringComparison.InvariantCultureIgnoreCase))
            {
                nId = 3;
                return true;
            }

            for (int i = 0; i < AttachpointNames.Length; i++)
            {
                if (AttachpointNames[i].Equals(name, StringComparison.InvariantCultureIgnoreCase))
                {
                    nId = 54 - i;
                    return true;
                }
            }

            nId = -1;
            return false;
        }
    }
}
