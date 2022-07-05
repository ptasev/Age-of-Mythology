using DynamicData;
using ReactiveUI;
using SharpGLTF.Schema2;
using System.Collections.ObjectModel;

namespace AoMModelEditor.Models;

public abstract class GltfImportSettingsViewModel : ReactiveObject
{
    private ModelRoot? _gltf;

    public abstract string Name { get; }

    private Scene? _selectedScene;
    public Scene? SelectedScene
    {
        get => _selectedScene;
        set => this.RaiseAndSetIfChanged(ref _selectedScene, value);
    }

    public ObservableCollection<Scene> Scenes { get; }

    private Animation? _animation;
    public Animation? SelectedAnimation
    {
        get => _animation;
        set => this.RaiseAndSetIfChanged(ref _animation, value);
    }

    public ObservableCollection<Animation> Animations { get; }

    public GltfImportSettingsViewModel()
    {
        Animations = new ObservableCollection<Animation>();
        Scenes = new ObservableCollection<Scene>();
    }

    public void SetGltfModel(ModelRoot gltf)
    {
        _gltf = gltf;

        Scenes.Clear();
        Scenes.AddRange(_gltf.LogicalScenes);
        SelectedScene = _gltf.DefaultScene;

        Animations.Clear();
        Animations.AddRange(_gltf.LogicalAnimations);
        SelectedAnimation = _gltf.LogicalAnimations.Count > 0 ? _gltf.LogicalAnimations[0] : null;
    }
}
