using AoMEngineLibrary.Graphics.Brg;
using AoMEngineLibrary.Graphics.Grn;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Text;

namespace AoMModelEditor.Models.Grn
{
    public class GrnMeshViewModel : TreeViewItemModelObject
    {
        private GrnMesh _mesh;

        public int VertexCount
        {
            get => _mesh.Vertices.Count;
        }

        public int FaceCount
        {
            get => _mesh.Faces.Count;
        }

        public GrnMeshViewModel(GrnMesh mesh)
            : base()
        {
            _mesh = mesh;
            Name = string.IsNullOrEmpty(_mesh.Name) ? "Mesh" : $"Mesh {_mesh.Name}";
        }
    }
}
