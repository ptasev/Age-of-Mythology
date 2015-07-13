using AoMEngineLibrary.Graphics.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoMEngineLibrary.Graphics.Grn
{
    public class GrnBoneBinding
    {
        public Int32 BoneIndex { get; set; }
        public float Unknown { get; set; }
        public Vector3D OBBMin { get; set; }
        public Vector3D OBBMax { get; set; }

        public GrnBoneBinding()
        {
            BoneIndex = -1;
        }
    }
}
