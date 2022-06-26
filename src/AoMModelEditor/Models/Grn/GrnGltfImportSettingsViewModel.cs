using AoMEngineLibrary.Graphics.Grn;
using ReactiveUI;

namespace AoMModelEditor.Models.Grn;

public class GrnGltfImportSettingsViewModel : GltfImportSettingsViewModel
{
    public override string Name => "Grn Settings";

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

    public GrnGltfImportSettingsViewModel()
    {
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
