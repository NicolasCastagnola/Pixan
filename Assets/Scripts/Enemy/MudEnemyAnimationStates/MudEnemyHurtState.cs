using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MudEnemyHurtState : MudEnemyBaseState
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _mudEnemyMelee.Hurt.Play2D();
        //_mudEnemyMelee.Health.SetInvulnerability(true);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //_mudEnemyMelee.Health.SetInvulnerability(false);
        if (!_mudEnemyMelee.Health.IsDead)
        {
            _mudEnemyMelee.StateMachine.ChangeState(States.EnemyIdle);
        }
    }
}