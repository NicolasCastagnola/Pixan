using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkState : BaseAnimationState
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _characterView.Play2DAudio(PlayerSounds.Sounds.soundWalk);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _characterView.StopAudio(PlayerSounds.Sounds.soundWalk);
    }
}
