using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MudEnemyDeathState : MudEnemyBaseState
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _mudEnemyMelee.DeathSound.Play2D();
        _mudEnemyMelee.Controller.enabled = false;
        _mudEnemyMelee.Motor.Capsule.enabled = false;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}