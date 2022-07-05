namespace AoMEngineLibrary.Graphics.Grn;

public class GltfGrnParameters
{
    public bool ConvertMeshes { get; set; }

    public bool ConvertAnimations { get; set; }

    public int AnimationIndex { get; set; }

    public int SceneIndex { get; set; }

    public GltfGrnParameters()
    {
        ConvertMeshes = true;
        ConvertAnimations = false;
        AnimationIndex = 0;
        SceneIndex = 0;
    }
}
