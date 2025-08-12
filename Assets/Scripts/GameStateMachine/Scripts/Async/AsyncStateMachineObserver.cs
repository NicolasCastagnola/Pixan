﻿using System;
using System.Collections.Generic;
using UnityEngine;

public static class AsyncStateMachineObserver
{
    public static Dictionary<AsyncState, Stack<AsyncState>> ActiveStateMachines { get; private set; }= new Dictionary<AsyncState, Stack<AsyncState>>();

    public static event Action OnUpdate;

    [RuntimeInitializeOnLoadMethod]
    public static void Initialize()
    {
        AsyncState.OnAnySwitchState += OnAnySwitchState;
    }

    private static void OnAnySwitchState(AsyncState state)
    {
        if(state == null) return;
        ActiveStateMachines[state] = state.GetStates();
        OnUpdate?.Invoke();
    }
}