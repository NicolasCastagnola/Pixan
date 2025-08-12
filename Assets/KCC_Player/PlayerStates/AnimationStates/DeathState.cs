using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathState : BaseAnimationState
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _characterView.ResetAllBools();
        _characterView.Play2DAudio(PlayerSounds.Sounds.soundDeath);
        _characterView.Handler.DisableWeaponColission();
        _characterView.Handler.DisablePlayerDmg();
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _characterView.Handler.AnimationFinished();
        _characterView.ResetAllBools();
    }
}
