using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MudGolemAnimatorController : EntityAnimatorController
{
    [SerializeField] private MudGolemEnemy _mudGolemEnemy;
    [SerializeField] private EnemyColliderDamage stompColliderDamage;
    [SerializeField] private ParticleSystem StompParticles;
    [SerializeField] private Transform StompParticlesPivot;
    [SerializeField] private GameObject grabbedMud;

    private void Awake()
    {
        stompColliderDamage.SetDamage(_mudGolemEnemy.StompDamage);
    }
    public void Throw()
    {
        grabbedMud.SetActive(false);
        
        _mudGolemEnemy.ThrowBullet();
    }
    public void StartStomp()
    {
        grabbedMud.SetActive(true);
    }
    public void EndStomp()
    {
        grabbedMud.SetActive(false);
        
        Instantiate(StompParticles, StompParticlesPivot.position, Quaternion.identity);
        
        ActivateCollider();
        
        _mudGolemEnemy.Stomp();
    }
    public void EndSummon() => _mudGolemEnemy.EndSummoningMinions();
    public void StartSummon() => _mudGolemEnemy.StartSummoningMinions();
    public void ActivateCollider() => stompColliderDamage.Activate();
    public void DeactivateCollider() => stompColliderDamage.Deactivate();

}
