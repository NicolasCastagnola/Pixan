using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController;

public class GolemEnemyIdleState : IState
{
    private readonly MudGolemEnemy _owner;
    public GolemEnemyIdleState(MudGolemEnemy owner)
    {
        _owner = owner;
    }
    public void OnEnter() => _owner.PlayAnimation("Idle");
    public void OnExit()
    {
    }
    public void OnUpdate()
    {
        if (!_owner.IsInitialized)
            return;

        var inputs = new AICharacterInputs
        {
            MoveVector = Vector3.zero
        };
        
        _owner.Controller.SetInputs(ref inputs);

        if (!_owner.PlayerOnSight()) return;
            
        if (Vector3.Distance(_owner.PlayerTarget.transform.position, _owner.transform.position) < _owner.chaseRange)
        {
            _owner.StateMachine.ChangeState(States.EnemyChase);
        }
        else
        {
            _owner.StateMachine.ChangeState(States.EnemyRangeAttack);
        }
    }
    public void OnFixedUpdate()
    {
       
    }
}
