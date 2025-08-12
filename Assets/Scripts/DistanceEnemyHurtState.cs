using System.Collections;
using KinematicCharacterController;
using UnityEngine;
public class DistanceEnemyHurtState : IState
{
    private readonly MudEnemyDistance _mudEnemyDistance;
    public DistanceEnemyHurtState(MudEnemyDistance mudEnemyDistance)
    {
        _mudEnemyDistance = mudEnemyDistance;
    }
    public void OnEnter()
    {
        if (_mudEnemyDistance.EnemyPreValidations()) return;
        
        _mudEnemyDistance.PlayAnimation("HurtState");
        _mudEnemyDistance.StartCoroutine(WaitForIdle());
    }
    private IEnumerator WaitForIdle()
    {
        yield return new WaitForSeconds(0.5f);

        _mudEnemyDistance.StateMachine.ChangeState(States.EnemyIdle);
    }
    public void OnExit(){}
    public void OnFixedUpdate(){}
    public void OnUpdate()
    {
        var inputs = new AICharacterInputs
        {
            MoveVector = Vector3.zero
        };
        
        _mudEnemyDistance.Controller.SetInputs(ref inputs);
    }
}