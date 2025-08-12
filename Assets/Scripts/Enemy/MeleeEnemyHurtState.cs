using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController;

public class MeleeEnemyHurtState : IState
{
    MudEnemyMelee _mudEnemyMelee;

    public MeleeEnemyHurtState(MudEnemyMelee mudEnemyMelee)
    {
        _mudEnemyMelee = mudEnemyMelee;
    }

    public void OnEnter()
    {
        if (_mudEnemyMelee.EnemyPreValidations()) return;

        _mudEnemyMelee.PlayAnimation("HurtState");
    }

    public void OnExit() { }

    public void OnUpdate()
    {
        AICharacterInputs inputs = new AICharacterInputs();
        inputs.MoveVector = Vector3.zero;
        _mudEnemyMelee.Controller.SetInputs(ref inputs);
    }
    public void OnFixedUpdate()
    {
       
    }
}
