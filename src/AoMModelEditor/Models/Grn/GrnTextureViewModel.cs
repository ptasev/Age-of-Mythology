using AoMEngineLibrary.Graphics.Grn;
using ReactiveUI;
using System.IO;

namespace AoMModelEditor.Models.Grn
{
    public class GrnTextureViewModel : TreeViewItemModelObject
    {
        private GrnTexture _tex;

        public override string Name
        {
            get => string.IsNullOrEmpty(_tex.Name) ? "Texture" : $"Tex {_tex.Name}";
        }

        public string TextureName
        {
            get => _tex.Name;
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
                var fileName = Path.GetFileName(value);
                _tex.ParentFile.SetDataExtensionFileName(_tex.DataExtensionIndex, fileName);
                _tex.ParentFile.SetDataExtensionObjectName(_tex.DataExtensionIndex, Path.GetFileNameWithoutExtension(fileName));
                this.RaisePropertyChanged(nameof(FileName));
                this.RaisePropertyChanged(nameof(TextureName));
                this.RaisePropertyChanged(nameof(Name));
            }
        }

        public GrnTextureViewModel(GrnTexture texture)
            : base()
        {
            _tex = texture;
        }
    }
}
