using KinematicCharacterController;
using UnityEngine;
public class MictlantecuhtliDeathState : IState
{
    private readonly Mictlantecuhtli _owner;
    public MictlantecuhtliDeathState(Mictlantecuhtli owner) => _owner = owner;
    public void OnEnter()
    {
        _owner.AnimatorController.TriggerDeath();
        
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
    public void OnExit(){}
    public void OnFixedUpdate(){}
}