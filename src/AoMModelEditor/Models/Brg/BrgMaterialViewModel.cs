using AoMEngineLibrary.Graphics.Brg;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Numerics;
using System.Text;

namespace AoMModelEditor.Models.Brg
{
    public class BrgMaterialViewModel : TreeViewItemModelObject
    {
        private BrgMaterial _mat;

        public override string Name
        {
            get => string.IsNullOrEmpty(_mat.DiffuseMapName) ? "Material" : $"Mat {_mat.DiffuseMapName}";
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
                this.RaisePropertyChanged(nameof(Name));
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

        public float Opacity
        {
            get => _mat.Opacity;
            set
            {
                _mat.Opacity = Math.Clamp(value, 0.0f, 1.0f);
                if (value < (1.0f - 0.001f))
                {
                    Flags |= BrgMatFlag.Alpha;
                }
                this.RaisePropertyChanged(nameof(Opacity));
            }
        }

        public float SpecularExponent
        {
            get => _mat.SpecularExponent;
            set
            {
                _mat.SpecularExponent = value;
                if (_mat.SpecularExponent > 0)
                {
                    Flags |= BrgMatFlag.SpecularExponent;
                }
                this.RaisePropertyChanged(nameof(SpecularExponent));
            }
        }

        public string CubeMap
        {
            get => _mat.CubeMapInfo.CubeMapName;
            set
            {
                _mat.CubeMapInfo.CubeMapName = value;
                if (string.IsNullOrEmpty(value))
                {
                    Flags &= ~(BrgMatFlag.CubeMapInfo | BrgMatFlag.AdditiveCubeBlend);
                }
                else
                {
                    Flags |= (BrgMatFlag.CubeMapInfo | BrgMatFlag.AdditiveCubeBlend);
                }
                this.RaisePropertyChanged(nameof(CubeMap));
            }
        }

        public byte CubeMapBlendPercentage
        {
            get => _mat.CubeMapInfo.TextureFactor;
            set
            {
                _mat.CubeMapInfo.TextureFactor = Math.Clamp(value, (byte)0, (byte)100);
                this.RaisePropertyChanged(nameof(CubeMapBlendPercentage));
            }
        }

        public BrgMaterialViewModel(BrgMaterial mat)
            : base()
        {
            _mat = mat;
        }
    }
}
