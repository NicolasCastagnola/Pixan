using System;
using System.Collections;
using System.Collections.Generic;
using Audio;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bonfire : Interactable
{
    [SerializeField] private GameObject fireContainer;

    [SerializeField] private ParticleSystem BonfireLitParticles;
    [SerializeField] private AudioConfigurationData restOrLitBonfireSound, idleBonfireSound;
    
    [ShowInInspector, ReadOnly] private string bonfireID;
    [ShowInInspector, ReadOnly] public bool BonfireUnlocked { get; private set; }
    [field: SerializeField] public string BonfireName { get; private set; }
    [field: SerializeField] public Transform BonfireTravelPoint { get; private set; }
    [field: SerializeField, MinMaxSlider(1f, 15f)] public Vector2 BonfireSoundDistance { get; private set; }

    public Action SaveStats;

    public void Initialize()
    {
        bonfireID = $"{SceneManager.GetActiveScene().name}/{BonfireName}";

        int value = PlayerPrefs.GetInt(bonfireID, 0);

        BonfireUnlocked = value == 1;

        fireContainer.SetActive(BonfireUnlocked);

        if (BonfireUnlocked)
        {
            idleBonfireSound.Play3D(bonfireID, new Audio.AudioManager.SoundParameters(transform, BonfireSoundDistance));
        }
    }

    public void Terminate()
    {
        idleBonfireSound.Stop(bonfireID);
    }

    public void UnlockBonfire()
    {
        OnHoverMessage = "Rest";
        PlayerPrefs.SetInt(bonfireID, 1);
        BonfireUnlocked = true;
        fireContainer.SetActive(true);
        BonfireLitParticles.Play();
        
        idleBonfireSound.Play3D(bonfireID, new Audio.AudioManager.SoundParameters(transform, BonfireSoundDistance));
        
        restOrLitBonfireSound.Play2D();
    }
    public override void OnEnterReachableRadius(InteractComponent component)
    {
        base.OnEnterReachableRadius(component);

        OnHoverMessage = BonfireUnlocked ? "Rest" : "Lit";
    }
    public override void OnInteract()
    {
        if (Canvas_Playing.InBonfireMenu) return;
        
        BonfireManager.Instance.SetLastInteractedBonfire(this);
        
        if (BonfireUnlocked)
        {
            componentWithinReach.PlayerComponent.SitInBonfire();
            
            restOrLitBonfireSound.Play2D();
        }
        else
        {
            UnlockBonfire();
        }
    }
    public override void OnExitOnReachableRadius()
    {
    }
}
