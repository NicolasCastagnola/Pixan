using System.Collections;
using System.Collections.Generic;
using Audio;
using UnityEngine;
using KinematicCharacterController;

public class GolemEnemyRangeAttackState : IState
{
    private readonly MudGolemEnemy _owner;
    private readonly AudioConfigurationData _audioConfigurationData;

    private bool isAttacking;

    public GolemEnemyRangeAttackState(MudGolemEnemy owner, AudioConfigurationData audioConfigurationData)
    {
        _owner = owner;
        _audioConfigurationData = audioConfigurationData;
    }

    public void OnEnter()
    {
        if (_owner.EnemyPreValidations()) return;

        _audioConfigurationData.Play3D();
        
        _owner.StartCoroutine(Throw());
    }
    public void OnExit(){}
    public void OnFixedUpdate(){}
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
        
        if (!isAttacking && _owner.InStompRange())
        {
            _owner.StateMachine.ChangeState(States.EnemyAttack);
        }
        
        if (!isAttacking && _owner.HasThrowTokens && _owner.InThrowRange())
        {
            _owner.StartCoroutine(Throw());
        }
        
        if (!isAttacking && !_owner.HasThrowTokens)
        {
            _owner.StateMachine.ChangeState(States.EnemyChase);
        }
        
    }
    private IEnumerator Throw()
    {
        isAttacking = true;

        _owner.AnimatorController.TriggerThrow();
        
        yield return new WaitForSeconds(2.267f);

        isAttacking = false;
    }
}
