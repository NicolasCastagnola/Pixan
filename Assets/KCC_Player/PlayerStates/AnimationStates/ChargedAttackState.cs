using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargedAttackState : BaseAnimationState
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("TopBodyLayer", true);
        _characterView.Handler.EnableParticleSystem();
        _characterView.Handler.AttackWeaponAnim();
        _characterView.Play2DAudio(PlayerSounds.Sounds.soundChargeAttack);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("TopBodyLayer", false);
        _characterView.Handler.DisableParticleSystem();
        _characterView.Handler.IdleWeaponAnim();
        _characterView.Handler.AnimationFinished();
    }
}
