using AoMEngineLibrary.Graphics.Brg;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;

namespace AoMModelEditor.Models.Brg
{
    public class BrgMeshViewModel : ReactiveObject, IModelObject
    {
        private readonly BrgMesh _mesh;

        public string Name => "Mesh";

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set => this.RaiseAndSetIfChanged(ref _isSelected, value);
        }

        public BrgMeshViewModel(BrgMesh mesh)
        {
            _mesh = mesh;
        }
    }
}
