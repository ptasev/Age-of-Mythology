﻿using AoMEngineLibrary.Graphics.Grn;
using ReactiveUI;
using System;

namespace AoMModelEditor.Models.Grn
{
    public class GrnStatsViewModel : TreeViewItemModelObject
    {
        private readonly GrnFile _grn;

        public int VertexCount { get; private set; }

        public int FaceCount { get; private set; }

        public int MeshCount { get; private set; }

        public int BoneCount { get; private set; }

        public int MaterialCount { get; private set; }

        public float AnimationDuration { get; private set; }

        public GrnStatsViewModel(GrnFile grn)
            : base()
        {
            _grn = grn;
            Name = "Grn Statistics";

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
