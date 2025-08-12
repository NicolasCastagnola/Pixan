using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerAnimations;
using PlayerSounds;
//using UnityEditor.Timeline.Actions;

public class ChargedAttackPlayerState : PlayerState
{
    Animations _currentAnimation = Animations.Idle;
    public ChargedAttackPlayerState(Player p) : base(p)
    {
    }

    public override void OnEnter()
    {
        _player.view.ChangeAnim(Animations.ChargedAttack);
        _player.Stamina.ConsumeStamina(20f);
        _player.Controller.charhedAttackAction.actionConsumed = true;
        _player.Controller.charhedAttackAction.isDoingAction = true;
        _player.Controller.charhedAttackAction.actionFinished = false;
        _player.Controller.charhedAttackAction.animationFinished = false;

        _player.Controller.OnActionFinished = () =>
        {
            _player.Controller.charhedAttackAction.actionConsumed = false;

        };
        _player.Controller.OnAnimationFinished = () =>
        {
            if (_player.view.GetCurrentAnimation() == Animations.Sit)
            {
                //_player.ChangeState(States.PlayerInBonfire);
                return;
            }
            else
                Transitions();
        };
    }
    public override void OnExit()
    {
        _player.Controller.charhedAttackAction.actionConsumed = false;
    }

    public override void OnUpdate()
    {
        IdleOrWalk();
        _player.view.SetBool("IsMoving", false);
    }
    void IdleOrWalk()
    {

        var inputs = _player.Inputs;
        inputs.MoveAxisForward *= 0.1f;
        inputs.MoveAxisRight *= 0.1f;
        _player.Controller.Walk(inputs, Time.deltaTime, false, true);

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
