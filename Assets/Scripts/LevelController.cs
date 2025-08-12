using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public struct KeyAreasNamesAndPointKeyValuePair
{
    public string AreaName;
    public Transform areaKeyPoint;
}
public class LevelController : BaseMonoSingleton<LevelController>
{
    public event Action OnWin, OnRestart, OnLose, OnNextLevel;
    public Player PlayerReference { get; private set; }
    [field:SerializeField] public List<KeyAreasNamesAndPointKeyValuePair> AreaKeyAreas { get; private set; }
    
    [field:SerializeField, TabGroup("Level References")] private Transform levelDefaultStartingPoint;
    [field:SerializeField, TabGroup("Level References")] public EnemyManager EnemyManager { get; private set; }
    [field:SerializeField, TabGroup("Level References")] public BossFightArena[] LevelBossFights { get; private set; }
    [field:SerializeField, TabGroup("Level References")] public KeyBehaviour[] LevelKeys { get; private set; }
    [field:SerializeField, TabGroup("Level References")] public NPC[] LevelNPCs { get; private set; }

    public void Initialize(Action onWin, Action onRestart, Action onLose, Action onNextLevel)
    {
        PlayerReference = Player.Instance;

        var spawnPos = levelDefaultStartingPoint;
        
        if (BonfireManager.Instance != null)
        {
            int index = PlayerPrefs.GetInt(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name+".LastRestedBonfire", -1);
            spawnPos = index == -1 ? levelDefaultStartingPoint : BonfireManager.Instance.LevelBonfires[index].BonfireTravelPoint;
        }
        
        PlayerReference.Initialize(Lose, spawnPos);
        
        EnemyManager.Instance.Initialize(PlayerReference);

        foreach (var bossFight in LevelBossFights)
        {
            bossFight.Initialize(PlayerReference);
        }

        foreach (var npc in LevelNPCs)
        {
            if(npc != null) npc.Initialize();
        }
        
        OnWin = onWin;
        OnNextLevel = onNextLevel;
        OnRestart = onRestart;
        OnLose = onLose;
    }
    public void TpPlayerToTargetArea(string key)
    {
        var target = AreaKeyAreas.FirstOrDefault(x => x.AreaName == key).areaKeyPoint;

        GameManager.ShowLog($"Teletransporting player to: {target} position");
        
        Player.Instance.Motor.SetPositionAndRotation(target.position, target.rotation); 
    }
    public void Lose() => OnLose?.Invoke();
    public void NextLevel() => OnNextLevel?.Invoke();
    public void Lose(HealthComponent.HealthModificationReport healthModificationReport) => OnLose?.Invoke();
    public void Win() => OnWin?.Invoke();
    public void ResetScene() => OnRestart?.Invoke();
    public void GetAllKeys()
    {
        if(LevelKeys.Length == 0) return;
        
        foreach (var key in LevelKeys)
        {
            key.OnInteract();
        }
    }

    [Button]
    public void FindBossFights() => LevelBossFights = FindObjectsOfType<BossFightArena>();
    [Button]
    public void FindKeys() => LevelKeys = FindObjectsOfType<KeyBehaviour>();
    
    [Button]
    public void FindNPCs() => LevelNPCs = FindObjectsOfType<NPC>();
}
