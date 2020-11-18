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
    public class GrnMeshesViewModel : TreeViewItemModelObject
    {
        public ObservableCollection<GrnMeshViewModel> Meshes { get; }

        public int VertexCount { get; }

        public int FaceCount { get; }

        public GrnMeshesViewModel(List<GrnMesh> meshes)
            : base()
        {
            Name = "Meshes";
            VertexCount = 0;
            FaceCount = 0;
            Meshes = new ObservableCollection<GrnMeshViewModel>();
            foreach (var mesh in meshes)
            {
                VertexCount += mesh.Vertices.Count;
                FaceCount += mesh.Faces.Count;

                Meshes.Add(new GrnMeshViewModel(mesh));
            }
        }
    }
}
