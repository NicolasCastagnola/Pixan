using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerAnimations;
//using UnityEditor.Timeline.Actions;

public class AttackPlayerState : PlayerState
{
    Animations _currentAnimation = Animations.Idle;

    public AttackPlayerState(Player p) : base(p)
    {
    }

    public override void OnEnter()
    {

        if (_player.TargetLocking.LockingActive)
        {
            _player.Motor.RotateCharacter(Quaternion.LookRotation((_player.TargetLocking.CurrentTarget.transform.position - _player.transform.position).normalized));
        }
        
        switch (_player.Controller.attackAction.actionAmmount)
        {
            case 1:
                _player.view.ChangeAnim(Animations.Attack1);
                _player.Controller.attackAction.actionAmmount = 0;
                break;
            default:
                _player.Controller.attackAction.actionAmmount++;
                _player.view.ChangeAnim(Animations.Attack0);
                break;
        }
        
        _player.Stamina.ConsumeStamina(10f);
        
        _player.Controller.attackAction.actionConsumed = true;
        _player.Controller.attackAction.isDoingAction = true;
        _player.Controller.attackAction.actionFinished = false;
        _player.Controller.attackAction.animationFinished = false;

        /*_player.Controller.attackAction.canChangeAttack = false;

        // SetAnim("Attack" + attackAction.actionAmmount);
        _player.Controller.attackAction.actionAmmount = _player.Controller.attackAction.actionAmmount++;

        _player.Controller.attackAction.actionConsumed = true;

        _player.Controller.attackAction.isDoingAction = true;
        _player.Controller.attackAction.actionFinished = false;
        _player.Controller.attackAction.animationFinished = false;
        */
        _player.Controller.OnActionFinished = () =>
        {
            //_player.Controller.attackAction.actionFinished = true;
            //_player.Controller.attackAction.actionConsumed = false;
            //_player.Controller.attackAction.actionAmmount++;
            _player.Controller.attackAction.actionConsumed = false;
            //_player.ChangeState(States.PlayerIdle);

        };
        _player.Controller.OnAnimationFinished = () =>
        {
            //_player.Controller.attackAction.actionFinished = true;
            //_player.Controller.attackAction.actionConsumed = false;
            //_player.Controller.attackAction.actionAmmount++;
            //_player.ChangeState(States.PlayerIdle);
            //_player.view.ChangeAnim(Animations.Idle);
            //_player.Controller.attackAction.actionAmmount = 0;

            if (_player.view.GetCurrentAnimation() == Animations.Sit)
            {
                //_player.ChangeState(States.PlayerInBonfire);
                return;
            }
            else
                Transitions();
        };

        /*_player.Controller.OnAnimationFinished = () =>
        {
            _player.Controller.attackAction.actionAmmount = 0;
            if (_player.Controller.attackAction.isDoingAction)
                Transitions();
        };*/
    }
    public override void OnExit()
    {
        //_player.Controller.attackAction.actionConsumed = false;
        //_player.Controller.attackAction.isDoingAction = false;
        //_player.Controller.attackAction.actionFinished = true;
        //_player.Controller.attackAction.animationFinished = true;
        _player.Controller.attackAction.actionConsumed = false;
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
        _player.Controller.Walk(inputs, Time.deltaTime,false, true);

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
