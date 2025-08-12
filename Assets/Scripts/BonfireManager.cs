using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class BonfireManager : BaseMonoSingleton<BonfireManager>
{
    [ShowInInspector, ReadOnly] public Bonfire LastInteractedBonfire { get; private set; }
    [field: SerializeField] public List<Bonfire> LevelBonfires { get; private set; }


    protected override void Awake()
    {
        base.Awake();

        foreach (var bonfire in LevelBonfires)
        {
            bonfire.Initialize();
        }
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        
        foreach (var bonfire in LevelBonfires)
        {
            bonfire.Terminate();
        }
    }

    [Button]
    public void UnlockAll()
    {
        foreach (var bonfire in LevelBonfires)
        {
            bonfire.UnlockBonfire();
        }
    }

    [Button] 
    public void FindBonfires() => LevelBonfires = FindObjectsOfType<Bonfire>().ToList();
    public void SetLastInteractedBonfire(Bonfire bonfire)
    {
        PlayerPrefs.SetInt(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name + ".LastRestedBonfire", LevelBonfires.IndexOf(bonfire));
        LastInteractedBonfire = bonfire;
    }
}
