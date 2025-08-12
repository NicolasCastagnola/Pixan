using KinematicCharacterController;
using UnityEngine;
public class DistanceEnemyMeleeAttackState : IState
{
    private readonly MudEnemyDistance _mudEnemyDistance;
    private readonly float _attackTimeInterval;
    private bool isAttacking;
    private float timerToMove;
    private readonly float maxTime;

    public DistanceEnemyMeleeAttackState(MudEnemyDistance mudEnemyDistance, float attackTimeInterval)
    {
        _mudEnemyDistance = mudEnemyDistance;
        _attackTimeInterval = attackTimeInterval;
    }
    public void OnEnter()
    {
        if (_mudEnemyDistance.EnemyPreValidations()) return;

        _mudEnemyDistance.PlayAnimation("AttackState");

        isAttacking = true;
        timerToMove = 0;
    }
    public void OnExit() => _mudEnemyDistance.MeleeHitBoxColliderDamage.Deactivate();
    public void OnUpdate()
    {
        var inputs = new AICharacterInputs
        {
            MoveVector = Vector3.zero
        };

        _mudEnemyDistance.Controller.SetInputs(ref inputs);

        if (_mudEnemyDistance.EnemyPreValidations()) return;

        else if (_mudEnemyDistance.PlayerTarget.isDead) _mudEnemyDistance.StateMachine.ChangeState(States.EnemyIdle);

        var direction = _mudEnemyDistance.PlayerTarget.transform.position - _mudEnemyDistance.transform.position;
        var targetRotation = Quaternion.LookRotation(direction.normalized);
        targetRotation.x = 0;
        targetRotation.z = 0;
        _mudEnemyDistance.Motor.RotateCharacter(targetRotation);


        if (isAttacking)
        {
            timerToMove += Time.deltaTime;

            if (timerToMove >= maxTime)
            {
                isAttacking = false;
                timerToMove = 0;
            }

            return;
        }

        if (!_mudEnemyDistance.PlayerWithinMeleeAttackRadius() && _mudEnemyDistance.StateMachine.States.Contains(States.EnemyIdle))
        {
            _mudEnemyDistance.StateMachine.ChangeState(States.EnemyIdle);
        }
    }

    public void OnFixedUpdate()
    {
    }
}