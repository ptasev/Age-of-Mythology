namespace AoMEngineLibrary.Graphics.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Mesh
    {
        public string Name { get; set; }
        public List<Vector3D> Vertices { get; set; }
        public List<Vector3D> Normals { get; set; }
        public List<Face> Faces { get; set; }

        public List<Vector3D> TextureCoordinates { get; set; }
        public List<Color4D> Colors { get; set; }

        public int SkeletonIndex { get; set; }
        public List<VertexWeight> VertexWeights { get; set; }

        public List<Mesh> MeshAnimations { get; set; }

        public Mesh()
        {
            this.Name = "(unnamed)";
            this.Vertices = new List<Vector3D>();
            this.Normals = new List<Vector3D>();
            this.Faces = new List<Face>();

            this.TextureCoordinates = new List<Vector3D>();
            this.Colors = new List<Color4D>();

            this.SkeletonIndex = -1;
            this.VertexWeights = new List<VertexWeight>();

            this.MeshAnimations = new List<Mesh>();
        }
    }
}
