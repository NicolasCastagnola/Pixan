using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemEnemyArenaSlowState : IState
{
    private readonly MudGolemEnemy _enemy;
    public GolemEnemyArenaSlowState(MudGolemEnemy enemy)
    {
        _enemy = enemy;
    }
    public void OnEnter(){}
    public void OnExit(){}
    public void OnUpdate(){}
    public void OnFixedUpdate(){}
}
