using AoMEngineLibrary.Graphics;
using AoMEngineLibrary.Graphics.Brg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoMMaxPlugin
{
    public static class BrgAttachpointExtensions
    {
        public static string GetMaxTransform(this BrgAttachpoint att)
        {
            //string xVector = Maxscript.NewPoint3<float>("xVector", this.z.X, this.y.X, this.x.X); // original
            //string yVector = Maxscript.NewPoint3<float>("yVector", this.z.Z, this.y.Z, this.x.Z);
            //string zVector = Maxscript.NewPoint3<float>("zVector", this.z.Y, this.y.Y, this.x.Y);

            string xVector = Maxscript.NewPoint3<float>("xVector", -att.ZVector.X, -att.YVector.X, -att.XVector.X);
            string yVector = Maxscript.NewPoint3<float>("yVector", -att.ZVector.Z, -att.YVector.Z, -att.XVector.Z);
            string zVector = Maxscript.NewPoint3<float>("zVector", att.ZVector.Y, att.YVector.Y, att.XVector.Y);
            string posVector = Maxscript.NewPoint3<float>("rotPosVect", 0f, 0f, 0f);
            return Maxscript.NewMatrix3("transformMatrix", xVector, yVector, zVector, posVector);
        }
        public static string GetMaxPosition(this BrgAttachpoint att)
        {
            return Maxscript.NewPoint3<float>("posVector", -att.Position.X, -att.Position.Z, att.Position.Y);
        }
        public static string GetMaxBoxSize(this BrgAttachpoint att)
        {
            return Maxscript.NewPoint3<float>("boxSize", 1, 1, 1);
        }
        public static string GetMaxScale(this BrgAttachpoint att)
        {
            return Maxscript.NewPoint3<float>("boundingScale", (att.BoundingBoxMax.X - att.BoundingBoxMin.X), (att.BoundingBoxMax.Z - att.BoundingBoxMin.Z), (att.BoundingBoxMax.Y - att.BoundingBoxMin.Y));
        }
        public static string GetMaxName(this BrgAttachpoint att)
        {
            return String.Format("Dummy_{0}", att.Name);
            //return String.Format("atpt{0:D2}.{1}", Index, this.Name);
        }
    }
}
