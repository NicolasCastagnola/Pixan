using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class StateMachine 
{
    //Coleccion de states
    public string CurrentStateName => _currentState.ToString();
    
    private IState _currentState;//= new BlankState(); 
    private States _currentKeyState;
    private readonly Dictionary<States, IState> _allStates = new Dictionary<States, IState>();

    public List<States> States => _allStates.Select(x => x.Key).ToList();

    public void Update()
    {
        if (_currentState == null)
        {
            if(_allStates.Any(x => x.Key == global::States.PlayerIdle))
            {
                ChangeState(global::States.PlayerIdle);
            }
            else
            {
                ChangeState(global::States.EnemyIdle);
            }
        }
        _currentState?.OnUpdate();
    }

    public void FixedUpdate() => _currentState?.OnFixedUpdate();

    public void AddState(States key, IState state)
    {
        if (_allStates.ContainsKey(key)) return;

        _allStates.Add(key, state);
    }

    public void ChangeState(States key){
        if (!_allStates.ContainsKey(key)) return;
        _currentState?.OnExit();
        _currentState = _allStates[key];
        _currentState.OnEnter();
        _currentKeyState = key;

    }

    public States GetCurrentState()
    {
        return _currentKeyState;
    }
}

//public class BlankState : IState {
//    public void OnEnter(){
//        throw new System.NotImplementedException();
//    }
//
//    public void OnExit(){
//        throw new System.NotImplementedException();
//    }
//
//    public void OnUpdate(){
//        throw new System.NotImplementedException();
//    }
//}