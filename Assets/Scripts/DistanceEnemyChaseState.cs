using KinematicCharacterController;
using UnityEngine;
public class DistanceEnemyChaseState : IState
{
    private readonly MudEnemyDistance _owner;
    public DistanceEnemyChaseState(MudEnemyDistance owner)
    {
        _owner = owner;
    }
    public void OnEnter()
    {
        if (_owner.EnemyPreValidations()) return;
        
        _owner.PlayAnimation("ChaseState");


        _owner.Controller.Motor.enabled = false;
        _owner.Controller.enabled = false;

        RecalculatePath();
    }

    public void OnExit()
    {
        Vector3 pos = _owner.transform.position;
        Quaternion rot = _owner.transform.rotation;

        _owner.Controller.Motor.enabled = true;
        _owner.Controller.enabled = true;
        _owner.Controller.Motor.SetPositionAndRotation(pos, rot);
    }

    public void OnUpdate()
    {
        if (_owner.EnemyPreValidations() || _owner.PlayerTarget == null) return;
        
        if (!_owner.PlayerWithinMeleeAttackRadius()) _owner.StateMachine.ChangeState(States.EnemyDistanceAttack);

        if (WithinRangeAndInsideFOV()) _owner.StateMachine.ChangeState(States.EnemyAttack);
        
        else ChaseTarget();
    }

    private void ChaseTarget()
    {
        if (_owner.currentCooldown <= 0)
            RecalculatePath();
        else
            _owner.currentCooldown -= Time.deltaTime;

        var player = _owner.PlayerTarget.CenterOfMassPivot.position;
        player.y = _owner.transform.position.y;
        _owner.transform.forward = player - _owner.transform.position;
    }
    void RecalculatePath()
    {
        _owner.currentCooldown = _owner.cooldown;
        _owner.NavMeshAgent.SetDestination(_owner.PlayerTarget.CenterOfMassPivot.position);
    }

    private bool WithinRangeAndInsideFOV()
    {
        var dir = _owner.transform.position - _owner.PlayerTarget.CenterOfMassPivot.position;
        
        if (!(Vector3.Angle(_owner.transform.forward, _owner.PlayerTarget.transform.InverseTransformDirection(dir)) < _owner.FieldOfView)) return false;
            
        return Vector3.Distance(_owner.PlayerTarget.transform.position, _owner.transform.position) < _owner.MeleeAttackTimeInterval;

    }

    public void OnFixedUpdate()
    {
       
    }
}