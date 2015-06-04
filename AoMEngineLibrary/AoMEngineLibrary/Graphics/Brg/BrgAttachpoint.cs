namespace AoMEngineLibrary.Graphics.Brg
{
    using AoMEngineLibrary.Graphics.Model;
    using System;

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
        public Vector3D XVector;
        public Vector3D YVector;
        public Vector3D ZVector;
        public Vector3D Position;
        public Vector3D BoundingBoxMin;
        public Vector3D BoundingBoxMax;

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
            XVector = new Vector3D(0, 1, 0);
            YVector = new Vector3D(0, 0, -1);
            ZVector = new Vector3D(-1, 0, 0);
            Position = new Vector3D(0f);
            BoundingBoxMin = new Vector3D(-0.25f);
            BoundingBoxMax = new Vector3D(0.25f);
        }
        public BrgAttachpoint(BrgAttachpoint prev)
        {
            Index = prev.Index;
            NameId = prev.NameId;
            XVector = prev.XVector;
            YVector = prev.YVector;
            ZVector = prev.ZVector;
            Position = prev.Position;
            BoundingBoxMin = prev.BoundingBoxMin;
            BoundingBoxMax = prev.BoundingBoxMax;
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

            string xVector = Maxscript.NewPoint3<float>("xVector", -this.ZVector.X, -this.YVector.X, -this.XVector.X);
            string yVector = Maxscript.NewPoint3<float>("yVector", -this.ZVector.Z, -this.YVector.Z, -this.XVector.Z);
            string zVector = Maxscript.NewPoint3<float>("zVector", this.ZVector.Y, this.YVector.Y, this.XVector.Y);
            string posVector = Maxscript.NewPoint3<float>("rotPosVect", 0, 0, 0);
            return Maxscript.NewMatrix3("transformMatrix", xVector, yVector, zVector, posVector);
        }
        public string GetMaxPosition()
        {
            return Maxscript.NewPoint3<float>("posVector", -this.Position.X, -this.Position.Z, this.Position.Y);
        }
        public string GetMaxBoxSize()
        {
            return Maxscript.NewPoint3<float>("boxSize", 1, 1, 1);
        }
        public string GetMaxScale()
        {
            return Maxscript.NewPoint3<float>("boundingScale", (this.BoundingBoxMax.X - this.BoundingBoxMin.X), (this.BoundingBoxMax.Z - this.BoundingBoxMin.Z), (this.BoundingBoxMax.Y - this.BoundingBoxMin.Y));
        }
        public string GetMaxName()
        {
            return String.Format("atpt{0:D2}.{1}", Index, this.Name);
        }
    }
}
