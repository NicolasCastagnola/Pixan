using KinematicCharacterController;
using UnityEngine;
public class DistanceEnemyStunState : IState
{
    private readonly MudEnemyDistance _mudEnemyDistance;
    private float timer;
    public DistanceEnemyStunState(MudEnemyDistance mudEnemyDistance)
    {
        _mudEnemyDistance = mudEnemyDistance;
    }
    public void OnEnter()
    {
        if (_mudEnemyDistance.EnemyPreValidations()) return;
        _mudEnemyDistance.PlayAnimation("StunState");
        timer = 0;
    }
    public void OnFixedUpdate(){}
    public void OnExit(){}
    public void OnUpdate()
    {
        var inputs = new AICharacterInputs
        {
            MoveVector = Vector3.zero
        };
        
        _mudEnemyDistance.Controller.SetInputs(ref inputs);

        timer += Time.deltaTime;

        if (timer >= 1) _mudEnemyDistance.StateMachine.ChangeState(States.EnemyIdle);
    }
}