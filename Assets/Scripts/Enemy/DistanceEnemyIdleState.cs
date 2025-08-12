using KinematicCharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceEnemyIdleState : IState
{
    private readonly MudEnemyDistance _owner;

    public DistanceEnemyIdleState(MudEnemyDistance owner)
    {
        _owner = owner;
    }
    public void OnEnter() => _owner.PlayAnimation("IdleState");
    public void OnExit(){}
    public void OnFixedUpdate(){}
    public void OnUpdate()
    {
        if (_owner.EnemyPreValidations()) return;
        
        var inputs = new AICharacterInputs{MoveVector = Vector3.zero};
        
        _owner.Controller.SetInputs(ref inputs);

        if (_owner.PlayerWithinDistanceAttackRadius())
        {
            _owner.StateMachine.ChangeState(States.EnemyDistanceAttack);
        }
    }
}
