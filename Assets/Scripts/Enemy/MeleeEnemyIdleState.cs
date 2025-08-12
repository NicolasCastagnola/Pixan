using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController;

public class MeleeEnemyIdleState : IState
{
    MudEnemyMelee _mudEnemyMelee;

    public MeleeEnemyIdleState(MudEnemyMelee mudEnemyMelee)
    {
        _mudEnemyMelee = mudEnemyMelee;
    }

    public void OnEnter()
    {
        _mudEnemyMelee.PlayAnimation("IdleState");
    }

    public void OnExit(){}

    public void OnUpdate()
    {
        if (_mudEnemyMelee.EnemyPreValidations())
            return;

        AICharacterInputs inputs = new AICharacterInputs();
        inputs.MoveVector = Vector3.zero;
        _mudEnemyMelee.Controller.SetInputs(ref inputs);

        if (_mudEnemyMelee.PlayerCanBeSeen())
        {
            _mudEnemyMelee.StateMachine.ChangeState(States.EnemyChase);
        }
        else
            _mudEnemyMelee.PlayAnimation("IdleState");
    }
    public void OnFixedUpdate(){}
}
