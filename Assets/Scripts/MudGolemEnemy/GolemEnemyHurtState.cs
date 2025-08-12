using System.Collections;
using System.Collections.Generic;
using Audio;
using UnityEngine;
using KinematicCharacterController;

public class GolemEnemyHurtState : IState
{
    MudGolemEnemy _enemy;
    private readonly AudioConfigurationData _audioConfigurationData;

    public GolemEnemyHurtState(MudGolemEnemy enemy, AudioConfigurationData audioConfigurationData)
    {
        _enemy = enemy;
        _audioConfigurationData = audioConfigurationData;
    }

    public void OnEnter()
    {
        if (_enemy.EnemyPreValidations()) return;
        
        _audioConfigurationData.Play3D();
        _enemy.PlayAnimation("Hurt");
        GameManager.instance.WaitForXSeconds(0.5f, () => _enemy.StateMachine.ChangeState(States.EnemyIdle));
    }

    public void OnExit()
    {
        //GameManager.ShowLog("EnemyChase exit");
    }

    public void OnUpdate()
    {
        AICharacterInputs inputs = new AICharacterInputs();
        inputs.MoveVector = Vector3.zero;
        _enemy.Controller.SetInputs(ref inputs);

        if (_enemy.EnemyPreValidations()) return;
    }
    public void OnFixedUpdate()
    {
       
    }
}
