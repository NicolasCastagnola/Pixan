using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealState : BaseAnimationState
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("TopBodyLayer", true);
        _characterView.Play2DAudio(PlayerSounds.Sounds.soundHeal);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("TopBodyLayer", false);
        animator.SetBool("Heal", false);
        _characterView.Handler.AnimationFinished();
    }
}
