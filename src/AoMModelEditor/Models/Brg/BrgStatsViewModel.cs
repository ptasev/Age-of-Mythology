using AoMEngineLibrary.Graphics.Brg;
using ReactiveUI;
using System;
using System.Linq;

namespace AoMModelEditor.Models.Brg
{
    public class BrgStatsViewModel : TreeViewItemModelObject
    {
        private readonly BrgFile _brg;

        public int VertexCount { get; private set; }

        public int FaceCount { get; private set; }

        public int DummyCount { get; private set; }

        public int FrameCount { get; private set; }

        public int MaterialCount { get; private set; }

        public float AnimationDuration { get; private set; }

        public BrgStatsViewModel(BrgFile brg)
            : base()
        {
            _brg = brg;
            Name = "Brg Statistics";

            Update();
        }

        public void Update()
        {
            var mesh = _brg.Meshes.FirstOrDefault();
            if (mesh is null) return;

            VertexCount = mesh.Vertices.Count;
            FaceCount = mesh.Faces.Count;
            DummyCount = mesh.Attachpoints.Count;

            FrameCount = _brg.Meshes.Count;
            MaterialCount = _brg.Materials.Count;
            AnimationDuration = _brg.Animation.Duration;

            this.RaisePropertyChanged(nameof(VertexCount));
            this.RaisePropertyChanged(nameof(FaceCount));
            this.RaisePropertyChanged(nameof(DummyCount));
            this.RaisePropertyChanged(nameof(FrameCount));
            this.RaisePropertyChanged(nameof(MaterialCount));
            this.RaisePropertyChanged(nameof(AnimationDuration));
        }
    }
}
