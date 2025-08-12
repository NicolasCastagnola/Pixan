using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class GameLevelsManager : BaseMonoSingleton<GameLevelsManager>
{
    [field:SerializeField] public List<AssetReference> Levels { get; private set; }
    [field:SerializeField] public AssetReference CanvasPlaying { get; private set; }
    [field:SerializeField] public AssetReference Menu { get; private set; }
    [field:SerializeField] public AssetReference WinLevel { get; private set; }
    
    [field: SerializeField] public List<AssetReference> ExtraLevels { get; set; } = new List<AssetReference>();

    public IEnumerable<AssetReference> GetIndependentScenes() => new[]
    {
        Menu,
        WinLevel,
        CanvasPlaying,
    };

    
#if UNITY_EDITOR
    public List<UnityEditor.SceneAsset> editorOnlyScenes;
#endif
}
