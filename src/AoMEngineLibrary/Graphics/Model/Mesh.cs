namespace AoMEngineLibrary.Graphics.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Numerics;
    using System.Text;
    using System.Threading.Tasks;

    public class Mesh
    {
        public virtual string Name { get; set; }
        public List<Vector3> Vertices { get; set; }
        public List<Vector3> Normals { get; set; }
        public List<Face> Faces { get; set; }

        public List<Vector3> TextureCoordinates { get; set; }
        public List<Color4D> Colors { get; set; }

        public List<Mesh> MeshAnimations { get; set; }

        public Mesh()
        {
            this.Name = "Mesh";
            this.Vertices = new List<Vector3>();
            this.Normals = new List<Vector3>();
            this.Faces = new List<Face>();

            this.TextureCoordinates = new List<Vector3>();
            this.Colors = new List<Color4D>();

            this.MeshAnimations = new List<Mesh>();
        }
    }
}
