using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class EnemyPauseState<T> : IState where T : Entity
{
    T _entity;

    float animSpeed = 0f;

    public EnemyPauseState(T entity)
    {
        _entity = entity;
    }

    public void OnEnter()
    {
        animSpeed = _entity.Animator.speed;
        _entity.Animator.speed = 0;
        _entity.Motor.enabled = false;
        if(_entity.NavMeshAgent == true)
            _entity.NavMeshAgent.enabled = false;
    }

    public void OnExit()
    {
        _entity.Motor.enabled = true;
        _entity.Animator.speed = animSpeed;
        if (_entity.NavMeshAgent == true)
            _entity.NavMeshAgent.enabled = true;
    }

    public void OnFixedUpdate()
    {

    }

    public void OnUpdate()
    {
        
    }
}
