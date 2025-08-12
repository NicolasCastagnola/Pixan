using GameStateMachine;
using GameStateMachine.States;
using UnityEngine;

[DefaultExecutionOrder(100)]
public class RootStateInitializer : MonoBehaviour
{
    private void Start() => RootState.Initialize();
}
