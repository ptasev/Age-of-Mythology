using AoMEngineLibrary.Graphics.Brg;
using AoMEngineLibrary.Graphics.Grn;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;

namespace AoMModelEditor.Models.Grn
{
    public class GrnTextureViewModel : ReactiveObject, IModelObject
    {
        private GrnTexture _tex;

        public string Name
        {
            get => string.IsNullOrEmpty(_tex.Name) ? "Texture" : $"Tex {_tex.Name}";
        }

        public ObservableCollection<IModelObject> Children { get; }

        public string TextureName
        {
            get => _tex.Name;
            set
            {
                //_tex.Name = value;
                this.RaisePropertyChanged(nameof(TextureName));
                this.RaisePropertyChanged(nameof(Name));
            }
        }

        public int Width
        {
            get => _tex.Width;
        }

        public int Height
        {
            get => _tex.Height;
        }

        public string FileName
        {
            get => Path.GetFileName(_tex.FileName);
            set
            {
                //_tex.FileName = Path.GetFileName(value);
                this.RaisePropertyChanged(nameof(FileName));
            }
        }

        public GrnTextureViewModel(GrnTexture texture)
        {
            _tex = texture;

            Children = new ObservableCollection<IModelObject>();
        }
    }
}
