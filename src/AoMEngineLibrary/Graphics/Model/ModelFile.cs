namespace AoMEngineLibrary.Graphics.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ModelFile<TMesh, TMaterial, TAnimation>
        where TMesh : Mesh
        where TMaterial : Material
        where TAnimation : Animation
    {
        public List<TMesh> Meshes { get; set; }
        public List<TMaterial> Materials { get; set; }

        public TAnimation Animation { get; set; }

        public ModelFile()
        {
            this.Meshes = new List<TMesh>();
            this.Materials = new List<TMaterial>();

            this.Animation = (TAnimation)Activator.CreateInstance(typeof(TAnimation));
        }
    }
}
