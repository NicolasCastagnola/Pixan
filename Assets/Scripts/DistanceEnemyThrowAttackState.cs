using System.Collections;
using UnityEngine;
public class DistanceEnemyThrowAttackState : IState
{
    private readonly MudEnemyDistance _mudEnemyDistance;
    private readonly float _attackTimeInterval;
    private bool shouldThrow = true;
    
    public DistanceEnemyThrowAttackState(MudEnemyDistance mudEnemyDistance, float attackTimeInterval)
    {
        _mudEnemyDistance = mudEnemyDistance;
        _attackTimeInterval = attackTimeInterval;
    }
    public void OnUpdate()
    {
        if (_mudEnemyDistance.EnemyPreValidations() || _mudEnemyDistance.PlayerTarget == null) return;

        if (_mudEnemyDistance.PlayerWithinMeleeAttackRadius())
        {
            _mudEnemyDistance.StateMachine.ChangeState(States.EnemyAttack);
        }
        //else if (_mudEnemyDistance.PlayerWithinDistanceAttackRadius() && shouldThrow)
        //{
        //    _mudEnemyDistance.StartCoroutine(ShouldThrow());
        //}

        var direction = _mudEnemyDistance.PlayerTarget.transform.position - _mudEnemyDistance.transform.position;
        var targetRotation = Quaternion.LookRotation(direction.normalized);
        targetRotation.x = 0;
        targetRotation.z = 0;
        _mudEnemyDistance.Motor.RotateCharacter(targetRotation);
    }
    //private IEnumerator ShouldThrow()
    //{
    //    _mudEnemyDistance.AnimatorController.TriggerThrow();
    //    
    //    shouldThrow = false;
    //    
    //    yield return new WaitForSeconds(_attackTimeInterval);
    //    
    //    shouldThrow = true;
    //}
    public void OnEnter(){
        _mudEnemyDistance.AnimatorController.TriggerThrow();
    }
    public void OnExit(){
            
    }
    public void OnFixedUpdate(){}
}