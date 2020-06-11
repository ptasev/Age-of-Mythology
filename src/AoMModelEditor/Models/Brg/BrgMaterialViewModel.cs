using AoMEngineLibrary.Graphics.Brg;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace AoMModelEditor.Models.Brg
{
    public class BrgMaterialViewModel : ReactiveObject, IModelObject
    {
        private BrgMaterial _mat;

        public string Name
        {
            get => string.IsNullOrEmpty(_mat.DiffuseMapName) ? "Material" : _mat.DiffuseMapName;
        }

        public BrgMatFlag Flags
        {
            get => _mat.Flags;
            set
            {
                _mat.Flags = value;
                this.RaisePropertyChanged(nameof(Flags));
            }
        }

        public string DiffuseMap
        {
            get => _mat.DiffuseMapName;
            set
            {
                _mat.DiffuseMapName = value;
                this.RaisePropertyChanged(nameof(DiffuseMap));
            }
        }

        public Vector3 DiffuseColor
        {
            get => _mat.DiffuseColor;
            set
            {
                _mat.DiffuseColor = Vector3.Clamp(value, Vector3.Zero, Vector3.One);
                this.RaisePropertyChanged(nameof(DiffuseColor));
            }
        }

        public Vector3 AmbientColor
        {
            get => _mat.AmbientColor;
            set
            {
                _mat.AmbientColor = Vector3.Clamp(value, Vector3.Zero, Vector3.One);
                this.RaisePropertyChanged(nameof(AmbientColor));
            }
        }

        public Vector3 SpecularColor
        {
            get => _mat.SpecularColor;
            set
            {
                _mat.SpecularColor = Vector3.Clamp(value, Vector3.Zero, Vector3.One);
                this.RaisePropertyChanged(nameof(SpecularColor));
            }
        }

        public Vector3 EmissiveColor
        {
            get => _mat.EmissiveColor;
            set
            {
                _mat.EmissiveColor = Vector3.Clamp(value, Vector3.Zero, Vector3.One);
                this.RaisePropertyChanged(nameof(EmissiveColor));
            }
        }

        public BrgMaterialViewModel(BrgMaterial mat)
        {
            _mat = mat;
        }
    }
}
