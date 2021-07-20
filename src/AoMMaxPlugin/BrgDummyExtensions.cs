using AoMEngineLibrary.Graphics.Brg;
using System;

namespace AoMMaxPlugin
{
    public static class BrgDummyExtensions
    {
        public static string GetMaxTransform(this BrgDummy att)
        {
            //string xVector = Maxscript.NewPoint3<float>("xVector", this.z.X, this.y.X, this.x.X); // original
            //string yVector = Maxscript.NewPoint3<float>("yVector", this.z.Z, this.y.Z, this.x.Z);
            //string zVector = Maxscript.NewPoint3<float>("zVector", this.z.Y, this.y.Y, this.x.Y);

            string xVector = Maxscript.NewPoint3<float>("xVector", -att.Right.X, -att.Forward.X, -att.Up.X);
            string yVector = Maxscript.NewPoint3<float>("yVector", -att.Right.Z, -att.Forward.Z, -att.Up.Z);
            string zVector = Maxscript.NewPoint3<float>("zVector", att.Right.Y, att.Forward.Y, att.Up.Y);
            string posVector = Maxscript.NewPoint3<float>("rotPosVect", 0f, 0f, 0f);
            return Maxscript.NewMatrix3("transformMatrix", xVector, yVector, zVector, posVector);
        }
        public static string GetMaxPosition(this BrgDummy att)
        {
            return Maxscript.NewPoint3<float>("posVector", -att.Position.X, -att.Position.Z, att.Position.Y);
        }
        public static string GetMaxBoxSize(this BrgDummy att)
        {
            return Maxscript.NewPoint3<float>("boxSize", 1, 1, 1);
        }
        public static string GetMaxScale(this BrgDummy att)
        {
            return Maxscript.NewPoint3<float>("boundingScale", (att.BoundingBoxMax.X - att.BoundingBoxMin.X), (att.BoundingBoxMax.Z - att.BoundingBoxMin.Z), (att.BoundingBoxMax.Y - att.BoundingBoxMin.Y));
        }
        public static string GetMaxName(this BrgDummy att)
        {
            return String.Format("Dummy_{0}", att.Name);
            //return String.Format("atpt{0:D2}.{1}", Index, this.Name);
        }
    }
}
