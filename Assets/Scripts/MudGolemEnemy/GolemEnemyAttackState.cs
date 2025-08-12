using System.Collections;
using System.Collections.Generic;
using Audio;
using UnityEngine;
using KinematicCharacterController;

public class GolemEnemyAttackState : IState
{
    private readonly AudioConfigurationData _audioConfigurationData;
    private readonly MudGolemEnemy _owner;
    private bool isAttacking;

    public GolemEnemyAttackState(MudGolemEnemy owner, AudioConfigurationData audioConfigurationData)
    {
        _owner = owner;
        _audioConfigurationData = audioConfigurationData;
    }
    public void OnEnter()
    {
        if (_owner.EnemyPreValidations()) return;
        
        _audioConfigurationData.Play3D();
        
        _owner.StartCoroutine(Attack());

    }
    public void OnFixedUpdate(){}
    public void OnExit(){}
    public void OnUpdate()
    {
        if (_owner.EnemyPreValidations()) return;
        
        var towardsDir = (_owner.PlayerTarget.transform.position - _owner.transform.position).normalized;
        towardsDir.y = 0;
        
        var inputs = new AICharacterInputs
        {
            MoveVector = Vector3.zero,
            LookVector = towardsDir
        };
        
        _owner.Controller.SetInputs(ref inputs);
        
        if (!_owner.HasThrowTokens && !_owner.InThrowRange() && !_owner.InStompRange())
        {
            _owner.StateMachine.ChangeState(States.EnemyChase);
        }
        if (!isAttacking && !_owner.HasThrowTokens && _owner.InStompRange())
        {
            _owner.StartCoroutine(Attack());
        }
        if (_owner.HasThrowTokens && _owner.InThrowRange())
        {
            _owner.StateMachine.ChangeState(States.EnemyRangeAttack);
        }
    }

    private IEnumerator Attack()
    {
        isAttacking = true;
        
        _owner.AnimatorController.TriggerStomp();
        
        yield return new WaitForSeconds(2f);
        
        isAttacking = false;
    }

 
}
