using System.Collections;
using System.Collections.Generic;

public interface IStateMachine 
{
    IGameState CurrentState { get; }
    void SwitchState(IGameState nState);
}
