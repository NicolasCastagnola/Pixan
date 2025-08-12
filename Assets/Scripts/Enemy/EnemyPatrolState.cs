using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using KinematicCharacterController;

public class EnemyPatrolState : IState
{
    MudEnemyMelee _mudEnemyMelee;

    int currentWaypoint = 0;

    public EnemyPatrolState(MudEnemyMelee mudEnemyMelee)
    {
        _mudEnemyMelee = mudEnemyMelee;
    }

    public void OnEnter()
    {
        //GameManager.ShowLog("EnemyPatrol enter");
        if (_mudEnemyMelee.EnemyPreValidations()) return;
        // if( _enemy.Waypoints.Count > 0 && _enemy.Waypoints[0] != null) 
        // { 
        //     _enemy.RotateToTarget(_enemy.Waypoints[currentWaypoint].position - _enemy.transform.position, 0.25f);
        //     _enemy.PlayAnimation("PatrolState");
        //
        // }
    }

    public void OnExit()
    {
        //GameManager.ShowLog("EnemyPatrol exit");
        AICharacterInputs inputs = new AICharacterInputs();
        inputs.MoveVector = Vector3.zero;
        _mudEnemyMelee.Controller.SetInputs(ref inputs);
    }

    public void OnUpdate()
    {
        AICharacterInputs inputs = new AICharacterInputs();

        if (_mudEnemyMelee.EnemyPreValidations()) return;
        if (_mudEnemyMelee.PlayerWithinAwarenessRadius())
        {
            _mudEnemyMelee.StateMachine.ChangeState(States.EnemyChase);
        }

        // if (_enemy.Waypoints.Count > 0 && _enemy.Waypoints[0] != null) 
        // {
        //     if (Vector3.Distance(_enemy.transform.position, _enemy.Waypoints[currentWaypoint].position) > 0.5f)
        //     {
        //         inputs.MoveVector = _enemy.Waypoints[currentWaypoint].position - _enemy.transform.position;
        //     }
        //     else {
        //         if (currentWaypoint == _enemy.Waypoints.Count - 1) currentWaypoint = 0;
        //         else currentWaypoint++;
        //
        //         inputs.LookVector=(_enemy.Waypoints[currentWaypoint].position - _enemy.transform.position).normalized;
        //
        //         _enemy.StateMachine.ChangeState(States.EnemyIdle);
        //    
        //     }
        //     _enemy.Controller.SetInputs(ref inputs);
        // }
    }
    public void OnFixedUpdate()
    {
     
    }
}
