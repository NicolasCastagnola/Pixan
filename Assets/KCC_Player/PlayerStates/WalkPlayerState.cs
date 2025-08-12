using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerAnimations;

public class WalkPlayerState : PlayerState
{
    public WalkPlayerState(Player p) : base(p)
    {
    }
    public override void OnEnter()
    {
        if (Transitions()) return;

        _player.view.ChangeAnim(Animations.Walk);

    }
    public override void OnUpdate()
    {
        if (Transitions()) return;

        _player.Controller.Walk(_player.Inputs, Time.deltaTime);
    }

    protected override bool Transitions()
    {
        //_player.Inputs.MoveAxisRight
        if (inputs.MoveAxisRight == 0 && inputs.MoveAxisForward == 0)
        {
            _player.ChangeState(States.PlayerIdle);
            return true;
        }
        // Block input
        if (inputs.BlockDown && !_player.Controller.blockAction.actionConsumed && _player.Stamina.Current > 5f)
        {
            _player.ChangeState(States.PlayerBlock);
            return true;
        }
        // Roll input
        if (inputs.Roll && !_player.Controller.rollAction.actionConsumed && _player.Stamina.Current > 45f)
        {
            _player.ChangeState(States.PlayerRoll);
            return true;
        }
        if (inputs.heal && _player.healItems > 0 && !_player.Controller.healAction.actionConsumed)
        {
            _player.ChangeState(States.PlayerHeal);
            return true;
        }
        // Attack input
        if (inputs.Attack && !_player.Controller.attackAction.actionConsumed && _player.Stamina.Current > 10f)
        {
            _player.ChangeState(States.PlayerAttack);
            return true;
        }
        // Charhed Attack input
        if (inputs.ChargedAttack && !_player.Controller.charhedAttackAction.actionConsumed && _player.Stamina.Current > 20f&&_player.StateMachine.GetCurrentState()!= States.PlayerChargedAttack)
        {
            _player.ChangeState(States.PlayerChargedAttack);
            return true;
        }
        return false;
    }
}