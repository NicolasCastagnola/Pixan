using System.Collections;
using System.Collections.Generic;
using Audio;
using UnityEngine;
using KinematicCharacterController;

public class GolemEnemyStunState : IState
{
    MudGolemEnemy _enemy;
    private readonly AudioConfigurationData _audioConfigurationData;

    float timer;

    public GolemEnemyStunState(MudGolemEnemy enemy, AudioConfigurationData audioConfigurationData)
    {
        _enemy = enemy;
        _audioConfigurationData = audioConfigurationData;
    }

    public void OnEnter()
    {
        if (_enemy.EnemyPreValidations()) return;
        
        _enemy.PlayAnimation("Stunned");
        
        _audioConfigurationData.Play2D();
        
        GameManager.instance.WaitForXSeconds(1.433f, () => _enemy.StateMachine.ChangeState(States.EnemyIdle));
    }

    public void OnExit()
    {
        //GameManager.ShowLog("StunState exit");
    }

    public void OnUpdate()
    {
        AICharacterInputs inputs = new AICharacterInputs();
        inputs.MoveVector = Vector3.zero;
        _enemy.Controller.SetInputs(ref inputs);
    }
    public void OnFixedUpdate()
    {
        
    }
}
