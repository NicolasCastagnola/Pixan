using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoStateMachine<T> : MonoBehaviour, IStateMachine where T: MonoStateMachine<T>
{
    public IGameState CurrentState { get; private set; }
    
    public void SwitchState(IGameState nState)
    {
        GameManager.ShowLog($"<color=green> {CurrentState}>>>{nState}  </color>");

        CurrentState?.Exit();
        CurrentState = nState;
        CurrentState?.Enter();
    }
}
public abstract class MonoState<T> : IGameState, IStateMachine where T : IStateMachine
{
    protected readonly T Owner;

    protected MonoState(T owner)
    {
        Owner = owner;
    }

    public IGameState CurrentState { get; private set; }
    
    public void SwitchState(IGameState nState)
    {
        GameManager.ShowLog($"<color=green> {CurrentState}>>>{nState}  </color>");
        
        CurrentState?.Exit();
        CurrentState = nState;
        CurrentState?.Enter();
    }

    public virtual void Enter() {}
    public virtual void Exit() {}

}