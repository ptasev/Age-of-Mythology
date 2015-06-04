namespace AoMEngineLibrary.Graphics.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ModelFile<TMesh, TMaterial>
        where TMesh : Mesh
        where TMaterial : Material
    {
        public string Name { get; set; }
        public List<TMesh> Meshes { get; set; }
        public List<TMaterial> Materials { get; set; }

        public Animation Animation { get; set; }

        public ModelFile()
        {
            this.Name = "(unnamed)";
            this.Meshes = new List<TMesh>();
            this.Materials = new List<TMaterial>();

            this.Animation = new Animation();
        }
    }
}
