using Audio;
using KinematicCharacterController;
using UnityEngine;
public class MictlantecuhtliChaseState : IState
{
    private readonly Mictlantecuhtli _owner;
    private readonly AudioConfigurationData _audioConfigurationData;

    public MictlantecuhtliChaseState(Mictlantecuhtli owner, AudioConfigurationData audioConfigurationData)
    {
        _owner = owner;
        _audioConfigurationData = audioConfigurationData;
    }
    public void OnEnter()
    {
        //play steps
        // _audioConfigurationData.Play3D();
    }
    public void OnUpdate()
    {
        if (_owner.PlayerTarget == null || _owner.Health.IsDead) return;

        if (_owner.InStompRange())
        {
            _owner.StateMachine.ChangeState(States.StompAttack);
            return;
        }
        if (_owner.InMeleeRange())
        {
            _owner.StateMachine.ChangeState(States.EnemyAttack);
            return;
        }

        ChaseTarget();
    }
    public void OnExit()
    {
    }
    public void OnFixedUpdate()
    {
    }
    
    private void ChaseTarget()
    {
        var targetTransform = _owner.PlayerTarget.CenterOfMassPivot;
        var dir = targetTransform.position - _owner.transform.position;
        
        Debug.DrawRay(_owner.transform.position, dir, Color.red);
        
        var inputs = new AICharacterInputs
        {
            LookVector = new Vector3(dir.normalized.x, 0f, dir.normalized.z),
            MoveVector = dir.normalized * (Time.deltaTime * _owner.EntityStats.rawWalkSpeed) * 1.75f
        };
        
        _owner.AnimatorController.SetLocomotion(1);
        _owner.Controller.SetInputs(ref inputs);
    }
}