using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockState : BaseAnimationState
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("TopBodyLayer", true);
        _characterView.Handler.EnableBlockingColission();
        if (_characterView.Handler.IsMoving())// && !animator.GetBool("IsMoving")) //Si esto no estuviese, el player se buguearía al rollear y después bloquear
        {
            animator.SetBool("IsMoving", true);
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("TopBodyLayer", false);
        _characterView.Handler.DisableBlockingColission();
        _characterView.Handler.AnimationFinished();
    }
}
