using AoMEngineLibrary.Graphics.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoMEngineLibrary.Graphics.Grn
{
    public class GrnAnimation : Animation
    {
        public List<GrnBoneTrack> BoneTracks { get; set; }

        public GrnAnimation()
            : base()
        {
            this.BoneTracks = new List<GrnBoneTrack>();
        }
    }
}
