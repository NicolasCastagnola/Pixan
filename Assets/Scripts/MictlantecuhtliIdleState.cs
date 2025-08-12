using KinematicCharacterController;
using UnityEngine;
public class MictlantecuhtliIdleState : IState
{
    private readonly Mictlantecuhtli _owner;
    
    public MictlantecuhtliIdleState(Mictlantecuhtli owner)
    {
        _owner = owner;
    }
    public void OnEnter()
    {
        _owner.Animator.ResetTrigger("Punch");
        _owner.Animator.ResetTrigger("Stomp");
        _owner.Animator.ResetTrigger("Death");
        _owner.Animator.ResetTrigger("Intro");


    }
    public void OnUpdate()
    {
        if (_owner.PlayerTarget == null) return;
        
        var inputs = new AICharacterInputs { MoveVector = Vector3.zero };
        
        _owner.Controller.SetInputs(ref inputs);
        
        _owner.AnimatorController.SetLocomotion(0);
        
        if (_owner.PlayerIsValidToAttack())
        {
            _owner.StateMachine.ChangeState(States.EnemyChase);
        }
    }
    public void OnExit()
    {
    }
    public void OnFixedUpdate()
    {
    }
}