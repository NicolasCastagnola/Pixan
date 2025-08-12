using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : BaseAnimationState
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("TopBodyLayer", true);
        _characterView.Handler.AttackWeaponAnim();
        //_characterView.Handler.EnableWeaponColission();
        _characterView.Play2DAudio(PlayerSounds.Sounds.soundAttack);
        _characterView.Handler.EnableParticleSystem();
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("TopBodyLayer", false);
        _characterView.Handler.DisableWeaponColission();
        _characterView.Handler.CanAttack();
        _characterView.Handler.IdleWeaponAnim();
        _characterView.Handler.DisableParticleSystem();
        _characterView.Handler.AnimationFinished();
    }
}
