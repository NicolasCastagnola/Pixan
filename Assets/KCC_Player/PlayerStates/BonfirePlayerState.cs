using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerAnimations;

public class BonfirePlayerState : PlayerState
{
    public BonfirePlayerState(Player player) : base(player)
    {
    }
    public override void OnEnter()
    {
        _player.Controller.HaltVelocity(Time.deltaTime);
        _player.view.ResetAllBools();
        _player.view.ChangeAnim(Animations.Sit);

        //TODO: Ver como hacer para que la camara no quede torcida
        // var dir = BonfireManager.Instance.LastInteractedBonfire.transform.position - _player.transform.position;
        //
        // _player.Motor.SetRotation(Quaternion.LookRotation(dir.normalized));

        Canvas_Playing.Instance.ShowBonfireMenu(BonfireMenuClosed, BonfireMenuClosed);
    }
    private void BonfireMenuClosed()
    {
        _player.view.ResetAllBools();
        _player.StateMachine.ChangeState(States.PlayerIdle);
    }
    public override void OnUpdate(){ 

        Cursor.visible = true; 
        Cursor.lockState = CursorLockMode.Confined;


        //_player.StateMachine.ChangeState(States.PlayerInBonfire);

        //if(!_player.view.AnimIsPlaying(Animations.Sit))
        //{
        //    _player.view.ResetAllBools();
        //    _player.view.ChangeAnim(Animations.Sit);
        //}
    }
    protected override bool Transitions() => true;
}