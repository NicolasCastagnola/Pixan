using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MudEnemyStunState : MudEnemyBaseState
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _mudEnemyMelee.Health.SetCriticalState(true);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _mudEnemyMelee.Health.SetCriticalState(false);
    }
}