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
    public class GrnMaterialViewModel : ReactiveObject, IModelObject
    {
        private GrnMaterial _mat;

        public string Name
        {
            get => string.IsNullOrEmpty(_mat.Name) ? "Material" : $"Mat {_mat.Name}";
        }

        public ObservableCollection<IModelObject> Children { get; }

        public string MaterialName
        {
            get => _mat.Name;
            set
            {
                //_mat.Name = value;
                this.RaisePropertyChanged(nameof(MaterialName));
                this.RaisePropertyChanged(nameof(Name));
            }
        }

        public string DiffuseTextureName
        {
            get => _mat.DiffuseTexture?.Name ?? string.Empty;
        }

        public GrnMaterialViewModel(GrnMaterial material)
        {
            _mat = material;

            Children = new ObservableCollection<IModelObject>();
            var tex = _mat.DiffuseTexture;
            if (tex != null)
            {
                Children.Add(new GrnTextureViewModel(tex));
            }
        }
    }
}
