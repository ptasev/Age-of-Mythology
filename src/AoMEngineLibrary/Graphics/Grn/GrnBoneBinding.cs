using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AoMEngineLibrary.Graphics.Grn
{
    public class GrnBoneBinding
    {
        public Int32 BoneIndex { get; set; }
        public float Unknown { get; set; }
        public Vector3 OBBMin { get; set; }
        public Vector3 OBBMax { get; set; }

        public GrnBoneBinding()
        {
            BoneIndex = -1;
        }
    }
}
