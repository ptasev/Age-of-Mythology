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
    public class GrnMeshesViewModel : ReactiveObject, IModelObject
    {
        public string Name
        {
            get => "Meshes";
        }

        public ObservableCollection<IModelObject> Children { get; }

        public ObservableCollection<GrnMeshViewModel> Meshes { get; }

        public int VertexCount { get; }

        public int FaceCount { get; }

        public GrnMeshesViewModel(List<GrnMesh> meshes)
        {
            Children = new ObservableCollection<IModelObject>();

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
