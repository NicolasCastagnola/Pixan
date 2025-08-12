using System.Collections;
using System.Collections.Generic;
using Audio;
using UnityEngine;
using DG.Tweening;
using KinematicCharacterController;
public class GolemEnemyDeathState : IState
{
    private readonly MudGolemEnemy _enemy;
    private readonly AudioConfigurationData _audioConfigurationData;


    public GolemEnemyDeathState(MudGolemEnemy enemy, AudioConfigurationData audioConfigurationData)
    {
        _enemy = enemy;
        _audioConfigurationData = audioConfigurationData;

    }

    public void OnEnter()
    {
        _enemy.AnimatorController.TriggerDeath();
        
        _audioConfigurationData.Play3D();
    }

    public void OnExit()
    {
    }

    public void OnUpdate()
    {
        var inputs = new AICharacterInputs
        {
            MoveVector = Vector3.zero
        };
        
        _enemy.Controller.SetInputs(ref inputs);
    }
    public void OnFixedUpdate()
    {
        
    }
}
