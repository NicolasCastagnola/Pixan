using KinematicCharacterController;
using UnityEngine;
public class GolemEnemySummonMinionsState : IState
{
    private readonly MudGolemEnemy _owner;
    public GolemEnemySummonMinionsState(MudGolemEnemy owner)
    {
        _owner = owner;
    }
    public void OnEnter()
    {
        _owner.AnimatorController.TriggerCast();
    }
    public void OnUpdate()
    {
        var inputs = new AICharacterInputs
        {
            MoveVector = Vector3.zero,
            LookVector = _owner.transform.forward
        };
        
        _owner.Controller.SetInputs(ref inputs);
    }
    public void OnFixedUpdate()
    {
    }
    public void OnExit()
    {
    }
}