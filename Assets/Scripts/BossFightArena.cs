using System;
using System.Collections.Generic;
using Audio;
using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BossFightArena : MonoBehaviour
{
    public event Action OnArenaInitialization, OnArenaTermination;

    private Player _player;
    [SerializeField, Tooltip("If true, bossfights will reespawn even if has been defeated")] bool reviveDefeated;
    [ShowInInspector, ReadOnly] private string bossFightID => gameObject.name;
    [ShowInInspector, ReadOnly] private int remainingBosses;
    
    [field:SerializeField] public List<Transform> AvailableSpawnPoints { get; private set; }
    
    [SerializeField] private AudioConfigurationData bossFightMusic;
    [SerializeField] private AudioSource audioSource;
    
    [SerializeField] private FogWall entranceFogWall;
    [SerializeField] private GameObject[] fogWallBlockers;
    [SerializeField] private Boss[] arenaBosses;

    public void Initialize(Player playerReference)
    {
        remainingBosses = arenaBosses.Length;
        
        _player = playerReference;
        _player.OnDeath += PlayerDeath;

        audioSource = GetComponent<AudioSource>();

        if (entranceFogWall!=null)
            entranceFogWall.OnTraverseFogFinished += InitializeArena;

        bool alreadyKill = PlayerPrefs.GetInt(bossFightID, 0) == 1;

        if (alreadyKill && !reviveDefeated)
        {
            foreach (var boss in arenaBosses)
            {
                boss.gameObject.SetActive(false);
            }
            
            SetRoutesBlockers(false);
            if (entranceFogWall != null)
                entranceFogWall.gameObject.SetActive(false);
        }
    }
    private void PlayerDeath(HealthComponent.HealthModificationReport obj) => bossFightMusic.Stop();
    private void SetRoutesBlockers(bool value)
    {
        foreach (var item in fogWallBlockers)
        {
            item.SetActive(value);
        }
    }
    private void OnDestroy()
    {
        _player.OnDeath -= PlayerDeath;
        if (entranceFogWall != null)
            entranceFogWall.OnTraverseFogFinished -= InitializeArena;
    }
    private void TerminateArena()
    {
        foreach (var boss in arenaBosses)
        {
            boss.Health.OnDead -= ArenaBossDied;
        }
        
        SetRoutesBlockers(false);

        if (entranceFogWall != null)
            entranceFogWall.gameObject.SetActive(false);
        
        PlayerPrefs.SetInt(bossFightID, 1);
        
        _player.OnDeath -= PlayerDeath;

        if (entranceFogWall != null)
            entranceFogWall.OnTraverseFogFinished -= InitializeArena;
        
        OnArenaTermination?.Invoke();
    }
    public void InitializeArena()
    {
        foreach (var boss in arenaBosses)
        {
            boss.Health.OnDead += ArenaBossDied;
            boss.SetArena(this);
            boss.InitializeAI();
        }

        audioSource.clip = bossFightMusic.clip;
        //audioSource.outputAudioMixerGroup = _mainTheme.channel.ToString();
        audioSource.volume = bossFightMusic.volume;
        audioSource.loop = bossFightMusic.loop;
        audioSource.Play();
        
        SetRoutesBlockers(true);
        
        OnArenaInitialization?.Invoke();
    }
    private void ArenaBossDied(HealthComponent.HealthModificationReport healthModificationReport)
    {
        remainingBosses--;

        audioSource.Stop();
        
        if (remainingBosses == 0)
        {
            TerminateArena();
        }
    }
}
