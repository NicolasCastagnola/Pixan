using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SitState : BaseAnimationState
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("Sitting", true);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _characterView.Handler.AnimationFinished();
        animator.SetBool("Sit", false);
    }
}
