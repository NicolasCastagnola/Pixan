using System.Collections;
using System.Collections.Generic;
using Audio;
using UnityEngine;
using DG.Tweening;
using KinematicCharacterController;

public class GolemEnemyChaseState : IState
{
    private readonly MudGolemEnemy _owner;
    private readonly AudioConfigurationData _audioConfigurationData;

    public GolemEnemyChaseState(MudGolemEnemy owner, AudioConfigurationData audioConfigurationData)
    {
        _owner = owner;//play steps
        _audioConfigurationData = audioConfigurationData;
    }
    public void OnEnter()
    {
        if (_owner.EnemyPreValidations()) return;
        
        _owner.PlayAnimation("Chase");
    }
    public void OnExit(){}
    public void OnFixedUpdate(){}
    public void OnUpdate()
    {
        var inputs = new AICharacterInputs();

        if (_owner.EnemyPreValidations()) return;
        
        if (_owner.CanSummonsMinions)
        {
            _owner.StateMachine.ChangeState(States.EnemySpawnMinions);
        }
        if (_owner.HasThrowTokens && _owner.InThrowRange())
        {
            _owner.StateMachine.ChangeState(States.EnemyRangeAttack);
        }
        if (_owner.InStompRange())
        {
            _owner.StateMachine.ChangeState(States.EnemyAttack);
            return;
        }
        if (!_owner.PlayerOnSight())
        {
            _owner.StateMachine.ChangeState(States.EnemyIdle);
            return;
        }
        
        var normalizedDirectionTowardTarget = (_owner.PlayerTarget.transform.position - _owner.transform.position).normalized;
        inputs.LookVector = new Vector3(normalizedDirectionTowardTarget.x, 0f, normalizedDirectionTowardTarget.z);
        inputs.MoveVector = normalizedDirectionTowardTarget * (Time.deltaTime * _owner.EntityStats.rawWalkSpeed);
        
        _owner.Controller.SetInputs(ref inputs);
    }
}
