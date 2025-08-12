using PlayerAnimations;
using UnityEngine;

public class HealSelfState : PlayerState
{
    public HealSelfState(Player player) : base(player)
    {
    }
    public override void OnEnter()
    {
        _player.Controller.healAction.actionConsumed = true;
        _player.view.ChangeAnim(Animations.Heal);

        _player.Controller.OnActionFinished = () =>
        {
        };

        _player.Controller.OnAnimationFinished = () =>
        {
            _player.Controller.healAction.actionConsumed = false;

            _player.UpdateHealItem(-1);
            _player.Health.Heal(_player.cureAmount, _player.gameObject);

            if (_player.view.GetCurrentAnimation() == Animations.Sit)
            {
                return;
            }
            else
                _player.ChangeState(States.PlayerIdle);
        };
    }
    public override void OnUpdate()
    {
        base.OnUpdate();
        _player.Controller.Walk(default, Time.deltaTime);
    }
    public override void OnExit()
    {
        base.OnExit();
        _player.Controller.healAction.actionConsumed = false;
    }
    protected override bool Transitions() => true;
}
