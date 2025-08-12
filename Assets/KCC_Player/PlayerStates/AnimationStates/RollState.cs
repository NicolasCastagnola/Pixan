using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollState : BaseAnimationState
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _characterView.ResetAllBools();
        _characterView.Handler.DisablePlayerDmg();
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _characterView.Handler.EnablePlayerDmg();
        _characterView.Handler.AnimationFinished();
    }
}
