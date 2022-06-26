using AoMEngineLibrary.Graphics.Brg;
using ReactiveUI;
using System;

namespace AoMModelEditor.Models.Brg;

public class BrgGltfImportSettingsViewModel : GltfImportSettingsViewModel
{
    public override string Name => "Brg Settings";

    private float _sampleRateFps;
    public float SampleRateFps
    {
        get => _sampleRateFps;
        set
        {
            value = Math.Clamp(value, 1, 100);

            _sampleRateFps = value;
            this.RaisePropertyChanged(nameof(SampleRateFps));
        }
    }

    public BrgGltfImportSettingsViewModel()
    {
        _sampleRateFps = 10.0f;
    }

    public GltfBrgParameters CreateGltfBrgParameters()
    {
        return new GltfBrgParameters()
        {
            SampleRateFps = SampleRateFps,
            AnimationIndex = SelectedAnimation?.LogicalIndex ?? 0
        };
    }
}
