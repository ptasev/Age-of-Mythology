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
    public class GrnStatsViewModel : ReactiveObject, IModelObject
    {
        private readonly GrnFile _grn;

        public string Name
        {
            get => "Grn Statistics";
        }

        public ObservableCollection<IModelObject> Children { get; }

        public int VertexCount { get; private set; }

        public int FaceCount { get; private set; }

        public int MeshCount { get; private set; }

        public int BoneCount { get; private set; }

        public int MaterialCount { get; private set; }

        public float AnimationDuration { get; private set; }

        public GrnStatsViewModel(GrnFile grn)
        {
            _grn = grn;
            Children = new ObservableCollection<IModelObject>();

            Update();
        }

        public void Update()
        {
            VertexCount = 0;
            FaceCount = 0;
            foreach (var mesh in _grn.Meshes)
            {
                VertexCount += mesh.Vertices.Count;
                FaceCount += mesh.Faces.Count;
            }

            MeshCount = _grn.Meshes.Count;
            BoneCount = _grn.Bones.Count;
            MaterialCount = _grn.Materials.Count;

            AnimationDuration = _grn.Animation.Duration;

            this.RaisePropertyChanged(nameof(VertexCount));
            this.RaisePropertyChanged(nameof(FaceCount));
            this.RaisePropertyChanged(nameof(MeshCount));
            this.RaisePropertyChanged(nameof(BoneCount));
            this.RaisePropertyChanged(nameof(MaterialCount));
            this.RaisePropertyChanged(nameof(AnimationDuration));
        }
    }
}
