using AoMEngineLibrary.Graphics.Grn;
using ReactiveUI;
using System.Collections.ObjectModel;

namespace AoMModelEditor.Models.Grn
{
    public class GrnSettingsViewModel : ReactiveObject, IModelObject
    {
        public string Name { get; }

        public ObservableCollection<IModelObject> Children => new ObservableCollection<IModelObject>();

        private bool _convertMeshes;
        public bool ConvertMeshes
        {
            get => _convertMeshes;
            set
            {
                _convertMeshes = value;
                this.RaisePropertyChanged(nameof(ConvertMeshes));
            }
        }

        private bool _convertAnimation;
        public bool ConvertAnimation
        {
            get => _convertAnimation;
            set
            {
                _convertAnimation = value;
                this.RaisePropertyChanged(nameof(ConvertAnimation));
            }
        }

        public GrnSettingsViewModel()
        {
            Name = "Grn Settings";
            ConvertMeshes = true;
        }

        public GltfGrnParameters CreateGltfGrnParameters()
        {
            return new GltfGrnParameters()
            {
                ConvertMeshes = ConvertMeshes,
                ConvertAnimations = ConvertAnimation
            };
        }
    }
}
