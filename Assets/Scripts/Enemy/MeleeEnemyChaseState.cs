using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using KinematicCharacterController;


public class MeleeEnemyChaseState : IState
{
    private readonly MudEnemyMelee _owner;

    public MeleeEnemyChaseState(MudEnemyMelee mudEnemyMelee)
    {
        _owner = mudEnemyMelee;
    }

    public void OnEnter()
    {
        if (_owner.EnemyPreValidations()) return;
        _owner.PlayAnimation("ChaseState");

        _owner.Controller.Motor.enabled = false;
        _owner.Controller.enabled = false;

        RecalculatePath();
    }

    public void OnExit()
    {
        Vector3 pos = _owner.transform.position;
        Quaternion rot = _owner.transform.rotation;

        _owner.Controller.Motor.enabled = true;
        _owner.Controller.enabled = true;
        _owner.Controller.Motor.SetPositionAndRotation(pos, rot);
    }

    public void OnUpdate()
    {
        if (_owner.EnemyPreValidations() || _owner.PlayerTarget == null) return;

        if (!_owner.PlayerCanBeSeen() && !_owner.PlayerWithinAwarenessRadius())
            _owner.StateMachine.ChangeState(States.EnemyIdle);

        if (_owner.WithInAttackRange()) _owner.StateMachine.ChangeState(States.EnemyAttack);
        
        else ChaseTarget();
    }

    private void ChaseTarget()
    {
        if (_owner.currentCooldown <= 0)
            RecalculatePath();
        else
            _owner.currentCooldown -= Time.deltaTime;

        var player = _owner.PlayerTarget.CenterOfMassPivot.position;
        player.y = _owner.transform.position.y;
        _owner.transform.forward = player - _owner.transform.position;
    }
    void RecalculatePath()
    {
        _owner.currentCooldown = _owner.cooldown;
        _owner.NavMeshAgent.SetDestination(_owner.PlayerTarget.CenterOfMassPivot.position);
        _owner.NavMeshAgent.speed = _owner.EntityStats.rawWalkSpeed;
    }

    public void OnFixedUpdate()
    {
       
    }
}
