using AoMEngineLibrary.Graphics.Grn;
using ReactiveUI;

namespace AoMModelEditor.Models.Grn
{
    public class GrnMaterialViewModel : TreeViewItemModelObject
    {
        private GrnMaterial _mat;

        public override string Name
        {
            get => string.IsNullOrEmpty(_mat.Name) ? "Material" : $"Mat {_mat.Name}";
        }

        public string MaterialName
        {
            get => _mat.Name;
            set
            {
                _mat.ParentFile.SetDataExtensionObjectName(_mat.DataExtensionIndex, value);
                this.RaisePropertyChanged(nameof(MaterialName));
                this.RaisePropertyChanged(nameof(Name));
            }
        }

        public string DiffuseTextureName
        {
            get => _mat.DiffuseTexture?.Name ?? string.Empty;
        }

        public GrnMaterialViewModel(GrnMaterial material)
            : base()
        {
            _mat = material;

            var tex = _mat.DiffuseTexture;
            if (tex != null)
            {
                Children.Add(new GrnTextureViewModel(tex));
            }
        }
    }
}
