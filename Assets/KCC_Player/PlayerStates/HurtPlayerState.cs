using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerAnimations;

public class HurtPlayerState : PlayerState
{
    Animations _currentAnimation = Animations.Idle;
    public HurtPlayerState(Player p) : base(p)
    {
    }

    public override void OnEnter()
    {
        _player.view.ChangeAnim(Animations.Hurt);
        _player.Controller.OnAnimationFinished = () =>
        {
            Transitions();
        };
        _player.Controller.HaltVelocity(0.01f);
        //_player.SlowDownTime();
    }

    public override void OnUpdate()
    {
        //IdleOrWalk();
    }
    void IdleOrWalk()
    {
        _player.Controller.Walk(_player.Inputs, Time.deltaTime);
        if (inputs.MoveAxisRight == 0 && inputs.MoveAxisForward == 0 && _currentAnimation != Animations.Idle)
        {
            _player.view.SetBool("Idle");
            _player.view.SetBool("IsMoving", false);
            _currentAnimation = Animations.Idle;
        }
        else if ((inputs.MoveAxisRight != 0 || inputs.MoveAxisForward != 0) && _currentAnimation != Animations.Walk)
        {
            _player.view.SetBool("IsMoving");
            _player.view.SetBool("Idle", false);
            _currentAnimation = Animations.Walk;
        }
    }
    protected override bool Transitions()
    {
        _player.ChangeState(States.PlayerIdle);
        return true;
    }
}