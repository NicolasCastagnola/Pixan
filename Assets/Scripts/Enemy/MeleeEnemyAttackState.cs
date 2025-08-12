using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController;

public class MeleeEnemyAttackState : IState
{
    private readonly MudEnemyMelee _mudEnemyMelee;
    private bool isAttacking;
    private float timerToMove;
    private readonly float maxTime;

    public MeleeEnemyAttackState(MudEnemyMelee mudEnemyMelee, float mTime)
    {
        _mudEnemyMelee = mudEnemyMelee;
        maxTime = mTime;
    }

    public void OnEnter()
    {
        if (_mudEnemyMelee.EnemyPreValidations()) return;
        _mudEnemyMelee.PlayAnimation("AttackState");

        isAttacking = true;
        timerToMove = 0;
    }

    public void OnExit()
    {

    }
    public void OnFixedUpdate(){}
    public void OnUpdate()
    {
        if (_mudEnemyMelee.PlayerTarget.isDead)
        {
            _mudEnemyMelee.StateMachine.ChangeState(States.EnemyIdle);
            return;
        }
        
        Vector3 moveVector = Vector3.zero;
        Vector3 lookVector = (_mudEnemyMelee.PlayerTarget.transform.position - _mudEnemyMelee.transform.position).normalized;
        lookVector.y = 0;

        if (Vector3.Distance(_mudEnemyMelee.transform.position, _mudEnemyMelee.PlayerTarget.transform.position) < 1.5f)
        {
            moveVector = _mudEnemyMelee.transform.position - _mudEnemyMelee.PlayerTarget.transform.position;
            moveVector.Normalize();
        }

        var inputs = new AICharacterInputs
        {
            MoveVector = moveVector * 0.2f,
            LookVector = lookVector
        };
        
        _mudEnemyMelee.Controller.SetInputs(ref inputs);

        if (_mudEnemyMelee.EnemyPreValidations()) return;
        if (isAttacking)
        {
            timerToMove += Time.deltaTime;
 
            if (timerToMove >= maxTime)
            {
                isAttacking = false;
                timerToMove = 0;    
            }

            return;
        }

        if (!_mudEnemyMelee.PlayerWithinAwarenessRadius() && _mudEnemyMelee.StateMachine.States.Contains(States.EnemyIdle))
        {
            _mudEnemyMelee.StateMachine.ChangeState(States.EnemyIdle);
        }
        
        if (ShouldStartChasing())
        {
            _mudEnemyMelee.StateMachine.ChangeState(States.EnemyChase);
        }
    }


    private bool ShouldStartChasing() => Vector3.Distance(_mudEnemyMelee.PlayerTarget.transform.position, _mudEnemyMelee.transform.position) > _mudEnemyMelee.ValidRangeToAttack 
                                         && _mudEnemyMelee.StateMachine.States.Contains(States.EnemyChase);

}
