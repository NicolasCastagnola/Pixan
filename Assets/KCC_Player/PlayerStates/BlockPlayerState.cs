using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerAnimations;

public class BlockPlayerState : PlayerState
{
    Animations _currentAnimation = Animations.Idle;

    public BlockPlayerState(Player p) : base(p)
    {
    }

    public override void OnEnter()
    {
        _player.Stamina.ConsumeStamina(10f);
        _player.view.ChangeAnim(Animations.Block);
        //if (inputs.MoveAxisRight == 0 && inputs.MoveAxisForward == 0 && _currentAnimation != Animations.Idle)
        //{
            //_player.view.SetBool("IsMoving");
        //}
        _player.Controller.Block();
        _player.Controller.blockAction.isDoingAction = true;

        _player.Controller.OnAnimationFinished = () =>
        {
            //_player.character.attackAction.actionFinished = true;
            //_player.character.attackAction.actionConsumed = false;
            //_player.character.attackAction.actionAmmount++;
            //_player.ChangeState(States.PlayerIdle);
            //_player.view.ChangeAnim(Animations.Idle);
            //_player.character.attackAction.actionAmmount = 0;
        };
    }

    public override void OnUpdate()
    {
        if (inputs.BlockUp)
        {
            if (_player.view.GetCurrentAnimation() == Animations.Sit)
            {
                return;
            }
            else
                Transitions();
        }
        
        IdleOrWalk();
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
            _player.Stamina.ConsumeStamina(5f * Time.deltaTime);
            
            _player.view.SetBool("IsMoving");
            _player.view.SetBool("Idle", false);
            _currentAnimation = Animations.Walk;
        }
    }
    public override void OnExit()
    {
        _player.Controller.blockAction.isDoingAction = false;
    }

    protected override bool Transitions()
    {
        _player.view.ChangeAnim(Animations.Idle);
        _player.ChangeState(States.PlayerIdle);
        return true;
    }
}