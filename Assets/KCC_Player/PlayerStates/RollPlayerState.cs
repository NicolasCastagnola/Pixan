using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerAnimations;

public class RollPlayerState : PlayerState
{
    public RollPlayerState(Player p) : base(p)
    {
    }
    public override void OnEnter()
    {

        _player.Stamina.ConsumeStamina(15f);
        _player.view.ChangeAnim(Animations.Roll);
        _player.Controller.rollAction.isDoingAction = true;
        _player.Controller.rollAction.actionFinished = false;
        _player.Controller.rollAction.actionConsumed = true;
        _player.Controller.OnActionFinished = () =>
        {
            _player.Controller.rollAction.actionFinished = true;
        };
        _player.Controller.OnAnimationFinished = () =>
        {
            if (_player.view.GetCurrentAnimation() == Animations.Sit)
            {
                return;
            }
            else
                Transitions();

            _player.StartCoroutine(WaitForActionConsumed());
        };
    }
    private IEnumerator WaitForActionConsumed()
    {
        yield return new WaitForSeconds(0.2f);
        
        _player.Controller.rollAction.actionConsumed = false;
    }
    
    public override void OnExit()
    {

        _player.Controller.rollAction.isDoingAction = false;
        _player.Controller.rollAction.actionFinished = true;
        _player.StartCoroutine(WaitForActionConsumed());
        _player.EnableDamage();

        if (_player.view.GetCurrentAnimation() == Animations.Sit)
        {
            _player.ChangeState(States.PlayerInBonfire);
            return;
        }
    }
    public override void OnUpdate()
    {
        _player.Controller.Roll(_player.Controller.rollAction.actionSpeed * 50);
        //if (_player.character.rollAction.animationFinished) Transitions();
    }

    protected override bool Transitions()
    {
        _player.ChangeState(States.PlayerIdle);
        return true;
    }
}