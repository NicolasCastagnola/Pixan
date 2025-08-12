using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtState : BaseAnimationState
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _characterView.ResetAllBools();
        animator.SetBool("TopBodyLayer", true);
        _characterView.Play2DAudio(PlayerSounds.Sounds.soundGolpe);
        //_characterView.Handler.DisableWeaponColission();
        _characterView.Handler.DisablePlayerDmg();
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("TopBodyLayer", false);
        _characterView.Handler.EnablePlayerDmg();
        _characterView.Handler.AnimationFinished();
    }
}
