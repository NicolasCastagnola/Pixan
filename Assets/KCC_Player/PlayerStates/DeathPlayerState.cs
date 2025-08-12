using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerAnimations;

public class DeathPlayerState : PlayerState
{

    public DeathPlayerState(Player p) : base(p)
    {
    }

    public override void OnEnter()
    {
        _player.view.ChangeAnim(Animations.Death);
        _player.Controller.deadAction.isDoingAction = true;
        _player.Controller.OnAnimationFinished = () =>
        {
        };
        GameManager.instance.WaitForXSeconds(3, () =>
        {
            ParticlesManager.Instance.SpawnParticles(_player.transform.position + new Vector3(0, 1, 0), "Death");
            // UtilitiesManager.Instance.EndGame();
        });

        GameManager.instance.WaitForXSeconds(5, () =>
        {
            // UtilitiesManager.Instance.EndGameReset();
        });
    }

    public override void OnUpdate()
    {
        _player.Controller.Walk(default, Time.deltaTime);
    }

    protected override bool Transitions()
    {
        _player.ChangeState(States.PlayerIdle);
        return true;
    }
}