using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController;

public class MeleeEnemyStunState : IState
{
    private readonly MudEnemyMelee _mudEnemyMelee;
    private readonly float _stunDuration;
    private readonly float _damageMultiplier;
    private float _timer;

    public MeleeEnemyStunState(MudEnemyMelee mudEnemyMelee,float stunDuration = 2f, float damageMultiplier = 2f)
    {
        _mudEnemyMelee = mudEnemyMelee;
        _stunDuration = stunDuration;
        _damageMultiplier = damageMultiplier;
    }

    public void OnEnter()
    {
        if (_mudEnemyMelee.EnemyPreValidations()) return;

        _mudEnemyMelee.Health.SetDamageMultiplier(_damageMultiplier);
        _mudEnemyMelee.PlayAnimation("StunState");
        
        _timer = 0;
    }

    public void OnExit(){}

    public void OnUpdate()
    {
        var inputs = new AICharacterInputs
        {
            MoveVector = Vector3.zero
        };
        _mudEnemyMelee.Controller.SetInputs(ref inputs);

        _timer += Time.deltaTime;

        if (_timer >= _stunDuration) _mudEnemyMelee.StateMachine.ChangeState(States.EnemyIdle);
    }
    public void OnFixedUpdate()
    {
        
    }
}
