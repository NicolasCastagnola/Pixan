using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerAnimations;

public abstract class PlayerState : IState
{
    protected Player _player;
    protected PlayerInputs inputs { get => _player.Inputs; }

    public PlayerState(Player p)
    {
        _player = p;
    }

    public virtual void OnEnter()
    {
    }

    public virtual void OnUpdate()
    {
    }

    public virtual void OnExit()
    {

    }

    public virtual void OnFixedUpdate()
    {
    }

    protected abstract bool Transitions();
}
